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
    public static List<Tile> Melee(Vector2Int currentPosition, bool isPlayer1, StringBoolDictionary validTargets)
    {
        List<Tile> validTiles = new List<Tile>();
        validTargets.TryGetValue("Targets Enemies", out bool targetsEnemies);
        validTargets.TryGetValue("Targets Allies", out bool targetsAllies);
        validTargets.TryGetValue("Targets Self", out bool targetsSelf);

        int x = currentPosition.x - 1; //Only the y needs to be declared inside the loop
        for (; x < currentPosition.x + 2; x++)/* and */ for (int y = currentPosition.y - 1; y < currentPosition.y + 2; y++)
            {
                if (x < 0 || y < 0 || x > 5 || y > 4)
                    continue;

                Debug.Log($"Checking tile; {x}:{y}");
                Tile tile = GridManager.Instance.Grid[x, y];

                if (tile.GridPosition == currentPosition && targetsSelf)
                    validTiles.Add(tile);
                else if (tile.GetIsPlayer1 != isPlayer1 && targetsEnemies)
                    validTiles.Add(tile);
                else if (tile.GetIsPlayer1 == isPlayer1 && targetsAllies && tile.GridPosition != currentPosition)
                    validTiles.Add(tile);
            }
        return validTiles;
    }

    public static List<Tile> Reach(Vector2Int currentPosition, bool isPlayer1, StringBoolDictionary validTargets)
    {
        List<Tile> validTiles = new List<Tile>();
        validTargets.TryGetValue("Targets Enemies", out bool targetsEnemies);
        validTargets.TryGetValue("Targets Allies", out bool targetsAllies);
        validTargets.TryGetValue("Targets Self", out bool targetsSelf);

        int x = currentPosition.x - 1; //Only the y needs to be declared inside the loop
        if(isPlayer1)
        for (; x < currentPosition.x + 2; x++)/* and */ for (int y = currentPosition.y - 1; y < currentPosition.y + 2; y++)
            {
                if (x < 0 || y < 0 || x > 5 || y > 4)
                    continue;

                Tile tile = GridManager.Instance.Grid[x, y];

                if (tile.GridPosition == currentPosition && targetsSelf)
                    validTiles.Add(tile);
                else if (tile.GetIsPlayer1 != isPlayer1 && targetsEnemies)
                {
                    if ((!((x - 1) < 0) || !((x + 1) > 0)) && !tile.ActiveCard)
                        validTiles.Add(GridManager.Instance.Grid[x + 1, y]);

                    validTiles.Add(tile);
                }
                else if (tile.GetIsPlayer1 == isPlayer1 && targetsAllies && tile.GridPosition != currentPosition)
                {
                    if ((!((x - 1) < 0) || !((x + 1) > 0)) && !tile.ActiveCard)
                        validTiles.Add(GridManager.Instance.Grid[x - 1, y]);

                    validTiles.Add(tile);
                }
            }
        if (!isPlayer1)
        for (; x < currentPosition.x + 2; x++)/* and */ for (int y = currentPosition.y - 1; y < currentPosition.y + 2; y++)
            {
                if (x < 0 || y < 0 || x > 5 || y > 4)
                    continue;

                Tile tile = GridManager.Instance.Grid[x, y];

                if (tile.GridPosition == currentPosition && targetsSelf)
                    validTiles.Add(tile);
                else if (tile.GetIsPlayer1 != isPlayer1 && targetsEnemies)
                {
                    if ((!((x - 1) < 0) || !((x + 1) > 0)) && !tile.ActiveCard)
                        validTiles.Add(GridManager.Instance.Grid[x - 1, y]);

                    validTiles.Add(tile);
                }
                else if (tile.GetIsPlayer1 == isPlayer1 && targetsAllies && tile.GridPosition != currentPosition)
                {
                    if ((!((x - 1) < 0) || !((x + 1) > 0)) && !tile.ActiveCard)
                        validTiles.Add(GridManager.Instance.Grid[x + 1, y]);

                    validTiles.Add(tile);
                }
            }

        return validTiles;
    }

    public static List<Tile> Ranged(Vector2Int currentPosition, bool isPlayer1, StringBoolDictionary validTargets)
    {
        if (currentPosition.x == (isPlayer1 ? 2 : 3))
            return Reach(currentPosition, isPlayer1, validTargets);

        List<Tile> validTiles = new List<Tile>();
        validTargets.TryGetValue("Targets Enemies", out bool targetsEnemies);
        validTargets.TryGetValue("Targets Allies", out bool targetsAllies);
        validTargets.TryGetValue("Targets Self", out bool targetsSelf);

        int enemyFrontline = isPlayer1 ? 3 : 2;
        if (isPlayer1)
            for (int x = 0; x < enemyFrontline + 1; x++)
                for (int y = 0; y < 5; y++)
                {
                    if (x < 0 || y < 0 || x > 5 || y > 4)
                        continue;

                    Tile tile = GridManager.Instance.Grid[x, y];

                    if (tile.GridPosition == currentPosition && targetsSelf)
                        validTiles.Add(tile);
                    else if (tile.GetIsPlayer1 != isPlayer1 && targetsEnemies)
                    {
                        if ((!((x - 1) < 0) || !((x + 1) > 0)) && !tile.ActiveCard)
                            validTiles.Add(GridManager.Instance.Grid[x + 1, y]);

                        validTiles.Add(tile);
                    }
                    else if (tile.GetIsPlayer1 == isPlayer1 && targetsAllies && tile.GridPosition != currentPosition)
                        validTiles.Add(tile);
                }
        else
            for (int x = 5; x > enemyFrontline - 1; x--)
                for (int y = 0; y < 5; y++)
                {
                    if (x < 0 || y < 0 || x > 5 || y > 4)
                        continue;

                    Tile tile = GridManager.Instance.Grid[x, y];

                    if (tile.GridPosition == currentPosition && targetsSelf)
                        validTiles.Add(tile);
                    else if (tile.GetIsPlayer1 != isPlayer1 && targetsEnemies)
                    {
                        if ((!((x - 1) < 0) || !((x + 1) > 0)) && !tile.ActiveCard)
                            validTiles.Add(GridManager.Instance.Grid[x - 1, y]);

                        validTiles.Add(tile);
                    }
                    else if (tile.GetIsPlayer1 == isPlayer1 && targetsAllies && tile.GridPosition != currentPosition)
                        validTiles.Add(tile);
                }

        return validTiles;
    }

    public static List<Tile> Global(Vector2Int currentPosition, bool isPlayer1, StringBoolDictionary validTargets)
    {
        List<Tile> validTiles = new List<Tile>();
        validTargets.TryGetValue("Targets Enemies", out bool targetsEnemies);
        validTargets.TryGetValue("Targets Allies", out bool targetsAllies);
        validTargets.TryGetValue("Targets Self", out bool targetsSelf);

        for (int x = 0; x < 6; x++)/* and */ for (int y = 0; y < 5; y++)
        {
                if (x < 0 || y < 0 || x > 5 || y > 4)
                    continue;

                Tile tile = GridManager.Instance.Grid[x, y];

            if (tile.GridPosition == currentPosition && targetsSelf)
                validTiles.Add(tile);
            else if (tile.GetIsPlayer1 != isPlayer1 && targetsEnemies)
                validTiles.Add(tile);
            else if (tile.GetIsPlayer1 == isPlayer1 && targetsAllies && tile.GridPosition != currentPosition)
                validTiles.Add(tile);
        }

        return validTiles;
    }
}
