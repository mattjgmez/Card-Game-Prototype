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

        bool targetSlain = false;

        foreach (UnitCard targetCard in targetCards)
        {
            if (action.HasKeyword(ActionKeywords.Heal))
            {
                PerformHeal(card, targetCard);
            }

            if (action.HasKeyword(ActionKeywords.Damage))
            {
                int damageDealt = PerformDamage(card, targetCard, action, ref targetSlain);
            }

            if (action.HasKeyword(ActionKeywords.Provoke))
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
        targetCard.Heal(card.GetPower);
    }

    /// <summary>
    /// Performs a damage action on the target card and returns the damage dealt.
    /// </summary>
    /// <param name="card">The card using the action.</param>
    /// <param name="targetCard">The target card to damage.</param>
    /// <param name="action">The action being performed.</param>
    /// <param name="targetSlain">Whether the target has been slain.</param>
    /// <returns>The amount of damage dealt.</returns>
    private static int PerformDamage(UnitCard card, UnitCard targetCard, ActionInfo action, ref bool targetSlain)
    {
        int damageDealt = card.GetPower;
        int targetHealth = targetCard.GetHealth;

        targetCard.TakeDamage(damageDealt, action.HasKeyword(ActionKeywords.DeathTouch));

        if (action.HasKeyword(ActionKeywords.Overkill) && targetHealth <= damageDealt)
        {
            targetSlain = true;

            if (targetHealth < damageDealt)
            {
                int overkillDamage = damageDealt - targetHealth;

                Tile targetTile = targetCard.CurrentTile;
                Tile overkillTargetTile = GridManager.Instance.Grid[targetTile.GridPosition.x + 1, targetTile.GridPosition.y];
                UnitCard overkillTarget = overkillTargetTile.ActiveCard;

                overkillTarget.TakeDamage(overkillDamage, action.HasKeyword(ActionKeywords.DeathTouch));
            }
        }

        if (action.HasKeyword(ActionKeywords.Drain))
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

        if (action.HasKeyword(ActionKeywords.Nova))
        {
            AddNovaTiles(targetCards, validTiles);
        }
        else
        {
            targetCards.Add(targetTile.ActiveCard);

            if (action.HasKeyword(ActionKeywords.Cleave) && targetTile != null)
            {
                AddCleaveTiles(card, targetTile, targetCards);
            }

            if (action.HasKeyword(ActionKeywords.Burst) && targetTile != null)
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
    private static void AddCleaveTiles(UnitCard card, Tile targetTile, List<UnitCard> targetCards)
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
    private static void AddBurstTiles(UnitCard card, Tile targetTile, List<UnitCard> targetCards)
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
    private static void AddNovaTiles(List<UnitCard> targetCards, List<Tile> validTiles)
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
        List<Tile> validTiles = new List<Tile>();

        switch (action.Range)
        {
            case ActionRange.Melee:
                validTiles = ActionRanges.Melee(card.CurrentTile.GridPosition, card.IsPlayer1, action.ValidTargets);
                break;
            case ActionRange.Ranged:
                validTiles = ActionRanges.Ranged(card.CurrentTile.GridPosition, card.IsPlayer1, action.ValidTargets);
                break;
            case ActionRange.Reach:
                validTiles = ActionRanges.Reach(card.CurrentTile.GridPosition, card.IsPlayer1, action.ValidTargets);
                break;
            case ActionRange.Global:
                validTiles = ActionRanges.Global(card.CurrentTile.GridPosition, card.IsPlayer1, action.ValidTargets);
                break;
        }

        if (card.IsProvoked && validTiles.Contains(card.GetProvokingCard.CurrentTile))
        {
            validTiles.Clear();
            validTiles.Add(card.GetProvokingCard.CurrentTile);
        }

        return validTiles;
    }
}
