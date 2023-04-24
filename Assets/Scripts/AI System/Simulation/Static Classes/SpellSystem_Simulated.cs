using System.Collections.Generic;
using UnityEngine;

public static class SpellSystem_Simulated
{
    public static void PerformSpell(SpellCardData spell, List<UnitCardData> targetCards, GameState gameState)
    {
        SpellInfo spellInfo = spell.CardInfo as SpellInfo;
        // Spawn VFX

        foreach (UnitCardData targetCard in targetCards)
        {
            if (spellInfo.HasTag(SpellTags.Damage))
            {
                PerformDamage(spell, targetCard, gameState);
            }

            if (spellInfo.HasTag(SpellTags.Heal))
            {
                PerformHeal(spell, targetCard);
            }
        }
    }

    public static void PerformDamage(SpellCardData spell, UnitCardData targetCard, GameState gameState)
    {
        SpellInfo spellInfo = spell.CardInfo as SpellInfo;

        int damageDealt = spell.Power;

        targetCard.TakeDamage(damageDealt, spellInfo.HasTag(SpellTags.DeathTouch), gameState);
    }

    private static void PerformHeal(SpellCardData spell, UnitCardData targetCard)
    {
        targetCard.Heal(spell.Power);
    }

    public static List<TileData> GetTargetTiles(TileData cursorTarget, Vector2Int areaOfEffect, bool isPlayer1, List<bool> validTargets, TileData[,] grid)
    {
        List<TileData> validTiles = new List<TileData>();

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
                TileData currentTile = grid[x, y];

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

        return validTiles;
    }
}
