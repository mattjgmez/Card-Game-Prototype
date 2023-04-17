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
    /// <summary>
    /// Returns a list of valid tiles based on the current position, player, valid target criteria, and action range,
    /// prioritizing tiles on the same row as the given card.
    /// </summary>
    /// <param name="currentPosition">The current position of the character performing the action.</param>
    /// <param name="isPlayer1">Boolean indicating if the character belongs to Player 1.</param>
    /// <param name="validTargets">List of booleans representing valid target types (enemies, allies, self).</param>
    /// <param name="actionRange">The range of the action being performed (Melee, Reach, Ranged, or Global).</param>
    /// <returns>A list of valid Tile objects.</returns>
    public static List<Tile> GetValidTiles(Vector2Int currentPosition, bool isPlayer1, List<bool> validTargets, ActionRange actionRange)
    {
        List<Tile> validTiles = new List<Tile>();
        List<Tile> sameRowTiles = new List<Tile>();

        bool targetsEnemies = validTargets[0];
        bool targetsAllies = validTargets[1];
        bool targetsSelf = validTargets[2];

        int xMin = 0;
        int xMax = 5;

        if (actionRange != ActionRange.Global)
        {
            xMin = isPlayer1 ? Mathf.Max(currentPosition.x - 1, 0) : Mathf.Max(currentPosition.x - 2, 0);
            xMax = isPlayer1 ? Mathf.Min(currentPosition.x + 2, 5) : Mathf.Min(currentPosition.x + 1, 5);
        }

        for (int x = xMin; x <= xMax; x++)
        {
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

        // Prioritize tiles on the same row by adding them first
        validTiles.InsertRange(0, sameRowTiles);
        ActionSystem.DebugList(validTiles);
        return validTiles;
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
        return GetTilesInRange(currentPosition, isPlayer1, validTargets, int.MaxValue);
    }

    private static List<Tile> GetTilesInRange(Vector2Int currentPosition, bool isPlayer1, List<bool> validTargets, int range)
    {
        List<Tile> validTiles = new List<Tile>();

        int[] rowPriority = { currentPosition.y, currentPosition.y + 1, currentPosition.y - 1 };

        for (int i = 0; i < 3; i++)
        {
            if (rowPriority[i] >= 0 && rowPriority[i] < 5)
            {
                int startX = isPlayer1 ? 2 : 3;
                int endX = isPlayer1 ? -1 : 6;
                int stepX = isPlayer1 ? -1 : 1;

                for (int x = startX; x != endX; x += stepX)
                {
                    if (Mathf.Abs(x - currentPosition.x) <= range)
                    {
                        Tile tile = GridManager.Instance.Grid[x, rowPriority[i]];

                        if (IsValidTile(currentPosition, isPlayer1, validTargets, tile))
                        {
                            validTiles.Add(tile);
                        }
                    }
                }
            }
        }

        return validTiles;
    }

    private static bool IsValidTile(Vector2Int currentPosition, bool isPlayer1, List<bool> validTargets, Tile tile)
    {
        bool targetsEnemies = validTargets[0];
        bool targetsAllies = validTargets[1];
        bool targetsSelf = validTargets[2];

        if (tile.GridPosition == currentPosition && targetsSelf)
        {
            return true;
        }
        else if (tile.GetIsPlayer1 != isPlayer1 && targetsEnemies)
        {
            return true;
        }
        else if (tile.GetIsPlayer1 == isPlayer1 && targetsAllies && tile.GridPosition != currentPosition)
        {
            return true;
        }

        return false;
    }
}