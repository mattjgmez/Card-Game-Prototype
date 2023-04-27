using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// The ActionSystem class provides a static interface for managing and executing card actions
/// based on their action keywords and target tiles.
/// </summary>
public static class ActionSystem_Simulated
{
    public static void PerformAction(UnitCardData card, ActionInfo action, List<UnitCardData> targetCards, GameState gameState)
    {
        Debug.Log($"PerformAction: Card: {card}, CurrentTile: {card.CurrentTile}");

        bool targetSlain = false;

        foreach (UnitCardData targetCard in targetCards)
        {
            if (action.HasKeyword(ActionKeyword.Heal))
            {
                PerformHeal(card, targetCard);
            }

            if (action.HasKeyword(ActionKeyword.Damage))
            {
                int damageDealt = PerformDamage(card, targetCard, action, ref targetSlain, gameState);
            }

            if (action.HasKeyword(ActionKeyword.Provoke))
            {
                PerformProvoke(card, targetCard);
            }
        }
    }

    private static void PerformHeal(UnitCardData card, UnitCardData targetCard)
    {
        targetCard.Heal(card.Power);
    }

    private static int PerformDamage(UnitCardData card, UnitCardData targetCard, ActionInfo action, ref bool targetSlain, GameState gameState)
    {
        int damageDealt = card.Power;
        int targetHealth = targetCard.Health;

        targetCard.TakeDamage(damageDealt, action.HasKeyword(ActionKeyword.DeathTouch), gameState);

        if (action.HasKeyword(ActionKeyword.Overkill) && targetHealth <= damageDealt)
        {
            targetSlain = true;

            if (targetHealth < damageDealt)
            {
                int overkillDamage = damageDealt - targetHealth;
                TileData targetTile = targetCard.CurrentTile;
                int direction = card.IsPlayer1 ? 1 : -1;
                int newPosition = targetTile.GridPosition.x + direction;

                if (newPosition < gameState.Grid.GetLength(0) && newPosition >= 0)
                {
                    TileData overkillTargetTile = gameState.Grid[newPosition, targetTile.GridPosition.y];
                    UnitCardData overkillTarget = overkillTargetTile.ActiveCard;

                    if (overkillTarget != null)
                    {
                        overkillTarget.TakeDamage(overkillDamage, action.HasKeyword(ActionKeyword.DeathTouch), gameState);
                    }
                }
            }
        }

        if (action.HasKeyword(ActionKeyword.Drain))
        {
            card.Heal(damageDealt);
        }

        return damageDealt;
    }

    private static void PerformProvoke(UnitCardData provokingCard, UnitCardData targetCard)
    {
        targetCard.IsProvoked = true;
        targetCard.ProvokingCard = provokingCard;
    }

    /// <summary>
    /// Determines the target tiles for a specific card action based on the action keywords and valid tiles.
    /// </summary>
    /// <param name="card">The card using the action.</param>
    /// <param name="action">The action being performed.</param>
    /// <returns>A list of target tiles for the given action.</returns>
    public static List<UnitCardData> TargetCards(UnitCardData card, ActionInfo action, GameState gameState)
    {
        if (card == null)
        {
            Debug.LogError($"TargetCards: Card is null.");
        }
        if (card.CurrentTile == null)
        {
            Debug.LogError($"TargetCards: CurrentTile is null for card {card}."); // This is being called
        }


        List<UnitCardData> targetCards = new();
        List<TileData> validTiles = ValidTiles(card, action, gameState);
        TileData targetTile = null;

        foreach (TileData tile in validTiles)
        {
            if (tile != null && tile.ActiveCard != null)
            {
                targetTile = tile;
                break;
            }
        }

        if (targetTile == null)
        {
            return targetCards;
        }

        if (action.HasKeyword(ActionKeyword.Nova))
        {
            AddNovaTiles(targetCards, validTiles);
        }
        else
        {
            targetCards.Add(targetTile.ActiveCard);

            if (action.HasKeyword(ActionKeyword.Cleave) && targetTile != null)
            {
                AddCleaveTiles(card, targetTile, targetCards, gameState);
            }

            if (action.HasKeyword(ActionKeyword.Burst) && targetTile != null)
            {
                AddBurstTiles(card, targetTile, targetCards, gameState);
            }
        }

        return targetCards;
    }

