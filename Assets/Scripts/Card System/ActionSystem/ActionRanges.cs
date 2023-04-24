using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionRange
{
    Melee,
    Ranged,
    Reach,
    Global,
}

public static class ActionRanges
{
    public static List<Tile> GetValidTiles(Vector2Int currentPosition, bool isPlayer1, List<bool> validTargets, ActionRange actionRange)
    {
        List<Tile> validTiles = new List<Tile>();
        List<Tile> sameRowTiles = new List<Tile>();

        int xMin = 0;
        int xMax = 5;

        if (actionRange != ActionRange.Global)
        {
            xMin = isPlayer1 ? Mathf.Max(currentPosition.x - 1, 0) : Mathf.Max(currentPosition.x - 2, 0);
            xMax = isPlayer1 ? Mathf.Min(currentPosition.x + 2, 5) : Mathf.Min(currentPosition.x + 1, 5);
        }

        if (isPlayer1)
        {
            for (int x = xMax; x >= xMin; x--)
            {
                AddValidTilesForColumn(currentPosition, isPlayer1, validTargets, actionRange, validTiles, sameRowTiles, x);
            }
        }
        else
        {
            for (int x = xMin; x <= xMax; x++)
            {
                AddValidTilesForColumn(currentPosition, isPlayer1, validTargets, actionRange, validTiles, sameRowTiles, x);
            }
        }

        // Prioritize tiles on the same row by adding them first
        validTiles.InsertRange(0, sameRowTiles);

        Debug.Log($"ActionRanges.GetValidTiles: {DebugTools.ListToString(validTiles)}");

        return validTiles;
    }

    private static void AddValidTilesForColumn(Vector2Int currentPosition, bool isPlayer1, List<bool> validTargets, ActionRange actionRange, List<Tile> validTiles, List<Tile> sameRowTiles, int x)
    {
        bool targetsEnemies = validTargets[0];
        bool targetsAllies = validTargets[1];
        bool targetsSelf = validTargets[2];

        for (int yOffset = -1; yOffset <= 1; yOffset++)
        {
            int y = currentPosition.y + yOffset;

            if (y >= 0 && y <= 4)
            {
                Tile tile = GridManager.Instance.Grid[x, y];
                bool tileValid = false;

                if (tile.GridPosition == currentPosition && targetsSelf)
                {
                    tileValid = true;
                }
                else if (tile.GetIsPlayer1 != isPlayer1 && targetsEnemies)
                {
                    tileValid = true;
                }
                else if (tile.GetIsPlayer1 == isPlayer1 && targetsAllies && tile.GridPosition != currentPosition)
                {
                    tileValid = true;
                }

                if (tileValid)
                {
                    if (actionRange == ActionRange.Melee || actionRange == ActionRange.Reach || actionRange == ActionRange.Global)
                    {
                        if (yOffset == 0)
                        {
                            sameRowTiles.Add(tile);
                        }
                        else
                        {
                            validTiles.Add(tile);
                        }
                    }
                    else if (actionRange == ActionRange.Ranged && IsValidRangedTile(currentPosition, isPlayer1, tile))
                    {
                        if (yOffset == 0)
                        {
                            sameRowTiles.Add(tile);
                        }
                        else
                        {
                            validTiles.Add(tile);
                        }
                    }
                }
            }
        }
    }

    private static bool IsValidRangedTile(Vector2Int currentPosition, bool isPlayer1, Tile tile)
    {
        int xDistance = Mathf.Abs(currentPosition.x - tile.GridPosition.x);
        int yDistance = Mathf.Abs(currentPosition.y - tile.GridPosition.y);

        if (xDistance <= 2 && yDistance <= 1)
        {
            return true;
        }

        return false;
    }

    public static List<Tile> Melee(Vector2Int currentPosition, bool isPlayer1, List<bool> validTargets)
    {
        return GetValidTiles(currentPosition, isPlayer1, validTargets, ActionRange.Melee);
    }

    public static List<Tile> Reach(Vector2Int currentPosition, bool isPlayer1, List<bool> validTargets)
    {
        return GetValidTiles(currentPosition, isPlayer1, validTargets, ActionRange.Reach);
    }

    public static List<Tile> Ranged(Vector2Int currentPosition, bool isPlayer1, List<bool> validTargets)
    {
        return GetValidTiles(currentPosition, isPlayer1, validTargets, ActionRange.Ranged);
    }

    public static List<Tile> Global(Vector2Int currentPosition, bool isPlayer1, List<bool> validTargets)
    {
        return GetValidTiles(currentPosition, isPlayer1, validTargets, ActionRange.Global);
    }
}