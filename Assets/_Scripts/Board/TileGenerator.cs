using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Managers;
using _Scripts.Tiles;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Board
{
    public class TileGenerator : MonoBehaviour
    {
        private List<GameObject> _tilesList = new List<GameObject>();
        
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private Vector3 tileOffset;
        [SerializeField] private GameObject tileContainer;

        public List<GameObject> GenerateTiles(List<Vector3> positionsList, List<TileDataSo> tilesSoList)
        {
            
            int tileId = 0;
            
            foreach (var position in positionsList)
            {
                //add offset
                Vector3 spawnPosition = new Vector3(
                    position.x *  tileOffset.x,
                    position.y *  tileOffset.y *-1,
                    position.z *  tileOffset.z *-1
                );
                
                //instantiate
                GameObject tileGo = Instantiate(tilePrefab, spawnPosition, Quaternion.identity);
                
                //set stats
                tileGo.GetComponent<Tile>().SetSprite(tilesSoList[tileId].tileSprite);
                tileGo.GetComponent<Tile>().TileName = tilesSoList[tileId].tileName;
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