    /// <summary>
    /// Adds adjacent tile's active cards along the y axis to the given list of cards based on the given card's position.
    /// </summary>
    /// <param name="card">The card using the action.</param>
    /// <param name="targetTile">The hit tile from the raycast.</param>
    /// <param name="targetCards">The list of cards to add on to.</param>
    private static void AddCleaveTiles(UnitCardData card, TileData targetTile, List<UnitCardData> targetCards, GameState gameState)
    {
        int targetX = targetTile.GridPosition.x;
        int cardY = card.CurrentTile.GridPosition.y;
        int maxRows = 5;
        int[] rowOffsets = { -1, 1 };

        foreach (int yOffset in rowOffsets)
        {
            int newY = cardY + yOffset;

            if (newY < 0 || newY > 4)
            {
                continue;
            }

            UnitCardData targetCard = gameState.Grid[targetX, newY].ActiveCard;
            if (newY >= 0 && newY < maxRows && targetCard != null)
            {
                targetCards.Add(targetCard);
            }
        }
    }

    /// <summary>
    /// Adds the tiles behind the targeted tile's active cards to the given list of cards.
    /// </summary>
    /// <param name="card">The card using the action.</param>
    /// <param name="targetTile">The hit tile from the raycast.</param>
    /// <param name="targetCards">The list of cards to add on to.</param>
    private static void AddBurstTiles(UnitCardData card, TileData targetTile, List<UnitCardData> targetCards, GameState gameState)
    {
        int y = targetTile.GridPosition.y;
        int direction = card.IsPlayer1 ? 1 : -1;
        int maxColumns = 6;

        for (int i = 1; i <= maxColumns; i++)
        {
            int newX = targetTile.GridPosition.x + (direction * i);

            // Check if the newX value is within the grid's bounds
            if (newX < 0 || newX >= maxColumns)
            {
                break;
            }

            UnitCardData targetCard = gameState.Grid[newX, y].ActiveCard;
            if (targetCard != null)
            {
                targetCards.Add(targetCard);
            }
        }
    }

    /// <summary>
    /// Adds all valid tile's active cards to given list of cards.
    /// </summary>
    /// <param name="targetCards">The list of cards to add on to.</param>
    /// <param name="validTiles">The list of valid tiles to add.</param>
    private static void AddNovaTiles(List<UnitCardData> targetCards, List<TileData> validTiles)
    {
        foreach (TileData tile in validTiles)
        {
            UnitCardData card = tile.ActiveCard;
            if (card != null)
            {
                targetCards.Add(card);
            }
        }
    }

    /// <summary>
    /// Determines the valid tiles for a specific card action based on the action range, card position, and action targets.
    /// Takes into account whether the card is provoked.
    /// </summary>
    /// <param name="card">The card using the action.</param>
    /// <param name="action">The action being performed.</param>
    /// <returns>A list of valid tiles for the given action.</returns>
    public static List<TileData> ValidTiles(UnitCardData card, ActionInfo action, GameState gameState)
    {
        List<TileData> validTiles = new List<TileData>();

        if (gameState == null)
        {
            Debug.LogError($"ValidTiles: GameState is null.");
        }
        if (card == null)
        {
            Debug.LogError($"ValidTiles: Card is null.");
        }
        if (card.CurrentTile == null)
        {
            Debug.LogError($"ValidTiles: Tile is null."); // This error gets logged
        }
        if (card.CurrentTile.GridPosition == null) // This causes the null reference
        {
            Debug.LogError($"ValidTiles: GridPosition is null.");
        }

        ActionRanges_Simulated.GetValidTargets(gameState, card, action.Range, action.ValidTargets);

        TileData provokingCardTile = gameState.GetTileDataByCard(card.ProvokingCard);
        if (card.IsProvoked && validTiles.Contains(provokingCardTile))
        {
            validTiles.Clear();
            validTiles.Add(provokingCardTile);
        }

        return validTiles;
    }
}
