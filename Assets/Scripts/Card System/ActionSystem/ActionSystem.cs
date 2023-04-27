using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// The ActionSystem class provides a static interface for managing and executing card actions
/// based on their action keywords and target tiles.
/// </summary>
public static class ActionSystem
{
    #region ACTION METHODS
    /// <summary>
    /// Performs an action on the target tiles based on the action keywords.
    /// </summary>
    /// <param name="card">The card using the action.</param>
    /// <param name="action">The action being performed.</param>
    /// <param name="targetCards">The list of target cards.</param>
    public static void PerformAction(UnitCard card, ActionInfo action, List<UnitCard> targetCards)
    {
        DebugList(card, action, targetCards);

        foreach (UnitCard targetCard in targetCards)
        {
            if (action.HasKeyword(ActionKeyword.Heal))
            {
                PerformHeal(card, targetCard);
            }

            if (action.HasKeyword(ActionKeyword.Damage))
            {
                PerformDamage(card, targetCard, action);
            }

            if (action.HasKeyword(ActionKeyword.Provoke))
            {
                PerformProvoke(card, targetCard);
            }
        }
    }

    public static void DebugList(UnitCard card, ActionInfo action, List<UnitCard> list)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] != null)
            {
                sb.Append($"{list[i].GetName} [{list[i].CurrentTile.name}]");
            }
            else
            {
                sb.Append($"Empty tile {list[i].name}");
            }
            if (i < list.Count - 1)
            {
                sb.Append(", ");
            }
        }

        string result = sb.ToString();
        Debug.Log($"{card.GetName} [{card.CurrentTile.name}] used {action.Name} against {result}.");
    }

    public static void DebugList(List<Tile> list)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] != null)
            {
                sb.Append(list[i].name);
            }
            else
            {
                sb.Append($"Empty tile {list[i].name}");
            }
            if (i < list.Count - 1)
            {
                sb.Append(", ");
            }
        }

        string result = sb.ToString();
        Debug.Log($"Debug List: {result}");
    }

    /// <summary>
    /// Performs a heal action on the target card.
    /// </summary>
    /// <param name="card">The card using the action.</param>
    /// <param name="targetCard">The target card to heal.</param>
    private static void PerformHeal(UnitCard card, UnitCard targetCard)
    {
        targetCard.Heal(card.Power);
    }

    /// <summary>
    /// Performs a damage action on the target card and returns the damage dealt.
    /// </summary>
    /// <param name="card">The card using the action.</param>
    /// <param name="targetCard">The target card to damage.</param>
    /// <param name="action">The action being performed.</param>
    /// <returns>The amount of damage dealt.</returns>
    private static int PerformDamage(UnitCard card, UnitCard targetCard, ActionInfo action)
    {
        int damageDealt = card.Power;
        int targetHealth = targetCard.Health;

        targetCard.TakeDamage(damageDealt, action.HasKeyword(ActionKeyword.DeathTouch));

        bool targetSlain = targetHealth <= damageDealt;

        // Handles Overkill Logic
        if (action.HasKeyword(ActionKeyword.Overkill) && targetSlain)
        {
            int overkillDamage = damageDealt - targetHealth;

            if (overkillDamage > 0)
            {
                int overkillDirection = card.IsPlayer1 ? 1 : -1;
                Tile targetTile = targetCard.CurrentTile;

                // Gets the appropriate tile to deal overkill damage to
                Tile overkillTargetTile = GridManager.Instance.Grid[targetTile.GridPosition.x + overkillDirection, targetTile.GridPosition.y];
                UnitCard overkillTarget = overkillTargetTile.ActiveCard;

                if (overkillTarget != null)
                {
                    overkillTarget.TakeDamage(overkillDamage, action.HasKeyword(ActionKeyword.DeathTouch));
                }
            }
        }

        if (action.HasKeyword(ActionKeyword.Drain))
        {
            card.Heal(damageDealt);
        }

        return damageDealt;
    }

    /// <summary>
    /// Performs a provoke action on the target card.
    /// </summary>
    /// <param name="card">The card using the action.</param>
    /// <param name="targetCard">The target card to provoke.</param>
    private static void PerformProvoke(UnitCard card, UnitCard targetCard)
    {
        targetCard.SetProvoked(true, card);
    }
    #endregion

    /// <summary>
    /// Determines the target tiles for a specific card action based on the action keywords and valid tiles.
    /// </summary>
    /// <param name="card">The card using the action.</param>
    /// <param name="action">The action being performed.</param>
    /// <returns>A list of target tiles for the given action.</returns>
    public static List<UnitCard> TargetCards(UnitCard card, ActionInfo action)
    {
        List<UnitCard> targetCards = new();
        List<Tile> validTiles = ValidTiles(card, action);
        Tile targetTile = null;

        foreach (Tile tile in validTiles)
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
                AddCleaveTiles(card, targetTile, targetCards);
            }

            if (action.HasKeyword(ActionKeyword.Burst) && targetTile != null)
            {
                AddBurstTiles(card, targetTile, targetCards);
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
    public static void AddCleaveTiles(UnitCard card, Tile targetTile, List<UnitCard> targetCards)
    {
        int targetX = targetTile.ActiveCard.CurrentTile.GridPosition.x;
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

            UnitCard targetCard = GridManager.Instance.Grid[targetX, newY].ActiveCard;
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
    public static void AddBurstTiles(UnitCard card, Tile targetTile, List<UnitCard> targetCards)
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

            UnitCard targetCard = GridManager.Instance.Grid[newX, y].ActiveCard;
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
    public static void AddNovaTiles(List<UnitCard> targetCards, List<Tile> validTiles)
    {
        foreach (Tile tile in validTiles)
        {
            UnitCard card = tile.ActiveCard;
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
    public static List<Tile> ValidTiles(UnitCard card, ActionInfo action)
    {
        List<Tile> validTiles = ActionRanges.GetValidTargets(card, action.Range, action.ValidTargets);
        List<Tile> tilesToRemove = new List<Tile>();

        foreach (Tile tile in validTiles)
        {
            if (action.HasKeyword(ActionKeyword.Heal) && tile.HasCard)
            {
                if (tile.ActiveCard.Health >= card.MaxHealth)
                {
                    tilesToRemove.Add(tile);
                }
            }
        }

        foreach (Tile tile in tilesToRemove)
        {
            validTiles.Remove(tile);
        }

        if (card.IsProvoked && validTiles.Contains(card.ProvokingCard.CurrentTile))
        {
            validTiles.Clear();
            validTiles.Add(card.ProvokingCard.CurrentTile);
        }

        return validTiles;
    }
}
