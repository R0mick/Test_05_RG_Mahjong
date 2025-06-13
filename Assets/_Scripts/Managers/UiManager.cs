using System;
using TMPro;
using UnityEngine;

namespace _Scripts.Managers
{
    public class UiManager:MonoBehaviour
    {
        [SerializeField] GameObject winMessage;


        private void OnEnable()
        {
            SimpleEventManager.Instance.OnBoardSolvedStatus += SetBoardSolvedStatus;
        }
        
        private void OnDisable()
        {
            SimpleEventManager.Instance.OnBoardSolvedStatus -= SetBoardSolvedStatus;
        }

        public void RebuildLevel()
        {
            SimpleEventManager.Instance.RebuildLevelRequest();
        }

        private void SetBoardSolvedStatus(bool solved)
        {
            winMessage.SetActive(solved);
        }
        
    }
}