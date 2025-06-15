using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Managers
{
    /// <summary>
    /// Contains references for ui content. Manages button clicks.
    /// </summary>
    public class UiManager:MonoBehaviour
    {
        [SerializeField] private GameObject winMessage;
        [SerializeField] private GameObject rebuildIcon;
        [SerializeField] private Button rebuildButton;
        [SerializeField] private Button autoSolveButton;

        private Coroutine _rebuildCoroutine;
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

        private void Start()
        {
            _rebuildCoroutine = StartCoroutine(LoadingSpinCoroutine());
        }

        public void RebuildLevel()
        {
            SetRebuildAnimationStatus(true);
            //rebuildIcon.SetActive(true);
            rebuildButton.interactable = false;
            autoSolveButton.interactable = false;
            SimpleEventManager.Instance.RebuildLevelRequest();
        }
        
        public void AutoSolveLevel()
        {
            rebuildButton.interactable = false;
            autoSolveButton.interactable = false;
            SimpleEventManager.Instance.AutoSolveRequest();
        }

        private void SetSetBoardSolvedStatus(bool solved)
        {
            winMessage.SetActive(solved);
            if (solved)
            {
                rebuildButton.interactable = true;
            }
        }

        private void RebuildComplete()
        {
            rebuildButton.interactable = true;
            autoSolveButton.interactable = true;
            SetRebuildAnimationStatus(false);
            //rebuildIcon.SetActive(false);
        }

        private void SetRebuildAnimationStatus(bool isActive)
        {
            if (isActive)
            {
                rebuildIcon.SetActive(true);
                _rebuildCoroutine = StartCoroutine(LoadingSpinCoroutine());
            }
            else
            {
                if (_rebuildCoroutine != null)
                {
                    StopCoroutine(_rebuildCoroutine);
                }

                rebuildIcon.SetActive(false);
            }
            
        }
        
        private IEnumerator LoadingSpinCoroutine()
        {
            Debug.Log("LoadingSpinCoroutine");
            rebuildIcon.transform.localRotation = Quaternion.Euler(0,0,0);
            while (true)
            {
                
                rebuildIcon.transform.Rotate(0f, 0f, -45f * Time.deltaTime);
                yield return new Null();
            }
        }
        
    }
}