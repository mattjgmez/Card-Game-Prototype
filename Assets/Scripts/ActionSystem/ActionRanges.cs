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
    /// Returns a list of valid target tiles for a card action with melee range based on the current position and the targeting rules.
    /// </summary>
    /// <param name="currentPosition">The current position of the card on the grid, as a Vector2Int.</param>
    /// <param name="isPlayer1">A boolean indicating whether the card belongs to Player 1.</param>
    /// <param name="validTargets">A dictionary containing the valid targeting rules for the action, using string keys and bool values.</param>
    /// <returns>A list of valid target tiles for the card action with melee range.</returns>
    public static List<Tile> Melee(Vector2Int currentPosition, bool isPlayer1, List<bool> validTargets)
    {
        List<Tile> validTiles = new List<Tile>();

        // Retrieve the targeting rules from the dictionary.
        bool targetsEnemies = validTargets[0];
        bool targetsAllies = validTargets[1];
        bool targetsSelf = validTargets[2];

        // Define a range for x and y based on the current position.
        int xMin = Mathf.Max(currentPosition.x - 1, 0);
        int xMax = Mathf.Min(currentPosition.x + 1, 5);
        int yMin = Mathf.Max(currentPosition.y - 1, 0);
        int yMax = Mathf.Min(currentPosition.y + 1, 4);

        // Iterate through the tiles within the defined range.
        for (int x = xMin; x <= xMax; x++)
        {
            for (int y = yMin; y <= yMax; y++)
            {
                Tile tile = GridManager.Instance.Grid[x, y];

                // If the tile is the current position and self-targeting is allowed, add the tile to the valid tiles.
                if (tile.GridPosition == currentPosition && targetsSelf)
                {
                    validTiles.Add(tile);
                }
                // If the tile belongs to an enemy and enemy-targeting is allowed, add the tile to the valid tiles.
                else if (tile.GetIsPlayer1 != isPlayer1 && targetsEnemies)
                {
                    validTiles.Add(tile);
                }
                // If the tile belongs to an ally, is not the current position, and ally-targeting is allowed, add the tile to the valid tiles.
                else if (tile.GetIsPlayer1 == isPlayer1 && targetsAllies && tile.GridPosition != currentPosition)
                {
                    validTiles.Add(tile);
                }
            }
        }

        return validTiles;
    }

    /// <summary>
    /// Returns a list of valid target tiles for a card action with reach based on the current position and the targeting rules.
    /// </summary>
    /// <param name="currentPosition">The current position of the card on the grid, as a Vector2Int.</param>
    /// <param name="isPlayer1">A boolean indicating whether the card belongs to Player 1.</param>
    /// <param name="validTargets">A dictionary containing the valid targeting rules for the action, using string keys and bool values.</param>
    /// <returns>A list of valid target tiles for the card action with reach.</returns>
    public static List<Tile> Reach(Vector2Int currentPosition, bool isPlayer1, List<bool> validTargets)
    {
        List<Tile> validTiles = new List<Tile>();

        // Retrieve the targeting rules from the dictionary.
        bool targetsEnemies = validTargets[0];
        bool targetsAllies = validTargets[1];
        bool targetsSelf = validTargets[2];

        // Define a range for x and y based on the current position.
        int xMin = Mathf.Max(currentPosition.x - 1, 0);
        int xMax = Mathf.Min(currentPosition.x + 1, 5);
        int yMin = Mathf.Max(currentPosition.y - 1, 0);
        int yMax = Mathf.Min(currentPosition.y + 1, 4);

        // Iterate through the tiles within the defined range.
        for (int x = xMin; x <= xMax; x++)
        {
            for (int y = yMin; y <= yMax; y++)
            {
                Tile tile = GridManager.Instance.Grid[x, y];

                // If the tile is the current position and self-targeting is allowed, add the tile to the valid tiles.
                if (tile.GridPosition == currentPosition && targetsSelf)
                {
                    validTiles.Add(tile);
                }
                // If the tile belongs to an enemy and enemy-targeting is allowed, add the tile to the valid tiles.
                else if (tile.GetIsPlayer1 != isPlayer1 && targetsEnemies)
                {
                    validTiles.Add(tile);
                }
                // If the tile belongs to an ally, is not the current position, and ally-targeting is allowed, add the tile to the valid tiles.
                else if (tile.GetIsPlayer1 == isPlayer1 && targetsAllies && tile.GridPosition != currentPosition)
                {
                    validTiles.Add(tile);
                }
            }
        }

        return validTiles;
    }

    /// <summary>
    /// Returns a list of valid target tiles for a ranged card action based on the current position and the targeting rules.
    /// </summary>
    /// <param name="currentPosition">The current position of the card on the grid, as a Vector2Int.</param>
    /// <param name="isPlayer1">A boolean indicating whether the card belongs to Player 1.</param>
    /// <param name="validTargets">A dictionary containing the valid targeting rules for the action, using string keys and bool values.</param>
    /// <returns>A list of valid target tiles for the ranged card action.</returns>
    public static List<Tile> Ranged(Vector2Int currentPosition, bool isPlayer1, List<bool> validTargets)
    {
        // If the card is on its frontline, treat it as a Reach action instead of Ranged.
        if (currentPosition.x == (isPlayer1 ? 2 : 3))
            return Reach(currentPosition, isPlayer1, validTargets);

        List<Tile> validTiles = new List<Tile>();

        // Retrieve the targeting rules from the dictionary.
        bool targetsEnemies = validTargets[0];
        bool targetsAllies = validTargets[1];
        bool targetsSelf = validTargets[2];

        // Set up loop variables based on the player.
        int enemyFrontline = isPlayer1 ? 3 : 2;
        int startX = isPlayer1 ? 0 : 5;
        int endX = isPlayer1 ? enemyFrontline + 1 : enemyFrontline - 1;
        int stepX = isPlayer1 ? 1 : -1;

        // Iterate through the grid based on the player's side, considering only the side up to and including the enemy frontline.
        for (int x = startX; isPlayer1 ? x < endX : x > endX; x += stepX)
        {
            for (int y = 0; y < 5; y++)
            {
                Tile tile = GridManager.Instance.Grid[x, y];

                // If the tile is the current position and self-targeting is allowed, add the tile to the valid tiles.
                if (tile.GridPosition == currentPosition && targetsSelf)
                {
                    validTiles.Add(tile);
                }
                // If the tile belongs to an enemy and enemy-targeting is allowed,
                // add the tile and its neighbor (if valid) to the valid tiles.
                else if (tile.GetIsPlayer1 != isPlayer1 && targetsEnemies)
                {
                    int neighborX = isPlayer1 ? x + 1 : x - 1;
                    if (neighborX >= 0 && neighborX <= 5 && !tile.ActiveCard)
                    {
                        validTiles.Add(GridManager.Instance.Grid[neighborX, y]);
                    }

                    validTiles.Add(tile);
                }
                // If the tile belongs to an ally, is not the current position, and ally-targeting is allowed,
                // add the tile to the valid tiles.
                else if (tile.GetIsPlayer1 == isPlayer1 && targetsAllies && tile.GridPosition != currentPosition)
                {
                    validTiles.Add(tile);
                }
            }
        }

        return validTiles;
    }

    /// <summary>
    /// Returns a list of valid target tiles for a global card action based on the current position and the targeting rules.
    /// </summary>
    /// <param name="currentPosition">The current position of the card on the grid, as a Vector2Int.</param>
    /// <param name="isPlayer1">A boolean indicating whether the card belongs to Player 1.</param>
    /// <param name="validTargets">A dictionary containing the valid targeting rules for the action, using string keys and bool values.</param>
    /// <returns>A list of valid target tiles for the global card action.</returns>
    public static List<Tile> Global(Vector2Int currentPosition, bool isPlayer1, List<bool> validTargets)
    {
        List<Tile> validTiles = new List<Tile>();

        // Retrieve the targeting rules from the dictionary.
        bool targetsEnemies = validTargets[0];
        bool targetsAllies = validTargets[1];
        bool targetsSelf = validTargets[2];

        // Iterate through the entire grid.
        for (int x = 0; x < 6; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                Tile tile = GridManager.Instance.Grid[x, y];

                // If the tile is the current position and self-targeting is allowed, add the tile to the valid tiles.
                if (tile.GridPosition == currentPosition && targetsSelf)
                {
                    validTiles.Add(tile);
                }
                // If the tile belongs to an enemy and enemy-targeting is allowed, add the tile to the valid tiles.
                else if (tile.GetIsPlayer1 != isPlayer1 && targetsEnemies)
                {
                    validTiles.Add(tile);
                }
                // If the tile belongs to an ally, is not the current position, and ally-targeting is allowed, add the tile to the valid tiles.
                else if (tile.GetIsPlayer1 == isPlayer1 && targetsAllies && tile.GridPosition != currentPosition)
                {
                    validTiles.Add(tile);
                }
            }
        }

        return validTiles;
    }
}
