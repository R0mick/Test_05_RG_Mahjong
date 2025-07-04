﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Board;
using _Scripts.Tiles;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Managers
{
    /// <summary>
    /// Main class. Generates solvable boards, controls tiles etc.
    /// </summary>
    public class BoardManager : MonoBehaviour
    {
        
        [Header("Tiles SO")] 
        [SerializeField] private List<TileDataSo> tilesSoList = new();

        [Header("References")] 
        [SerializeField] private TileGenerator tileGenerator;

        [Header("Figure")]
        [SerializeField] private BoardFigureBlueprints.Figures selectedFigure;
        [SerializeField] private int rebuildAttempts;
        
        
        private List<GameObject> _tilesGameObjects;
        private bool _isInAutoSolveMode;
        private bool _isBoardGenerating;

        private List<TileDataSo> _tilesRandomPairsList = new();
        private int _minimumTilesLeft;
        
        
        private void OnEnable()
        {
            SimpleEventManager.Instance.OnRebuildLevelRequest += GenerateSolvableBoard;
            SimpleEventManager.Instance.OnAutoSolveRequest += AutoSolveBoard;
            SimpleEventManager.Instance.OnRemoveTileRequest += RemoveTileGo;
            SimpleEventManager.Instance.OnGetTilesCount += GetTilesGoCount;
            SimpleEventManager.Instance.OnUpdateTilesOnBoardIsOpenRequest += UpdateTilesGoIsOpenStatus;
        }

        private void OnDisable()
        {
            SimpleEventManager.Instance.OnRebuildLevelRequest -= GenerateSolvableBoard;
            SimpleEventManager.Instance.OnAutoSolveRequest -= AutoSolveBoard;
            SimpleEventManager.Instance.OnRemoveTileRequest -= RemoveTileGo;
            SimpleEventManager.Instance.OnGetTilesCount -= GetTilesGoCount;
            SimpleEventManager.Instance.OnUpdateTilesOnBoardIsOpenRequest -= UpdateTilesGoIsOpenStatus;
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
            
            SimpleEventManager.Instance.SetBoardSolvedStatus(false);
            
            if (_tilesGameObjects != null)
            {
                foreach (var go in _tilesGameObjects)
                {
                    Destroy(go);
                }
                _tilesGameObjects.Clear();
                SimpleEventManager.Instance.ClearSelectedTilesRequest();
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
            
            List<Vector3> validatedPositions = new List<Vector3>();
            FigureValidator validator = new FigureValidator();
            
            int currentAttempt = 0;
            int batchSize = 30;
            for (int attempt = 0; attempt < rebuildAttempts; attempt++)
            {
                Debug.Log("Attempt "+attempt);
                
                validatedPositions = validator.ValidatePositions(blueprint);

                bool isSolvable = IsBoardSolvable(validatedPositions);
                
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
            
            _tilesGameObjects = tileGenerator.GenerateTiles(validatedPositions, _tilesRandomPairsList);

            UpdateTilesGoIsOpenStatus();

            _isBoardGenerating = false;
            SimpleEventManager.Instance.RebuildLevelComplete();
        }


        private bool IsBoardSolvable(List<Vector3> positions)
        {
           
            /*var virtualTiles = positions
                .Select(pair => new VirtualTile(pair.Value.tileName, pair.Key))
                .Cast<ITile>()
                .ToList();*/

            List<ITile> virtualTiles = new List<ITile>();

            _tilesRandomPairsList = TilePairsRandomizeList(positions, tilesSoList);
            
            for (int i = 0; i < positions.Count; i++)
            {
                virtualTiles.Add(new VirtualTile(_tilesRandomPairsList[i].TileName, positions[i]));
                
            }


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


        private void AutoSolveBoard()
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

                UpdateTilesGoIsOpenStatus();
                //Debug.Log(_tilesGameObjects.Count);
                yield return new WaitForSeconds(0.5f);
            }
            SimpleEventManager.Instance.UpdateInputBlockStatus(false);
            SimpleEventManager.Instance.SetBoardSolvedStatus(true);
            _isInAutoSolveMode = false;
            Debug.Log("Autosolve board finished");
        }
        
         
        private void UpdateTilesGoIsOpenStatus()
        {
            List<ITile> iTiles = _tilesGameObjects
                .Select(go => go.GetComponent<ITile>())
                .Where(tile => tile != null)
                .ToList();


            foreach (var tileGameObject in _tilesGameObjects)
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


        private List<TileDataSo> TilePairsRandomizeList(List<Vector3> positionsList, List<TileDataSo> tilesSoList)
        {
            //check available tileSo count
            if (tilesSoList.Count < positionsList.Count / 2)
            {
                throw new Exception(
                    "Not enough TileDataSo types to generate pairs. Check TileDataSo types in storage.");
            }
            
            //generate tile so pairs
            List<TileDataSo> firstElements = tilesSoList.Take(positionsList.Count / 2).ToList();

            List<TileDataSo> tilePairsRandomizeList = new List<TileDataSo>();

            tilePairsRandomizeList.AddRange(firstElements);
            tilePairsRandomizeList.AddRange(firstElements);

            tilePairsRandomizeList = tilePairsRandomizeList.OrderBy(x => Random.Range(0f, 1f)).ToList();
            return tilePairsRandomizeList;
        }

        private void RemoveTileGo(GameObject tileGo)
        {
            _tilesGameObjects.Remove(tileGo);
            Destroy(tileGo);
        }
        
        private int GetTilesGoCount()
        {
            return _tilesGameObjects.Count;
        }

    }
}