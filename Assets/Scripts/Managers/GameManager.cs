using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags] public enum Realm { Neutral = 0, Nightlands = 1, Glacius = 2, }
public enum GameState { Player1Turn = 0, Player2Turn = 1 }
public enum TurnState { TurnStart = 0, PlayPhase = 1, Advance = 2, DrawCards = 3, TurnEnd = 4 }

public class GameManager : MonoSingleton<GameManager>
{
    public delegate void GameStateDelegate(GameState state);
    public event GameStateDelegate StartTurn;
    public event GameStateDelegate PlayPhase;
    public event GameStateDelegate Advance;
    public event GameStateDelegate DrawCards;
    public event GameStateDelegate EndTurn;

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
            Debug.Log("Can advance is:" + CanAdvance());
    }

    public void NextPhase()
    {
        if (_currentTurnState == TurnState.TurnEnd)
        {
            _currentTurn = _currentTurn == GameState.Player1Turn ? GameState.Player2Turn : GameState.Player1Turn;
            _currentTurnState = TurnState.TurnStart;
        }
        else
            _currentTurnState += 1;

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
                if (CanAdvance())
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

    public bool CanAdvance()
    {
        bool isPlayer1Turn = _currentTurn == GameState.Player1Turn;
        int activeCards = 0;
        bool emptyFrontline;

        if (isPlayer1Turn)
        {
            for (int x = 0; x < 3; x++)
            {
                activeCards += GridManager.Instance.CheckColumn(x) ? 1 : 0;
            }

            emptyFrontline = !GridManager.Instance.CheckColumn(3);

            Debug.Log($"Player 1 Active cards: {activeCards} Empty Frontline: {emptyFrontline}");
            return activeCards > 0 && emptyFrontline;
        }
        else
        {
            for (int x = 3; x < 6; x++)
            {
                activeCards += GridManager.Instance.CheckColumn(x) ? 1 : 0;
            }

            emptyFrontline = !GridManager.Instance.CheckColumn(2);

            Debug.Log($"Player 2 Active cards: {activeCards} Empty Frontline: {emptyFrontline}");
            return activeCards > 0 && emptyFrontline;
        }
    }

    void IncrementScale(GameState state)
    {
        bool player1Turn = state == GameState.Player1Turn;

        _scale += player1Turn ? 1 : -1;

        switch (_scale)
        {
            case 5:
                //Player 1 wins
                break;
            case 4:
                GridManager.Instance.ColumnSetActive(5, false);
                GridManager.Instance.ColumnSetActive(4, false);
                break;
            case 3:
                GridManager.Instance.ColumnSetActive(5, false);
                GridManager.Instance.ColumnSetActive(4, true);
                break;
            case 2:
                GridManager.Instance.ColumnSetActive(5, true);
                break;
            case -2:
                GridManager.Instance.ColumnSetActive(0, true);
                break;
            case -3:
                GridManager.Instance.ColumnSetActive(1, true);
                GridManager.Instance.ColumnSetActive(0, false);
                break;
            case -4:
                GridManager.Instance.ColumnSetActive(1, false);
                GridManager.Instance.ColumnSetActive(0, false);
                break;
            case -5:
                //Player 2 wins
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(GameCursor.WorldPosition, .25f);
    }

    public int Scale { get { return _scale; } }
    public GameState CurrentTurn { get { return _currentTurn; } }
    public TurnState CurrentTurnState { get { return _currentTurnState; } }
}
