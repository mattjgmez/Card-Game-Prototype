using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GridSystem;

public enum PlayerTurn { Player1 = 1, Player2 = 2 }
public enum TurnPhase { Start = 0, Play = 1, Action = 2, Advance = 3, Draw = 4, End = 5 }

public class TurnManager : MonoSingleton<TurnManager>
{
    public delegate void TurnStateDelegate(PlayerTurn turn);
    public event TurnStateDelegate StartPhase;
    public event TurnStateDelegate PlayPhase;
    public event TurnStateDelegate ActionPhase;
    public event TurnStateDelegate AdvancePhase;
    public event TurnStateDelegate DrawPhase;
    public event TurnStateDelegate EndPhase;

    [SerializeField] private int _scale = 0;
    [SerializeField] private PlayerTurn _currentTurn;
    [SerializeField] private TurnPhase _currentTurnState;

    [SerializeField] private int _currentNumberOfTurns = 1;
    [SerializeField] private int _currentNumberOfRounds = 0;
    private bool _isFirstRound = true;
    private List<UnitCard> _activeCards = new List<UnitCard>();

    private void Start()
    {
        StartCoroutine(InitializeGame());
    }

    private IEnumerator InitializeGame()
    {
        // Randomly set _currentTurn to either PlayerTurn.Player1 or PlayerTurn.Player2
        _currentTurn = (UnityEngine.Random.Range(0, 2) == 0) ? PlayerTurn.Player1 : PlayerTurn.Player2;

        yield return new WaitForSeconds(1);

        _currentTurnState = TurnPhase.Start;
        ExecuteCurrentTurnState();
    }

    public void NextPhase()
    {
        if (_currentTurnState == TurnPhase.End)
        {
            _currentNumberOfTurns++;

            bool Player1Turn = _currentTurn == PlayerTurn.Player1;
            _currentTurn = Player1Turn ? PlayerTurn.Player2 : PlayerTurn.Player1;

            if (_currentNumberOfTurns % 2 == 0)
            {
                _isFirstRound = false;
                _currentNumberOfRounds++;
            }

            _currentTurnState = TurnPhase.Start;
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
            case TurnPhase.Start:
                //Debug.Log("StartTurn event called.");
                StartPhase?.Invoke(_currentTurn);

                NextPhase(); //The start phase is currently a placeholder
                break;

            case TurnPhase.Play:
                //Debug.Log("PlayPhase event called.");
                PlayPhase?.Invoke(_currentTurn);
                break;

            case TurnPhase.Action:
                //Debug.Log("ActionPhase event called.");
                ActionPhase?.Invoke(_currentTurn);

                PrepareActionPhase();
                StartCoroutine(ExecuteActions(_activeCards));
                break;

            case TurnPhase.Advance:
                StartCoroutine(ExecuteAdvance());
                break;

            case TurnPhase.Draw:
                //Debug.Log("DrawCards event called.");
                DrawPhase?.Invoke(_currentTurn);

                NextPhase();
                break;

            case TurnPhase.End:
                //Debug.Log("EndTurn event called.");
                EndPhase?.Invoke(_currentTurn);

                NextPhase(); //The end phase is currently a placeholder
                break;
        }
    }

    #region ACTION PHASE
    private void PrepareActionPhase()
    {
        _activeCards.Clear();

        _activeCards = GetActiveCards();
        //Debug.Log($"Active cards: {_activeCards}");
    }

    public List<UnitCard> GetActiveCards()
    {
        List<UnitCard> activeCards = new List<UnitCard>();

        int startColumn = _currentTurn == PlayerTurn.Player1 ? 0 : 3;
        int endColumn = _currentTurn == PlayerTurn.Player1 ? 3 : 6;

        for (int x = startColumn; x < endColumn; x++)
        {
            List<UnitCard> cardsInColumn = CardsInColumn(GridManager.Instance.Grid, x);
            activeCards.AddRange(cardsInColumn);
        }

        return activeCards;
    }

    private IEnumerator ExecuteActions(List<UnitCard> activeCards)
    {
        foreach (UnitCard card in activeCards)
        {
            float duration = card.TriggerAction();
            yield return new WaitForSeconds(duration);
        }

        NextPhase();
    }
    #endregion

    /// <summary>
    /// Responsible for automating advance phase, invokes the AdvancePhase event and waits before moving to next phase.
    /// </summary>
    private IEnumerator ExecuteAdvance()
    {
        if (GridManager.Instance.CanAdvance())
        {
            //Debug.Log("Advance event called.");
            IncrementScale();
            AdvancePhase?.Invoke(_currentTurn);

            yield return new WaitForSeconds(2f);
        }

        NextPhase();
    }

    /// <summary>
    /// Increments or decrements the scale based on the current game state and updates the visibility of columns accordingly.
    /// Triggers a win condition when the scale reaches +5 or -5.
    /// </summary>
    public void IncrementScale()
    {
        bool player1Turn = _currentTurn == PlayerTurn.Player1;

        _scale += player1Turn ? 1 : -1;

        UpdateColumnsVisibility();

        if (_scale == 4)
        {
            // Player 1 wins
        }
        if (_scale == -4)
        {
            // Player 2 wins
        }
    }

    /// <summary>
    /// Updates the visibility of columns based on the current value of _scale.
    /// </summary>
    private void UpdateColumnsVisibility()
    {
        GridManager.Instance.ColumnSetActive(5, !(_scale > 1));
        GridManager.Instance.ColumnSetActive(4, !(_scale > 2));
        GridManager.Instance.ColumnSetActive(1, !(_scale < -2));
        GridManager.Instance.ColumnSetActive(0, !(_scale < -1));
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(GameCursor.WorldPosition, .25f);
    }

    public bool ObjectSelected { get; set; }
    public int Scale { get { return _scale; } }
    public PlayerTurn CurrentTurn { get { return _currentTurn; } }
    public TurnPhase CurrentTurnPhase { get { return _currentTurnState; } }
    public int CurrentNumberOfTurns { get {  return _currentNumberOfTurns; } }
    public bool IsFirstRound { get { return _isFirstRound; } }
}
