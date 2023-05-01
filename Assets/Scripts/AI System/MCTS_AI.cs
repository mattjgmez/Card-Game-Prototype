using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a Monte Carlo Tree Search (MCTS) based AI for decision-making in a card game.
/// </summary>
public class MCTS_AI
{
    /// <summary>
    /// Maximum number of iterations for the MCTS algorithm.
    /// </summary>
    public int MaxIterations { get; set; }

    /// <summary>
    /// Exploration constant used in the Upper Confidence Bound 1 (UCB1) formula.
    /// </summary>
    public float ExplorationConstant { get; set; }

    /// <summary>
    /// Initializes a new instance of the MCTS_AI class with the specified parameters.
    /// </summary>
    /// <param name="maxIterations">The maximum number of iterations for the MCTS algorithm.</param>
    /// <param name="explorationConstant">The exploration constant used in the UCB1 formula.</param>
    public MCTS_AI(int maxIterations = 1000, float explorationConstant = 1.4142135f)
    {
        MaxIterations = maxIterations;
        ExplorationConstant = explorationConstant;
    }

    /// <summary>
    /// Gets the best move for the AI to make given the current game state.
    /// </summary>
    /// <param name="gameState">The current game state.</param>
    /// <returns>The best move (IGameAction) for the AI to make.</returns>
    public IGameAction GetBestMove(GameState gameState)
    {
        Debug.Log($"MCTS_AI/GetBestMove: ActiveCards list: {DebugTools.ListToString(gameState.ActiveCards)}");

        MCTS_Node root = new MCTS_Node(gameState, null, null);

        for (int i = 0; i < MaxIterations; i++)
        {
            MCTS_Node selectedNode = Select(root);
            Expand(selectedNode);
            float score = Simulate(selectedNode);
            Backpropagate(selectedNode, score);
        }

        MCTS_Node bestChild = null;
        float bestScore = float.MinValue;
        foreach (MCTS_Node child in root.Children)
        {
            float childScore = child.GetAverageScore();
            if (childScore > bestScore)
            {
                bestScore = childScore;
                bestChild = child;
            }
        }

        return bestChild != null ? bestChild.AssociatedAction : null;
    }

    // Select the most promising node using the UCB1 formula.
    private MCTS_Node Select(MCTS_Node node)
    {
        while (node.Children.Count > 0)
        {
            float bestScore = float.MinValue;
            MCTS_Node bestChild = null;

            foreach (MCTS_Node child in node.Children)
            {
                float ucb1 = child.GetAverageScore() + ExplorationConstant * Mathf.Sqrt(Mathf.Log(node.Visits) / child.Visits);
                if (ucb1 > bestScore || (ucb1 == bestScore && child.Visits < bestChild.Visits))
                {
                    bestScore = ucb1;
                    bestChild = child;
                }
            }

            node = bestChild;
        }

        return node;
    }

    // Expand the selected node by generating its children based on available actions.
    private void Expand(MCTS_Node node)
    {
        List<IGameAction> availableActions = node.State.GetAvailableActions();
        foreach (IGameAction action in availableActions)
        {
            GameState newState = action.Apply(node.State);
            MCTS_Node child = new MCTS_Node(newState, action, node);
            node.AddChild(child);
        }
    }

    // Simulate a game from the selected node to a terminal state using random actions.
    private float Simulate(MCTS_Node node)
    {
        GameState currentState = node.State.Clone();

        int gameLimiter = 0;
        while (!currentState.IsTerminal() && gameLimiter++ <= 100)
        {
            // Simulate each phase of the game

            // Start phase
            currentState.StartPhase();

            // Play phase
            int turnLimiter = 0;
            while (!currentState.IsTerminal() && turnLimiter++ < 1000)
            {
                IGameAction randomAction = currentState.GetRandomAction();
                Debug.Log($"MCTS_AI.Simulate: Random action is {randomAction}.");
                if (randomAction is EndTurnAction)
                {
                    break;
                }
                currentState = randomAction.Apply(currentState);
            }

            // Action phase
            currentState.ActionPhase();

            // Advance phase
            currentState.AdvancePhase();

            // Draw phase
            currentState.DrawPhase();

            // End phase
            currentState.EndPhase();

            // Switch turn
            currentState.SwitchTurn();
        }

        return currentState.GetScore();
    }

    // Backpropagate the simulation results up the tree to update the node scores.
    private void Backpropagate(MCTS_Node node, float score)
    {
        while (node != null)
        {
            node.Update(score);
            node = node.Parent;
        }
    }
}
