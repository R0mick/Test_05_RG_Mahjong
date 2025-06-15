using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Scripts.Board
{
    /// <summary>
    /// Validates figure before Board Manager can use for board beneration.
    /// </summary>
    public class FigureValidator
    {

        public List<Vector3> ValidatePositions(Vector3[] positions)
        {
            List<Vector3> boardList = new();

            //checks
            int tileCount = positions.Length;
            if (tileCount % 2 != 0)
            {
                throw new Exception("Positions count must be even. Check blueprint.");
            }

            if (positions.Distinct().Count() != positions.Length)
            {
                throw new Exception("Positions must be unique. Check positions.");
            }
            

            if (!CheckOverlapsInBlueprint(positions))
            {
                throw new Exception("Positions are overlaps in blueprint.");
            }
            

            for (int i = 0; i < positions.Length; i++)
            {
                boardList.Add(positions[i]);
            }
            
            Debug.Log("Total positions generated " + positions.Length);
            
            return boardList;
        }

        private bool CheckOverlapsInBlueprint(Vector3[] positions)
        {
            bool isValid = true;

            // Check multiply by 0.5
            for (int i = 0; i < positions.Length; i++)
            {
                Vector3 pos = positions[i];
                bool isPositionValid = true;

                if (!IsStrictlyMultipleOfHalf(pos.x))
                {
                    Debug.LogError($"Position {i}: X = {pos.x} not multiple of 0.5");
                    isPositionValid = false;
                }

                if (!IsStrictlyMultipleOfHalf(pos.y))
                {
                    Debug.LogError($"Position {i}: Y = {pos.y} not multiple of 0.5");
                    isPositionValid = false;
                }

                if (!IsStrictlyMultipleOfHalf(pos.z))
                {
                    Debug.LogError($"Position {i}: Z = {pos.z} not multiple of 0.5");
                    isPositionValid = false;
                }

                if (!isPositionValid)
                {
                    Debug.LogError($"Invalid position at index {i}: {pos}");
                    isValid = false;
                }
            }

            // group by z
            var layers = positions
                .Select((pos, index) => new { Position = pos, Index = index })
                .GroupBy(x => x.Position.z);

            foreach (var layer in layers)
            {
                var layerPositions = layer.ToArray();
                float z = layer.Key;

                for (int i = 0; i < layerPositions.Length; i++)
                {
                    for (int j = i + 1; j < layerPositions.Length; j++)
                    {
                        Vector3 pos1 = layerPositions[i].Position;
                        Vector3 pos2 = layerPositions[j].Position;

                        // check x with equal y
                        if (Mathf.Approximately(pos1.y, pos2.y))
                        {
                            float xDistance = Mathf.Abs(pos1.x - pos2.x);
                            if (xDistance < 1f)
                            {
                                Debug.LogError($"Positions too close in layer Z={z} at Y={pos1.y}: " +
                                               $"[{layerPositions[i].Index}]={pos1} and " +
                                               $"[{layerPositions[j].Index}]={pos2} (distance: {xDistance})");
                                isValid = false;
                            }
                        }

                        // check y with equal x
                        if (Mathf.Approximately(pos1.x, pos2.x))
                        {
                            float yDistance = Mathf.Abs(pos1.y - pos2.y);
                            if (yDistance < 1f)
                            {
                                Debug.LogError($"Positions too close in layer Z={z} at X={pos1.x}: " +
                                               $"[{layerPositions[i].Index}]={pos1} and " +
                                               $"[{layerPositions[j].Index}]={pos2} (distance: {yDistance})");
                                isValid = false;
                            }
                        }
                    }
                }
            }

            return isValid;
        }

        // Check multiply by 0.5
        private bool IsStrictlyMultipleOfHalf(float value)
        {
            return Mathf.RoundToInt(value * 2) == value * 2;
        }
    }
}