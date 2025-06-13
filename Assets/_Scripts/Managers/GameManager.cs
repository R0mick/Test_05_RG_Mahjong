using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Board;
using _Scripts.Tiles;
using UnityEngine;

namespace _Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        [Header("Tiles SO")] 
        [SerializeField] private List<TileDataSo> tilesSoList = new();

        [Header("References")] 
        [SerializeField] private TileGenerator tileGenerator;


        [Header("Technical")] 
        [SerializeField] private Tile firstSelectedTile;
        [SerializeField] private Tile secondSelectedTile;
        
        [Header("Figure")]
        [SerializeField] private BoardFigureBlueprints.Figures selectedFigure;
        [SerializeField] private int rebuildAttempts;


        private List<GameObject> _tilesGameObjects;
        private bool _isInAutoSolveMode;
        private bool _isBoardGenerating;

        private void OnEnable()
        {
            SimpleEventManager.Instance.OnTileClicked += CompareTiles;
            SimpleEventManager.Instance.OnRebuildLevelRequest += GenerateSolvableBoard;
            SimpleEventManager.Instance.OnAutoSolveRequest += AutoSolveBoard;
        }

        private void OnDisable()
        {
            SimpleEventManager.Instance.OnTileClicked -= CompareTiles;
            SimpleEventManager.Instance.OnRebuildLevelRequest -= GenerateSolvableBoard;
            SimpleEventManager.Instance.OnAutoSolveRequest -= AutoSolveBoard;
        }


        private void Start()
        {
            GenerateSolvableBoard();
        }


        private void GenerateSolvableBoard()
        {
            if (_isInAutoSolveMode)
            {
                Debug.Log("Solving Board in process!");
                return;
            }

            if (_isBoardGenerating)
            {
                Debug.Log("Board generation in process!");
                return;
            }

            _isBoardGenerating = true;
            var cor = StartCoroutine(GenerateSolvableBoardCoroutine());

        }


        private IEnumerator GenerateSolvableBoardCoroutine()
        {
            
            SimpleEventManager.Instance.BoardSolvedStatus(false);
            
            if (_tilesGameObjects != null)
            {
                foreach (var go in _tilesGameObjects)
                {
                    Destroy(go);
                }
                _tilesGameObjects.Clear();
                firstSelectedTile = null;
                secondSelectedTile = null;
            }

            Vector3[] blueprint;

            switch (selectedFigure)
            {
                case BoardFigureBlueprints.Figures.First:
                {
                    blueprint = BoardFigureBlueprints.FirstBlueprint;
                    break;
                }
                case BoardFigureBlueprints.Figures.Test:
                {
                    blueprint = BoardFigureBlueprints.TestBlueprint;
                    break;
                }
                default:
                {
                    blueprint = BoardFigureBlueprints.TestBlueprint;
                    break;
                }
            }


            BoardGenerator generator = new BoardGenerator();

            Dictionary<Vector3, TileDataSo> generatedBoard = new Dictionary<Vector3, TileDataSo>();
            
            int currentAttempt = 0;
            int batchSize = 30;
            for (int attempt = 0; attempt < rebuildAttempts; attempt++)
            {
                Debug.Log("Attempt "+attempt);
                
                generatedBoard = generator.GenerateBoard(blueprint, tilesSoList);

                bool isSolvable = IsBoardSolvable(generatedBoard);
                
                Debug.Log("Generated board solvable status" + isSolvable);
                
                
                if (isSolvable)
                {
                    break;
                }

                if (attempt == rebuildAttempts - 1)
                {
                    Debug.Log("Minimum tiles left from all generations "+ _minimumTilesLeft);
                    throw new Exception($"Cannot generate solvable board with {attempt+1} attempts");
                }

                currentAttempt++;
                //setting attempts count per frame
                if (attempt % batchSize == 0)
                {
                    yield return null;
                }
            }
            

            Debug.Log($"Attempts took to generate solvable board {currentAttempt+1}");
            
            _tilesGameObjects = tileGenerator.GenerateTiles(generatedBoard);

            UpdateTilesIsOpenStatus(_tilesGameObjects);

            _isBoardGenerating = false;
            SimpleEventManager.Instance.RebuildLevelComplete();
        }

        private int _minimumTilesLeft;
        private bool IsBoardSolvable(Dictionary<Vector3, TileDataSo> board)
        {
           
            var virtualTiles = board
                .Select(pair => new VirtualTile(pair.Value.tileName, pair.Key))
                .Cast<ITile>()
                .ToList();


            while (virtualTiles.Any())
            {


                //check isOpen
                foreach (var tile in virtualTiles)
                {
                    tile.IsOpen = IsTileAvailable(virtualTiles, tile);

                    //Debug.Log(tile.BoardPosition);
                    //Debug.Log(tile.TileName);
                    //Debug.Log(tile.IsOpen);
                    //Debug.Log("Is available " + IsTileAvailable(virtualTiles, tile));
                }
                
                
                //remove matched virtual tiles if exist
                List<ITile> matchingTiles = GetFirstAvailableMatchingTiles(virtualTiles);

                if (matchingTiles != null)
                {
                    RemoveAvailableMatchingTiles(matchingTiles, virtualTiles);
                    //Debug.Log("Tiles left "+ virtualTiles.Count);
                    if (_minimumTilesLeft > virtualTiles.Count)
                    {
                        _minimumTilesLeft = virtualTiles.Count;
                    }
                }
                else
                {
                    //Debug.Log("No pairs available");
                    //Debug.Log(virtualTiles.Count);
                    return false;
                }

            }

            return true;
        }

        private List<ITile> GetFirstAvailableMatchingTiles(List<ITile> iTiles)
        {
            iTiles.Sort((a, b) => String.Compare(a.TileName, b.TileName, StringComparison.Ordinal));

            for (int i = 0; i < iTiles.Count - 1; i++)
            {
                if (!iTiles[i].IsOpen || !iTiles[i + 1].IsOpen)
                    continue;

                if (iTiles[i].TileName == iTiles[i + 1].TileName)
                {
                    return new List<ITile> { iTiles[i], iTiles[i + 1] };
                }
            }

            return null;
        }

        private void RemoveAvailableMatchingTiles(List<ITile> listWithMatchingTiles, List<ITile> tilesToRemoveFrom)
        {
            foreach (var tile in listWithMatchingTiles)
            {
                tilesToRemoveFrom.Remove(tile);
            }
        }
        

        public void AutoSolveBoard()
        {
            if (_isInAutoSolveMode)
            {
                Debug.Log("Solving Board in process!");
                return;
            }
            
            if (_tilesGameObjects.Count == 0)
            {
                Debug.Log("No tiles on board");
                return;
            }

            SimpleEventManager.Instance.UpdateInputBlockStatus(true);
            _isInAutoSolveMode = true;
            

            var cor = StartCoroutine(AutoSolveBoardCoroutine());
            
        }

        private IEnumerator AutoSolveBoardCoroutine()
        {
            while (_tilesGameObjects.Any())
            {


                List<ITile> iTiles = _tilesGameObjects
                    .Select(go => go.GetComponent<ITile>())
                    .Where(tile => tile != null)
                    .ToList();

                var matchingTiles = GetFirstAvailableMatchingTiles(iTiles);


                var tilesToRemove = new List<GameObject>();

                foreach (var tile in matchingTiles)
                {
                    var tileGameObject = _tilesGameObjects.FirstOrDefault(go =>
                    {
                        var tileComponent = go.GetComponent<ITile>();
                        return tileComponent != null && tileComponent.BoardPosition == tile.BoardPosition;
                    });

                    if (tileGameObject != null)
                    {
                        tilesToRemove.Add(tileGameObject);
                    }
                }

                foreach (var go in tilesToRemove)
                {
                    _tilesGameObjects.Remove(go);
                    Destroy(go);
                }

                UpdateTilesIsOpenStatus(_tilesGameObjects);
                //Debug.Log(_tilesGameObjects.Count);
                yield return new WaitForSeconds(1f);
            }
            SimpleEventManager.Instance.UpdateInputBlockStatus(false);
            SimpleEventManager.Instance.BoardSolvedStatus(true);
            _isInAutoSolveMode = false;
            Debug.Log("Autosolve board finished");
        }


        private void UpdateTilesIsOpenStatus(List<GameObject> tileGameObjects)
        {
            List<ITile> iTiles = tileGameObjects
                .Select(go => go.GetComponent<ITile>())
                .Where(tile => tile != null)
                .ToList();


            foreach (var tileGameObject in tileGameObjects) //todo extract method to use in solvable and autosolve..?
            {
                Tile tile = tileGameObject.GetComponent<Tile>();

                bool isAvailable = IsTileAvailable(iTiles, tile);
                tileGameObject.GetComponent<Tile>().IsOpen = isAvailable;
                //Debug.Log(isAvailable);
            }

        }


        private bool IsTileAvailable(List<ITile> iTilesList, ITile iTile)
        {

            //check sides //todo add half border?
            bool hasLeft = iTilesList.Any(tile => tile.BoardPosition == iTile.BoardPosition + Vector3.left);
            bool hasRight = iTilesList.Any(tile => tile.BoardPosition == iTile.BoardPosition + Vector3.right);

            //check up
            bool hasUp = IsCoveredAbove(iTilesList, iTile);

            if (hasLeft && hasRight)
            {
                return false;
            }

            if (hasUp)
            {
                return false;
            }

            return true;
        }
        
        private static bool IsCoveredAbove(List<ITile> allTiles, ITile currentTile)
        {
            Vector3 currentPos = currentTile.BoardPosition;

            // All positions above for cover
            Vector3[] offsets =
            {
                new Vector3( 0f,   0f,   1),
                new Vector3( 0.5f, 0f,   1),
                new Vector3(-0.5f, 0f,   1),
                new Vector3( 0f,   0.5f, 1),
                new Vector3( 0f,  -0.5f, 1),
                new Vector3( 0.5f, 0.5f, 1),
                new Vector3( 0.5f,-0.5f, 1),
                new Vector3(-0.5f, 0.5f, 1),
                new Vector3(-0.5f,-0.5f, 1),
            };
            
            float epsilon = 0.01f;
            
            foreach (var offset in offsets)
            {
                Vector3 checkPosition = currentPos + offset;

                
                foreach (var tile in allTiles)
                {
                    Vector3 tilePos = tile.BoardPosition;

                    bool isCloseEnough =
                        Mathf.Abs(tilePos.x - checkPosition.x) < epsilon &&
                        Mathf.Abs(tilePos.y - checkPosition.y) < epsilon &&
                        Mathf.Abs(tilePos.z - checkPosition.z) < epsilon;

                    if (isCloseEnough)
                        return true;
                }
            }

            return false;
        }

        private void CompareTiles(Tile tile)
        {
            Debug.Log(tile.TileId);
            Debug.Log(tile.TileName);

            if (firstSelectedTile == null)
            {
                firstSelectedTile = tile;
                Debug.Log("add tile sa first");
                return;
            }

            if (firstSelectedTile == tile)
            {
                firstSelectedTile = null;
                Debug.Log("same tile.. clear");
                return;
            }

            if (firstSelectedTile != tile)
            {
                secondSelectedTile = tile;
                Debug.Log("add tile as second");
            }

            if (firstSelectedTile != null && secondSelectedTile != null)
            {
                Debug.Log("Compare tiles");
                if (firstSelectedTile.TileName != secondSelectedTile.TileName)
                {
                    firstSelectedTile.ToggleSelected();
                    secondSelectedTile.ToggleSelected();
                    Debug.Log("Tiles are not match");
                }
                else
                {
                    Debug.Log("Tiles are equal");
                    _tilesGameObjects.Remove(firstSelectedTile.gameObject);
                    _tilesGameObjects.Remove(secondSelectedTile.gameObject);
                    Destroy(firstSelectedTile.gameObject);
                    Destroy(secondSelectedTile.gameObject);
                }

                firstSelectedTile = null;
                secondSelectedTile = null;
                //Debug.Log(_tilesGameObjects.Count);
                UpdateTilesIsOpenStatus(_tilesGameObjects);

                if (_tilesGameObjects.Count == 0)
                {
                    SimpleEventManager.Instance.BoardSolvedStatus(true);
                }
            }


        }

    }
}