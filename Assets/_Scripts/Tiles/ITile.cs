
using UnityEngine;

namespace _Scripts.Tiles
{
    public interface ITile
    {
        Vector3 BoardPosition{get;set;}
        bool IsOpen { get; set; }
        
         string TileName{ get; set; }
         
    }
}