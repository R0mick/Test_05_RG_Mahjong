using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Tiles;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Board
{
    public class BoardGenerator
    {

        public Dictionary<Vector3, TileDataSo>
            GenerateBoard(Vector3[] positions,
                List<TileDataSo> tilesSo) //todo remove ve3 from return ? we have board position in ITile
        {
            Dictionary<Vector3, TileDataSo> boardDictionary = new();

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

            if (tilesSo.Count < tileCount / 2)
            {
                throw new Exception(
                    "Not enough TileDataSo types to generate pairs. Check TileDataSo types in storage.");
            }

            if (!CheckOverlapsInBlueprint(positions))
            {
                throw new Exception("Positions are overlaps in blueprint.");
            }


            //generate tile so pairs
            List<TileDataSo> firstElements = tilesSo.Take(tileCount / 2).ToList();

            List<TileDataSo> tilePairsRandomizeList = new List<TileDataSo>();

            tilePairsRandomizeList.AddRange(firstElements);
            tilePairsRandomizeList.AddRange(firstElements);

            tilePairsRandomizeList = tilePairsRandomizeList.OrderBy(x => Random.Range(0f, 1f)).ToList();

            //generate board
            if (positions.Length != tilePairsRandomizeList.Count)
                throw new System.ArgumentException("Positions and Tiles must have the same length");


            for (int i = 0; i < positions.Length; i++)
            {
                boardDictionary.Add(positions[i], tilePairsRandomizeList[i]);
            }


            Debug.Log("Total tiles generated "+ tilePairsRandomizeList.Count);

            return boardDictionary;
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