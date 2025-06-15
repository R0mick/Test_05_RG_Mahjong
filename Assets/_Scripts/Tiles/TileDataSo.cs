using UnityEditor;
using UnityEngine;

namespace _Scripts.Tiles
{
    /// <summary>
    /// Contains info for tiles.
    /// </summary>
    [CreateAssetMenu(fileName = "TileDataSo", menuName = "ScriptableObject/TileDataSo")]
    public class TileDataSo:ScriptableObject
    {
        [SerializeField] Sprite tileSprite;
        [SerializeField] string tileName;

        public string TileName
        {
            get => tileName;
            set => tileName = value;
        }
        public Sprite TileSprite => tileSprite;

        private void OnValidate()
        {
            #if UNITY_EDITOR
            tileName = tileSprite.name;
            EditorUtility.SetDirty(this);
            #endif
        }
    }
}