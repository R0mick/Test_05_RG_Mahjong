using UnityEngine;

namespace _Scripts.Managers
{
    /// <summary>
    /// Contains references for ui content. Manages button clicks.
    /// </summary>
    public class UiManager:MonoBehaviour
    {
        [SerializeField] private GameObject winMessage;
        [SerializeField] private GameObject rebuildIcon;


        private void OnEnable()
        {
            SimpleEventManager.Instance.OnSetBoardSolvedStatus += SetSetBoardSolvedStatus;
            SimpleEventManager.Instance.OnRebuildLevelComplete += RebuildComplete;
        }
        
        private void OnDisable()
        {
            SimpleEventManager.Instance.OnSetBoardSolvedStatus -= SetSetBoardSolvedStatus;
            SimpleEventManager.Instance.OnRebuildLevelComplete -= RebuildComplete;
        }

        public void RebuildLevel()
        {
            rebuildIcon.SetActive(true);
            SimpleEventManager.Instance.RebuildLevelRequest();
        }
        
        public void AutoSolveLevel()
        {
            SimpleEventManager.Instance.AutoSolveRequest();
        }

        private void SetSetBoardSolvedStatus(bool solved)
        {
            winMessage.SetActive(solved);
        }

        private void RebuildComplete()
        {
            rebuildIcon.SetActive(false);
        }
        
    }
}