using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Tiles
{
    [CreateAssetMenu(fileName = "TileDataSo", menuName = "ScriptableObject/TileDataSo")]
    public class TileDataSo:ScriptableObject,ITile
    {

        public Sprite tileSprite;
        
        public string tileName;
        public Vector3 BoardPosition { get; set; }
        public bool IsOpen { get; set; }

        public string TileName
        {
            get => tileName;
            set => tileName = value;
        }


        private void OnValidate()
        {
            #if UNITY_EDITOR
            tileName = tileSprite.name;
            UnityEditor.EditorUtility.SetDirty(this);
            #endif
        }
    }
}