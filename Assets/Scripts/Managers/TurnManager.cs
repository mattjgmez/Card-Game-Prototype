using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { Player1Turn = 0, Player2Turn = 1 }
public enum TurnState { TurnStart = 0, PlayPhase = 1, Advance = 2, DrawCards = 3, TurnEnd = 4 }

public class TurnManager : MonoSingleton<TurnManager>
{
    public delegate void TurnStateDelegate(GameState turn);
    public event TurnStateDelegate StartTurn;
    public event TurnStateDelegate PlayPhase;
    public event TurnStateDelegate Advance;
    public event TurnStateDelegate DrawCards;
    public event TurnStateDelegate EndTurn;

    [SerializeField] int _scale = 0;
    [SerializeField] GameState _currentTurn;
    [SerializeField] TurnState _currentTurnState;

    private void Start()
    {
        Advance += IncrementScale;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            Debug.Log("Can advance is:" + GridManager.Instance.CanAdvance());
    }

    public void NextPhase()
    {
        if (_currentTurnState == TurnState.TurnEnd)
        {
            _currentTurn = _currentTurn == GameState.Player1Turn ? GameState.Player2Turn : GameState.Player1Turn;
            _currentTurnState = TurnState.TurnStart;
        }
        else
        {
            _currentTurnState += 1;
        }

        ExecuteCurrentTurnState();
    }

    private void ExecuteCurrentTurnState()
    {
        switch (_currentTurnState)
        {
            case TurnState.TurnStart:
                Debug.Log("StartTurn event called.");
                StartTurn?.Invoke(_currentTurn);
                break;

            case TurnState.PlayPhase:
                Debug.Log("PlayPhase event called.");
                PlayPhase?.Invoke(_currentTurn);
                break;

            case TurnState.Advance:
                if (GridManager.Instance.CanAdvance())
                {
                    Debug.Log("Advance event called.");
                    Advance?.Invoke(_currentTurn);
                }
                break;

            case TurnState.DrawCards:
                Debug.Log("DrawCards event called.");
                DrawCards?.Invoke(_currentTurn);
                break;

            case TurnState.TurnEnd:
                Debug.Log("EndTurn event called.");
                EndTurn?.Invoke(_currentTurn);
                break;
        }
    }

    /// <summary>
    /// Increments or decrements the scale based on the current game state and updates the visibility of columns accordingly.
    /// Triggers a win condition when the scale reaches +5 or -5.
    /// </summary>
    /// <param name="state">The current game state, either Player1Turn or Player2Turn.</param>
    public void IncrementScale(GameState state)
    {
        bool player1Turn = state == GameState.Player1Turn;

        _scale += player1Turn ? 1 : -1;

        UpdateColumnsVisibility();

        if (_scale == 5)
        {
            // Player 1 wins
        }
        else if (_scale == -5)
        {
            // Player 2 wins
        }
    }

    /// <summary>
    /// Updates the visibility of columns based on the current value of _scale.
    /// </summary>
    private void UpdateColumnsVisibility()
    {
        GridManager.Instance.ColumnSetActive(5, _scale == 2 || _scale == -2);
        GridManager.Instance.ColumnSetActive(4, _scale == 3 || _scale == -4);
        GridManager.Instance.ColumnSetActive(1, _scale == -3);
        GridManager.Instance.ColumnSetActive(0, _scale == -2);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(GameCursor.WorldPosition, .25f);
    }

    public bool ObjectSelected { get; set; }
    public int Scale { get { return _scale; } }
    public GameState CurrentTurn { get { return _currentTurn; } }
    public TurnState CurrentTurnState { get { return _currentTurnState; } }
}
