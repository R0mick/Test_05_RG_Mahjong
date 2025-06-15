using System.Collections;
using _Scripts.Tiles;
using UnityEngine;

namespace _Scripts.Managers
{
    /// <summary>
    /// Initializes game start. Manages manual game sequence.
    /// </summary>
    public class GameManager : MonoBehaviour
    {


        [Header("Technical")] 
        [SerializeField] private Tile firstSelectedTile;
        [SerializeField] private Tile secondSelectedTile;
        
        
        
        private void OnEnable()
        {
            SimpleEventManager.Instance.OnTileClicked += CompareTiles;
            SimpleEventManager.Instance.OnClearSelectedTilesRequest += ClearSelectedTiles;
        }

        private void OnDisable()
        {
            SimpleEventManager.Instance.OnTileClicked -= CompareTiles;
            SimpleEventManager.Instance.OnClearSelectedTilesRequest -= ClearSelectedTiles;
        }


        private void Start()
        {
            SimpleEventManager.Instance.RebuildLevelRequest();
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

            StartCoroutine(CompareTiles());
        }

        private IEnumerator CompareTiles()
        {
            SimpleEventManager.Instance.UpdateInputBlockStatus(true);
            
            yield return new WaitForSeconds(0.2f); 

            if (firstSelectedTile.TileName != secondSelectedTile.TileName)
            {
                
                firstSelectedTile.DisplayMissMatch();
                secondSelectedTile.DisplayMissMatch();
                Debug.Log("tiles are not equal");
                
                yield return new WaitForSeconds(0.2f);
                
                firstSelectedTile.ToggleSelected();
                secondSelectedTile.ToggleSelected();
            }
            else
            {
                firstSelectedTile.DisplayMatch();
                secondSelectedTile.DisplayMatch();
                
                Debug.Log("tiles are equal");
                
                yield return new WaitForSeconds(0.2f);
                
                SimpleEventManager.Instance.RemoveTileRequest(firstSelectedTile.gameObject);
                SimpleEventManager.Instance.RemoveTileRequest(secondSelectedTile.gameObject);
            }

            ClearSelectedTiles();
            SimpleEventManager.Instance.UpdateInputBlockStatus(false);

            int tilesLeft = SimpleEventManager.Instance.GetTilesCount();
            if (tilesLeft > 0)
            {
                SimpleEventManager.Instance.UpdateTilesOnBoardIsOpenRequest();
            }
            else
            {
                SimpleEventManager.Instance.SetBoardSolvedStatus(true);
            }
        }

        private void ClearSelectedTiles()
        {
            firstSelectedTile = null;
            secondSelectedTile = null;
        }

    }
}