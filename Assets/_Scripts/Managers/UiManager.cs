using System;
using TMPro;
using UnityEngine;

namespace _Scripts.Managers
{
    public class UiManager:MonoBehaviour
    {
        [SerializeField] GameObject winMessage;
        [SerializeField] GameObject rebuildIcon;


        private void OnEnable()
        {
            SimpleEventManager.Instance.OnBoardSolvedStatus += SetBoardSolvedStatus;
            SimpleEventManager.Instance.OnRebuildLevelComplete += RebuildComplete;
        }
        
        private void OnDisable()
        {
            SimpleEventManager.Instance.OnBoardSolvedStatus -= SetBoardSolvedStatus;
            SimpleEventManager.Instance.OnRebuildLevelComplete -= RebuildComplete;
        }

        public void RebuildLevel()
        {
            rebuildIcon.SetActive(true);
            SimpleEventManager.Instance.RebuildLevelRequest();
        }

        private void SetBoardSolvedStatus(bool solved)
        {
            winMessage.SetActive(solved);
        }

        private void RebuildComplete()
        {
            rebuildIcon.SetActive(false);
        }
        
    }
}