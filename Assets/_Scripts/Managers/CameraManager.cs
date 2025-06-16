using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Managers
{
    /// <summary>
    /// Automatically aligns to the center of the figure on board.
    /// </summary>
    public class CameraManager:MonoBehaviour
    {
        private void OnEnable()
        {
            SimpleEventManager.Instance.OnCenterCameraRequest += CenterCameraRequestOnTiles;
        }
        
        private void OnDisable()
        {
            SimpleEventManager.Instance.OnCenterCameraRequest -= CenterCameraRequestOnTiles;
        }

        private void CenterCameraRequestOnTiles(List<GameObject> tilesList)
        {
            if (tilesList.Count == 0) return;

            // get figure borders
            float minX = float.MaxValue, maxX = float.MinValue;
            float minY = float.MaxValue, maxY = float.MinValue;

            foreach (GameObject tile in tilesList)
            {
                Vector3 pos = tile.transform.position;
                minX = Mathf.Min(minX, pos.x);
                maxX = Mathf.Max(maxX, pos.x);
                minY = Mathf.Min(minY, pos.y);
                maxY = Mathf.Max(maxY, pos.y);
            }

            // get center of figure
            float centerX = (minX + maxX) / 2f;
            float centerY = (minY + maxY) / 2f;
            
            float screenRatio = (float)Screen.width / Screen.height;
            
            if (Camera.main != null)
            {
                Camera.main.transform.position = new Vector3(centerX, centerY, -10);

                // set camera position
                float width = maxX - minX;
                float height = maxY - minY;
                //float requiredSize = Mathf.Max(width, height) * 0.9f;
                float requiredSize = (Mathf.Max(width, height) /3f) + screenRatio; //was found empirically

                Camera.main.orthographicSize = requiredSize;
                
            }
        }
    }
}