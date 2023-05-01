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
    public static List<int> YOffsets = new List<int>() { 0, -1, 1 };
    public static List<int> XOrder_Player1 = new List<int> { 3, 4, 5, 2, 1, 0 };
    public static List<int> XOrder_Player2 = new List<int> { 2, 1, 0, 3, 4, 5 };

    public static List<Tile> GetValidTargets(UnitCard unit, ActionRange range, List<bool> validTargets)
    {
        List<Tile> targets = new List<Tile>();

        switch (range)
        {
            case ActionRange.Melee:
                targets = GetMeleeTargets(unit, validTargets);
                break;
            case ActionRange.Ranged:
                targets = GetRangedTargets(unit, validTargets);
                break;
            case ActionRange.Reach:
                targets = GetReachTargets(unit, validTargets);
                break;
            case ActionRange.Global:
                targets = GetGlobalTargets(unit, validTargets);
                break;
        }

        return targets;
    }

    private static List<Tile> GetMeleeTargets(UnitCard unit, List<bool> validTargets)
    {
        List<Tile> targets = new List<Tile>();
        Vector2Int currentPosition = unit.CurrentTile.GridPosition;
        List<int> xOrder = unit.IsPlayer1 ? XOrder_Player1 : XOrder_Player2;

        foreach (int x in xOrder)
        {
            if (Mathf.Abs(x - currentPosition.x) > 1)
            {
                continue;
            }

            foreach (int offsetY in YOffsets)
            {
                int newY = currentPosition.y + offsetY;

                if (IsValidTile(x, newY) && IsValidTarget(unit, x, newY, validTargets))
                {
                    targets.Add(GridManager.Instance.Grid[x, newY]);
                }
            }
        }

        return targets;
    }

    private static List<Tile> GetRangedTargets(UnitCard unit, List<bool> validTargets)
    {
        List<Tile> targets = new List<Tile>();
        Vector2Int currentPosition = unit.CurrentTile.GridPosition;
        List<int> xOrder = unit.IsPlayer1 ? XOrder_Player1 : XOrder_Player2;

        foreach (int x in xOrder)
        {
            if (Mathf.Abs(x - currentPosition.x) > 3)
            {
                continue;
            }

            foreach (int offsetY in YOffsets)
            {
                int newY = currentPosition.y + offsetY;

                if (IsValidTile(x, newY) && IsValidTarget(unit, x, newY, validTargets))
                {
                    targets.Add(GridManager.Instance.Grid[x, newY]);
                }
            }
        }

        return targets;
    }

    private static List<Tile> GetReachTargets(UnitCard unit, List<bool> validTargets)
    {
        List<Tile> targets = new List<Tile>();
        Vector2Int currentPosition = unit.CurrentTile.GridPosition;
        List<int> xOrder = unit.IsPlayer1 ? XOrder_Player1 : XOrder_Player2;

        foreach (int x in xOrder)
        {
            if (Mathf.Abs(x - currentPosition.x) > 2)
            {
                continue;
            }

            foreach (int offsetY in YOffsets)
            {
                int newY = currentPosition.y + offsetY;

                if (IsValidTile(x, newY) && IsValidTarget(unit, x, newY, validTargets))
                {
                    targets.Add(GridManager.Instance.Grid[x, newY]);
                }
            }
        }

        return targets;
    }

    private static List<Tile> GetGlobalTargets(UnitCard unit, List<bool> validTargets)
    {
        List<Tile> targets = new List<Tile>();
        Vector2Int currentPosition = unit.CurrentTile.GridPosition;
        List<int> xOrder = unit.IsPlayer1 ? XOrder_Player1 : XOrder_Player2;

        foreach (int x in xOrder)
        {
            foreach (int offsetY in YOffsets)
            {
                int newY = currentPosition.y + offsetY;

                if (IsValidTile(x, newY) && IsValidTarget(unit, x, newY, validTargets))
                {
                    targets.Add(GridManager.Instance.Grid[x, newY]);
                }
            }
        }

        return targets;
    }

    private static bool IsValidTile(int x, int y)
    {
        return x >= 0 && x < 6 && y >= 0 && y < 5;
    }

    private static bool IsValidTarget(UnitCard unit, int x, int y, List<bool> validTargets)
    {
        Tile targetTile = GridManager.Instance.Grid[x, y];

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
