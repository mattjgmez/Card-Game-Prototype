using System.Collections.Generic;
using UnityEngine;

public static class SpellSystem
{
    public static void PerformSpell(SpellCard spell, List<UnitCard> targetCards)
    {
        SpellInfo spellInfo = spell.SpellInfo;
        // Spawn VFX

        foreach (UnitCard targetCard in targetCards)
        {
            if (spellInfo.HasTag(SpellTags.Damage))
            {
                PerformDamage(spell, targetCard);
            }

            if (spellInfo.HasTag(SpellTags.Heal))
            {
                PerformHeal(spell, targetCard);
            }
        }
    }

    public static void PerformDamage(SpellCard spell, UnitCard targetCard)
    {
        SpellInfo spellInfo = spell.SpellInfo;

        int damageDealt = spell.Power;

        targetCard.TakeDamage(damageDealt, spellInfo.HasTag(SpellTags.DeathTouch));
    }

    private static void PerformHeal(SpellCard spell, UnitCard targetCard)
    {
        targetCard.Heal(spell.Power);
    }

    public static List<Tile> GetTargetTiles(Tile cursorTarget, Vector2Int areaOfEffect, bool isPlayer1, List<bool> validTargets)
    {
        List<Tile> validTiles = new List<Tile>();

        bool targetsEnemies = validTargets[0];
        bool targetsAllies = validTargets[1];

        int offsetX = (areaOfEffect.x - 1) / 2;
        int offsetY = (areaOfEffect.y - 1) / 2;
        int extraX = areaOfEffect.x % 2 == 0 ? 1 : 0;
        int extraY = areaOfEffect.y % 2 == 0 ? 1 : 0;

        int minX = Mathf.Max(0, cursorTarget.GridPosition.x - offsetX);
        int maxX = Mathf.Min(GridManager.GridWidth - 1, cursorTarget.GridPosition.x + offsetX + extraX);
        int minY = Mathf.Max(0, cursorTarget.GridPosition.y - offsetY);
        int maxY = Mathf.Min(GridManager.GridHeight - 1, cursorTarget.GridPosition.y + offsetY + extraY);

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                Tile currentTile = GridManager.Instance.Grid[x, y];

                bool isPlayer1Tile = x <= 2;
                bool isPlayer2Tile = x >= 3;

                if (isPlayer1)
                {
                    if ((targetsEnemies && isPlayer2Tile) || (targetsAllies && isPlayer1Tile))
                    {
                        validTiles.Add(currentTile);
                    }
                }
                else
                {
                    if ((targetsEnemies && isPlayer1Tile) || (targetsAllies && isPlayer2Tile))
                    {
                        validTiles.Add(currentTile);
                    }
                }
            }
        }

        for (int x = 0; x < GridManager.GridWidth; x++)
        {
            for (int y = 0; y < GridManager.GridHeight; y++)
            {
                Tile currentTile = GridManager.Instance.Grid[x, y];
                currentTile.SetTileActive(validTiles.Contains(currentTile));
            }
        }

        return validTiles;
    }
}
