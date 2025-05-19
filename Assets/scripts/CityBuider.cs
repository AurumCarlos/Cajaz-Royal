using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CityBuider
{
    private static List<(int, int)> BatSize = new List<(int, int)>();
    private static bool[][] roadMap;
    
    public static void Initialize(bool[][] roads)
    {
        roadMap = roads;
        BatSize.Clear();
        BatSize.Add((8,9)); 

    }

    public static List<((int, int), (int, int))> BindBatPlace()
    {
        var possiblePlacements = new List<((int, int), (int, int))>();
        
        if (roadMap == null) return possiblePlacements;
        
        for (int row = 0; row < roadMap.Length; row++)
        {
            for (int col = 0; col < roadMap[0].Length; col++)
            {
                foreach (var size in BatSize)
                {
                    if (CanPlaceBuildingAt(row, col, size.Item1, size.Item2))
                    {
                        possiblePlacements.Add(((row, col), size));
                        col += size.Item1 - 1;
                        break;
                    }
                }
            }
        }
        return possiblePlacements;
    }

    public static bool CanPlaceBuildingAt(int row, int col, int width, int height)
    {
        if (row < 0 || col < 0) return false;
        if (row >= roadMap.Length - 1) return false;  
        if (col >= roadMap[0].Length - width) return false;

        for (int i = row; i >= row - height + 1; i--)
        {
            if (i < 0) return false;
            for (int j = col; j < col + width; j++)
            {
                if (j >= roadMap[0].Length) return false;
                if (roadMap[i][j])
                {
                    return false;
                }
            }
        }

        bool hasAdjacentRoad = false;

        if (row + 1 < roadMap.Length)
        {
            for (int j = col; j < col + width; j++)
            {
                if (roadMap[row + 1][j])
                {
                    hasAdjacentRoad = true;
                    break;
                }
            }
        }

        return hasAdjacentRoad;
    }
}
