using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCTS_Node
{
    public GameState State { get; private set; }
    public IGameAction AssociatedAction { get; private set; }
    public MCTS_Node Parent { get; private set; }
    public List<MCTS_Node> Children { get; private set; }

    public int Visits { get; private set; }
    public float TotalScore { get; private set; }

    public MCTS_Node(GameState state, IGameAction associatedAction, MCTS_Node parent)
    {
        State = state;
        AssociatedAction = associatedAction;
        Parent = parent;
        Children = new List<MCTS_Node>();

        Visits = 0;
        TotalScore = 0;
    }

    public void AddChild(MCTS_Node child)
    {
        Children.Add(child);
    }

    public void Update(float score)
    {
        Visits++;
        TotalScore += score;
    }

    public float GetAverageScore()
    {
        return Visits == 0 ? 0 : TotalScore / Visits;
    }
}
