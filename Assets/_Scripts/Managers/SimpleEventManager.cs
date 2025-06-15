using System;
using System.Collections.Generic;
using _Scripts.Tiles;
using UnityEngine;

namespace _Scripts.Managers
{
    /// <summary>
    /// Simple event bus for events transition between components.
    /// </summary>
    public class SimpleEventManager:MonoBehaviour
    {
        public static SimpleEventManager Instance;

        private void Awake()
        {
            Instance = this;
        }
        
        
        public event Action<Tile> OnTileClicked;
        public void TileClicked(Tile tile)
        {
            OnTileClicked?.Invoke(tile);
        }
        
        public event Action<List<GameObject>> OnCenterCameraRequest;
        public void CenterCameraRequest(List<GameObject> tilesGoList)
        {
            OnCenterCameraRequest?.Invoke(tilesGoList);
        }
        
        public event Action OnRebuildLevelRequest;
        public void RebuildLevelRequest()
        {
            OnRebuildLevelRequest?.Invoke();
        }
        
        public event Action OnRebuildLevelComplete;
        public void RebuildLevelComplete()
        {
            OnRebuildLevelComplete?.Invoke();
        }
        
        public event Action OnAutoSolveRequest;
        public void AutoSolveRequest()
        {
            OnAutoSolveRequest?.Invoke();
        }
        
        public event Action<bool> OnUpdateInputBlockStatus;
        public void UpdateInputBlockStatus(bool isBlocked)
        {
            OnUpdateInputBlockStatus?.Invoke(isBlocked);
        }
        
        public event Action<bool> OnSetBoardSolvedStatus;
        public void SetBoardSolvedStatus(bool isSolved)
        {
            OnSetBoardSolvedStatus?.Invoke(isSolved);
        }
        
        public event Action<GameObject> OnRemoveTileRequest;
        public void RemoveTileRequest(GameObject tile)
        {
            OnRemoveTileRequest?.Invoke(tile);
        }
        
        public event Action OnClearSelectedTilesRequest;
        public void ClearSelectedTilesRequest()
        {
            OnClearSelectedTilesRequest?.Invoke();
        }
        public event Action OnUpdateTilesOnBoardIsOpenRequest;
        public void UpdateTilesOnBoardIsOpenRequest()
        {
            OnUpdateTilesOnBoardIsOpenRequest?.Invoke();
        }
        
        public event Func<int> OnGetTilesCount;
        public int GetTilesCount()
        {
            if (OnGetTilesCount == null)
            {
                return -1;
            }
            return OnGetTilesCount.Invoke();
        }
        
    }
}