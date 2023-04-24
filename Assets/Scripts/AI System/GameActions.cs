using System;
using System.Collections;
using UnityEngine;

public interface IGameAction
{
    GameState Apply(GameState state);
}

public class PlayUnitCardAction : IGameAction
{
    public UnitCardData UnitCardToPlay;
    public Vector2Int Position;

    public PlayUnitCardAction(UnitCardData unitCardToPlay, Vector2Int position)
    {
        UnitCardToPlay = unitCardToPlay;
        Position = position;
    }

    public GameState Apply(GameState state)
    {
        // Apply the action to the game state:
        // 1. Remove the UnitCard from the player's hand
        // 2. Add the UnitCard to the list of active units
        // 3. Set the position of the unit on the board

        // Make a deep copy of the game state to avoid modifying the original state
        GameState newState = state.Clone();

        // Apply the action to the new state
        newState.RemoveCardFromHand(UnitCardToPlay);
        newState.AddActiveUnit(UnitCardToPlay, Position);

        return newState;
    }

    public override string ToString()
    {
        return $"PlayUnitCardAction: {UnitCardToPlay.Name} to {Position}";
    }
}

public class PlaySpellCardAction : IGameAction
{
    public SpellCardData SpellCardToPlay;
    public Vector2Int Position;

    public PlaySpellCardAction(SpellCardData spellCardToPlay, Vector2Int areaOfEffectCenter)
    {
        SpellCardToPlay = spellCardToPlay;
        Position = areaOfEffectCenter;
    }

    public GameState Apply(GameState state)
    {
        // Apply the action to the game state:
        // 1. Remove the SpellCard from the player's hand
        // 2. Apply the spell effect (e.g., deal damage, heal) to the affected units within the area of effect

        // Make a deep copy of the game state to avoid modifying the original state
        GameState newState = state.Clone();

        // Apply the action to the new state
        newState.RemoveCardFromHand(SpellCardToPlay);
        newState.ApplySpellEffect(SpellCardToPlay, Position);

        return newState;
    }

    public override string ToString()
    {
        return $"PlaySpellCardAction: {SpellCardToPlay.Name} to {Position}";
    }
}

public class EndTurnAction : IGameAction
{
    public GameState Apply(GameState state)
    {
        GameState newState = state.Clone();

        // Perform any necessary end-of-turn checks or updates here
        newState.PerformAdvanceChecks();

        return newState;
    }
}

