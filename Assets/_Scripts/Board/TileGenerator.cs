using System.Collections.Generic;
using _Scripts.Managers;
using _Scripts.Tiles;
using UnityEngine;

namespace _Scripts.Board
{
    public class TileGenerator : MonoBehaviour
    {
        private List<GameObject> _tilesList = new List<GameObject>();
        
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private Vector3 tileOffset;
        [SerializeField] private GameObject tileContainer;

        public List<GameObject> GenerateTiles(Dictionary<Vector3, TileDataSo> boardDictionary)
        {
            int tileId = 0;
            
            foreach (var pair in boardDictionary)
            {
                //add offset
                Vector3 spawnPosition = new Vector3(
                    pair.Key.x *  tileOffset.x,
                    pair.Key.y *  tileOffset.y *-1,
                    pair.Key.z *  tileOffset.z *-1
                );
                
                //instantiate
                GameObject tileGo = Instantiate(tilePrefab, spawnPosition, Quaternion.identity);
                
                //set stats
                tileGo.GetComponent<Tile>().SetSprite(pair.Value.tileSprite);
                tileGo.GetComponent<Tile>().TileName = pair.Value.tileName;
                tileGo.GetComponent<Tile>().TileId = tileId;
                tileGo.GetComponent<Tile>().BoardPosition = pair.Key;
                
                
                tileGo.transform.SetParent(tileContainer.transform);
                _tilesList.Add(tileGo);
                
                tileId++;
            }
            
            SimpleEventManager.Instance.CenterCameraRequest(_tilesList);
            
            return _tilesList;
        }
    }
}