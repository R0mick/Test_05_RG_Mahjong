
using UnityEngine;

namespace _Scripts.Tiles
{
    /// <summary>
    /// Used for most checks. (virtual and physical tiles)
    /// </summary>
    public interface ITile
    {
        Vector3 BoardPosition{get;set;}
        bool IsOpen { get; set; }
        
         string TileName{ get; set; }
         
    }
}