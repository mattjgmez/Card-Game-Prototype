using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class AIController : MonoBehaviour
{
    [SerializeField] int _maxIterations = 1000;

    private MCTS_AI _ai;

    private void Awake()
    {
        _ai = new MCTS_AI(_maxIterations);

        //GameManager.Instance.CurrentDeck_Player2 = SelectDeckFromFolder();
    }

    private void OnEnable()
    {
        TurnManager.Instance.PlayPhase += StartAITurn;
    }

    private void StartAITurn(PlayerTurn turn)
    {
        if (turn == PlayerTurn.Player2)
        {
            StartCoroutine(AITurn());
        }
    }

    public IEnumerator AITurn()
    {
        // Wait for a short delay before beginning the AI's turn, so that other scripts can initialize
        yield return new WaitForSeconds(2f);

        GameState currentState = new GameState();

        StringBuilder sb = new StringBuilder();

        sb.AppendLine($"AIController.StartAITurn: GameState{currentState.ID} constructed");
        sb.AppendLine($"Player1Hand.Count: {currentState.Player1Hand.Count}, Player2Hand.Count: {currentState.Player2Hand.Count}");
        sb.AppendLine($"ActiveCards.Count: {currentState.ActiveCards.Count}");

        Debug.Log(sb.ToString());

        while (true)
        {
            Debug.Log($"AIController.AITurn {currentState.ID}: Active Cards ({currentState.ActiveCards.Count}): {DebugTools.ListToString(currentState.ActiveCards)}");

            // Get the best move for the AI
            IGameAction bestAction = _ai.GetBestMove(currentState);
            Debug.Log($"AIController.AITurn: The best action is {bestAction}.");

            // Execute the best move
            if (bestAction is PlayUnitCardAction || bestAction is PlaySpellCardAction)
            {
                PlayCard(bestAction);
            }
            else if (bestAction is EndTurnAction)
            {
                break;
            }

            // If there are no valid actions left, end the turn
            if (currentState.GetAvailableActions().Count == 0 || bestAction == null)
            {
                break;
            }

            // Wait for a short delay before executing the next action or ending the AI's turn (e.g., 0.5 seconds)
            yield return new WaitForSeconds(0.5f);
        }

        EndTurn();
    }

    private void EndTurn()
    {
        TurnManager.Instance.NextPhase();
    }

    private void PlayCard(IGameAction action)
    {
        // Check if the action is a PlaySpellCardAction
        if (action is PlaySpellCardAction spellAction)
        {
            PlaySpellCard(spellAction);
        }
        // Check if the action is a PlayUnitCardAction
        else if (action is PlayUnitCardAction unitAction)
        {
            PlayUnitCard(unitAction);
        }
        else
        {
            // Log an error if the action type is unsupported
            Debug.LogError("AIController.PlayCard: Unsupported IGameAction type.");
        }
    }

    private static void PlayUnitCard(PlayUnitCardAction unitAction)
    {
        // Initialize the cardToPlay variable
        UnitCard cardToPlay = null;
        // Get the UnitCardData from the action
        UnitCardData unitCardData = unitAction.UnitCardToPlay;

        // Retrieve the list of cards in the player's hand
        List<Card> cardsInHand = HandManager.Instance.GetHand(2);

        // Iterate through the cards in hand to find the matching unit card
        foreach (Card card in cardsInHand)
        {
            if (card is UnitCard unitCard)
            {
                UnitCardData currentCardData = UnitCardData.FromUnitCard(unitCard);

                // Check if the current card data matches the action's card data
                if (currentCardData.Equals(unitCardData))
                {
                    // Set cardToPlay to the matching card and exit the loop
                    cardToPlay = unitCard;
                    break;
                }
            }
        }

        // Check if a matching card was found
        if (cardToPlay != null)
        {
            // Get the target position for the card
            int x = unitAction.Position.x;
            int y = unitAction.Position.y;

            // Play the card to the target tile
            cardToPlay.PlayToTile(GridManager.Instance.Grid[x, y]);
        }
        else
        {
            // Log an error if no matching card was found
            Debug.LogError("AIController.PlayCard: Unit card matching the given UnitCardData was not found in the hand.");
        }
    }

    private static void PlaySpellCard(PlaySpellCardAction spellAction)
    {
        // Initialize the cardToPlay variable
        SpellCard cardToPlay = null;
        // Get the SpellCardData from the action
        SpellCardData spellCardData = spellAction.SpellCardToPlay;

        // Retrieve the list of cards in the player's hand
        List<Card> cardsInHand = HandManager.Instance.GetHand(2);

        // Iterate through the cards in hand to find the matching spell card
        foreach (Card card in cardsInHand)
        {
            if (card is SpellCard spellCard)
            {
                SpellCardData currentCardData = SpellCardData.FromSpellCard(spellCard);

                // Check if the current card data matches the action's card data
                if (currentCardData.Equals(spellCardData))
                {
                    // Set cardToPlay to the matching card and exit the loop
                    cardToPlay = spellCard;
                    break;
                }
            }
        }

        // Check if a matching card was found
        if (cardToPlay != null)
        {
            // Get the target position for the card effect
            int x = spellAction.Position.x;
            int y = spellAction.Position.y;

            // Play the card to the target tile
            cardToPlay.PlayToTile(GridManager.Instance.Grid[x, y]);
        }
        else
        {
            // Log an error if no matching card was found
            Debug.LogError("AIController.PlayCard: Spell card matching the given SpellCardData was not found in the hand.");
        }
    }
}
