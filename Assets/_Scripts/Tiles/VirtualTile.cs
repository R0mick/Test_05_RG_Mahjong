using UnityEngine;

namespace _Scripts.Tiles
{
    /// <summary>
    /// For check if generated board is solvable
    /// </summary>
    public class VirtualTile: ITile
    {
        public string TileName { get; set; }
        public Vector3 BoardPosition { get; set; }
        public bool IsOpen { get; set; }

        public VirtualTile(string name, Vector3 position)
        {
            TileName = name;
            BoardPosition = position;
            IsOpen = false;
        }
    }
}