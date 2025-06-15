using System.Collections.Generic;
using _Scripts.Managers;
using _Scripts.Tiles;
using UnityEngine;

namespace _Scripts.Board
{
    /// <summary>
    /// Generates tiles go on the scene.
    /// </summary>
    public class TileGenerator : MonoBehaviour
    {
        private List<GameObject> _tilesList = new();
        
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private Vector3 tileOffset;
        [SerializeField] private GameObject tileContainer;

        public List<GameObject> GenerateTiles(List<Vector3> positionsList, List<TileDataSo> tilesSoList)
        {
            
            int tileId = 0;
            
            foreach (var position in positionsList)
            {
                
                //revers y and z to match positions grid
                Vector3 spawnPosition = new Vector3(
                    position.x *  tileOffset.x,
                    position.y *  tileOffset.y *-1,
                    position.z *  tileOffset.z *-1
                );
                
                //add little offset for the layers to compensate their slide
                if (position.z > 0)
                {
                    spawnPosition.x += position.z * 0.1f;
                    spawnPosition.y += position.z * 0.1f;
                }
                
                //instantiate
                GameObject tileGo = Instantiate(tilePrefab, spawnPosition, Quaternion.identity);
                
                //set stats
                tileGo.GetComponent<Tile>().SetSprite(tilesSoList[tileId].TileSprite);
                tileGo.GetComponent<Tile>().TileName = tilesSoList[tileId].TileName;
                tileGo.GetComponent<Tile>().TileId = tileId;
                tileGo.GetComponent<Tile>().BoardPosition = position;
                
                
                tileGo.transform.SetParent(tileContainer.transform);
                _tilesList.Add(tileGo);
                
                tileId++;
            }
            
            SimpleEventManager.Instance.CenterCameraRequest(_tilesList);
            
            return _tilesList;
        }
    }
}