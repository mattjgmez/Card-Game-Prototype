using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ActionRanges_Simulated
{
    public static List<TileData> GetValidTiles(GameState gameState, Vector2Int currentPosition, bool isPlayer1, List<bool> validTargets, ActionRange actionRange)
    {
        List<TileData> validTiles = new List<TileData>();
        List<TileData> sameRowTiles = new List<TileData>();

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
                    TileData tile = gameState.Grid[x, y];
                    bool tileValid = false;

                    if (tile.GridPosition == currentPosition && targetsSelf)
                    {
                        tileValid = true;
                    }
                    else if (tile.ActiveCard != null && tile.ActiveCard.IsPlayer1 != isPlayer1 && targetsEnemies)
                    {
                        tileValid = true;
                    }
                    else if (tile.ActiveCard != null && tile.ActiveCard.IsPlayer1 == isPlayer1 && targetsAllies && tile.GridPosition != currentPosition)
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

        validTiles.InsertRange(0, sameRowTiles);
        return validTiles;
    }

    private static bool IsValidRangedTile(Vector2Int currentPosition, bool isPlayer1, TileData tile)
    {
        int xDistance = Mathf.Abs(currentPosition.x - tile.GridPosition.x);
        int yDistance = Mathf.Abs(currentPosition.y - tile.GridPosition.y);

        if (xDistance <= 2 && yDistance <= 1)
        {
            return true;
        }

        return false;
    }

    public static List<TileData> Melee(GameState gameState, Vector2Int currentPosition, bool isPlayer1, List<bool> validTargets)
    {
        return GetValidTiles(gameState, currentPosition, isPlayer1, validTargets, ActionRange.Melee);
    }

    public static List<TileData> Reach(GameState gameState, Vector2Int currentPosition, bool isPlayer1, List<bool> validTargets)
    {
        return GetValidTiles(gameState, currentPosition, isPlayer1, validTargets, ActionRange.Reach);
    }

    public static List<TileData> Ranged(GameState gameState, Vector2Int currentPosition, bool isPlayer1, List<bool> validTargets)
    {
        return GetValidTiles(gameState, currentPosition, isPlayer1, validTargets, ActionRange.Ranged);
    }

    public static List<TileData> Global(GameState gameState, Vector2Int currentPosition, bool isPlayer1, List<bool> validTargets)
    {
        return GetValidTiles(gameState, currentPosition, isPlayer1, validTargets, ActionRange.Global);
    }
}