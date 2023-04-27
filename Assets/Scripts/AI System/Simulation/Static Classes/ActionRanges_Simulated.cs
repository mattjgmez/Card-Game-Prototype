using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ActionRanges_Simulated
{
    public static List<int> YOffsets = new List<int>() { 0, -1, 1 };

    public static List<TileData> GetValidTargets(GameState gameState, UnitCardData unit, ActionRange range, List<bool> validTargets)
    {
        List<TileData> targets = new List<TileData>();
        bool isPlayer1 = unit.IsPlayer1;

        int startX = isPlayer1 ? 2 : 3;
        int direction = isPlayer1 ? 1 : -1;

        switch (range)
        {
            case ActionRange.Melee:
                targets = GetMeleeTargets(gameState, unit, startX, direction, validTargets);
                break;
            case ActionRange.Ranged:
                targets = GetRangedTargets(gameState, unit, startX, direction, validTargets);
                break;
            case ActionRange.Reach:
                targets = GetReachTargets(gameState, unit, startX, direction, validTargets);
                break;
            case ActionRange.Global:
                targets = GetGlobalTargets(gameState, unit, startX, direction, validTargets);
                break;
        }

        return targets;
    }

    private static List<TileData> GetMeleeTargets(GameState gameState, UnitCardData unit, int startX, int direction, List<bool> validTargets)
    {
        List<TileData> targets = new List<TileData>();
        Vector2Int currentPosition = unit.CurrentTile.GridPosition;

        for (int offsetX = 0; offsetX <= 1; offsetX++)
        {
            foreach (int offsetY in YOffsets)
            {
                int newX = currentPosition.x + (offsetX * direction);
                int newY = currentPosition.y + offsetY;

                if (IsValidTile(newX, newY) && IsValidTarget(gameState, unit, newX, newY, validTargets))
                {
                    targets.Add(gameState.Grid[newX, newY]);
                }
            }
        }

        return targets;
    }

    private static List<TileData> GetRangedTargets(GameState gameState, UnitCardData unit, int startX, int direction, List<bool> validTargets)
    {
        List<TileData> targets = new List<TileData>();
        Vector2Int currentPosition = unit.CurrentTile.GridPosition;

        for (int offsetX = 0; offsetX < 3; offsetX++)
        {
            foreach (int offsetY in YOffsets)
            {
                int newX = startX + (offsetX * direction);
                int newY = currentPosition.y + offsetY;

                if (IsValidTile(newX, newY) && IsValidTarget(gameState, unit, newX, newY, validTargets))
                {
                    targets.Add(gameState.Grid[newX, newY]);
                }
            }
        }

        return targets;
    }

    private static List<TileData> GetReachTargets(GameState gameState, UnitCardData unit, int startX, int direction, List<bool> validTargets)
    {
        List<TileData> targets = new List<TileData>();
        Vector2Int currentPosition = unit.CurrentTile.GridPosition;

        for (int offsetX = 0; offsetX <= 2; offsetX++)
        {
            foreach (int offsetY in YOffsets)
            {
                int newX = startX + (offsetX * direction);
                int newY = currentPosition.y + offsetY;

                if (IsValidTile(newX, newY) && IsValidTarget(gameState, unit, newX, newY, validTargets))
                {
                    targets.Add(gameState.Grid[newX, newY]);
                }
            }
        }

        return targets;
    }

    private static List<TileData> GetGlobalTargets(GameState gameState, UnitCardData unit, int startX, int direction, List<bool> validTargets)
    {
        List<TileData> targets = new List<TileData>();
        Vector2Int currentPosition = unit.CurrentTile.GridPosition;

        for (int offsetX = 0; offsetX < 6; offsetX++)
        {
            foreach (int offsetY in YOffsets)
            {
                int newX = startX + (offsetX * direction);
                int newY = currentPosition.y + offsetY;

                if (IsValidTile(newX, newY) && IsValidTarget(gameState, unit, newX, newY, validTargets))
                {
                    targets.Add(gameState.Grid[newX, newY]);
                }
            }
        }

        return targets;
    }

    private static bool IsValidTile(int x, int y)
    {
        return x >= 0 && x < 6 && y >= 0 && y < 5;
    }

    private static bool IsValidTarget(GameState gameState, UnitCardData unit, int x, int y, List<bool> validTargets)
    {
        TileData targetTile = gameState.Grid[x, y];

        if (targetTile.ActiveCard == null) return false;

        bool targetsEnemies = validTargets[0];
        bool targetsAllies = validTargets[1];
        bool targetsSelf = validTargets[2];

        if (targetsSelf && unit == targetTile.ActiveCard)
        {
            return true;
        }
        else if (targetsAllies && unit.IsPlayer1 == targetTile.ActiveCard.IsPlayer1)
        {
            return true;
        }
        else if (targetsEnemies && unit.IsPlayer1 != targetTile.ActiveCard.IsPlayer1)
        {
            return true;
        }

        return false;
    }
}