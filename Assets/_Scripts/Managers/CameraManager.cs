using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Managers
{
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

        public void CenterCameraRequestOnTiles(List<GameObject> tilesList)
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

            
            if (Camera.main != null)
            {
                Camera.main.transform.position = new Vector3(centerX, centerY, -10);

                // set camera position
                float width = maxX - minX;
                float height = maxY - minY;
                float requiredSize = Mathf.Max(width, height) * 0.9f;

                Camera.main.orthographicSize = requiredSize;
                
                //Debug.Log(requiredSize);
                
                //add angle
                /*var xPositionCompensation = requiredSize * 4.5f;
                Camera.main.transform.rotation = Quaternion.Euler(0, 8, 0);
                Camera.main.transform.position = new Vector3(0, Camera.main.transform.position.y, -xPositionCompensation);
                */
                
                var zPositionCompensation = requiredSize * 4.5f;
                var yPositionCompensation = requiredSize *1.3f;
                var xPositionCompensation = requiredSize * 0.3f;
                Camera.main.transform.rotation = Quaternion.Euler(-10, 10, -1.8f);
                Camera.main.transform.position = new Vector3(-xPositionCompensation, -yPositionCompensation, -zPositionCompensation);
            }
        }
    }
}