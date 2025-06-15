using UnityEngine;

namespace _Scripts.Board
{
    /// <summary>
    /// Used to create figure blueprint via V3 coordinates
    /// </summary>
    public static class BoardFigureBlueprints
    {

        public enum Figures
        {
            First,
            Test
        }

        // First
        public static readonly Vector3[] FirstBlueprint = new Vector3[]
        {

            //layer 1 
            new(0, 0, 0),
            new(1, 0, 0),
            //new(2, 0, 0),
            new(3, 0, 0),
            new(4, 0, 0),
            //new(5, 0, 0),
            new(6, 0, 0),
            new(7, 0, 0),

            new(0, 1, 0),
            new(1, 1, 0),
            new(2, 1, 0),
            new(3, 1, 0),
            new(4, 1, 0),
            new(5, 1, 0),
            new(6, 1, 0),
            new(7, 1, 0),

            new(0, 2, 0),
            new(1, 2, 0),
            new(2, 2, 0),
            new(3, 2, 0),
            new(4, 2, 0),
            new(5, 2, 0),
            new(6, 2, 0),
            new(7, 2, 0),

            new(0, 3, 0),
            new(1, 3, 0),
            //new(2, 3, 0),
            new(3, 3, 0),
            new(4, 3, 0),
            //new(5, 3, 0),
            new(6, 3, 0),
            new(7, 3, 0),
            
            //layer 2 
            new(0.5f, 0.5f, 1),
            new(0.5f, 1.5f, 1),
            new(0.5f, 2.5f, 1),
            
            new(6.5f, 0.5f, 1),
            new(6.5f, 1.5f, 1),
            new(6.5f, 2.5f, 1),
            
            new(3f, 0f, 1),
            new(3f, 1f, 1),
            new(3f, 2f, 1),
            new(3f, 3f, 1),
            
            new(4f, 0f, 1),
            new(4f, 1f, 1),
            new(4f, 2f, 1),
            new(4f, 3f, 1),
            
            //layer 3
            new(0.5f, 1f, 2),
            new(0.5f, 2f, 2),
            
            new(1.5f, 1f, 2),
            new(1.5f, 2f, 2),
            
          
            new(3.5f, 1f, 2),
            new(3.5f, 2f, 2),
            
            
            new(5.5f, 1f, 2),
            new(5.5f, 2f, 2),
            
            new(6.5f, 1f, 2),
            new(6.5f, 2f, 2),
            
            

        };

        // Test
        public static readonly Vector3[] TestBlueprint = new Vector3[]
        {

            //layer 1 
            new(0, 0, 0),
            new(1, 0, 0),
            new(2, 0, 0),
            new(3, 0, 0),
            
            new(0, 1, 0),
            new(1, 1, 0),
            new(2, 1, 0),
            new(3, 1, 0),
            
            new(0, 2, 0),
            new(1, 2, 0),
            new(2, 2, 0),
            new(3, 2, 0),
            
            new(0, 3, 0),
            new(1, 3, 0),
            new(2, 3, 0),
            new(3, 3, 0),
            
            //layer 2 
            new(0, 0, 1),
            new(0, 1, 1),
            
            
            new(2.5f, 0.5f, 1),
            new(2.5f, 2.5f, 1),
        };
    }

}
