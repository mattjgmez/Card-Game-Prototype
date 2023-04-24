using UnityEngine;

public abstract class EventReceiver : Interactable
{
    protected TurnManager _turnManager;

    protected virtual void Awake()
    {
        if (TurnManager.Instance == null)
        {
            Debug.LogWarning("TurnManager not found.");
            return;
        }
        _turnManager = TurnManager.Instance;

        // Subscribe to events
        _turnManager.StartPhase += OnStartTurn;
        _turnManager.PlayPhase += OnPlayPhase;
        _turnManager.ActionPhase += OnActionPhase;
        _turnManager.AdvancePhase += OnAdvance;
        _turnManager.DrawPhase += OnDrawCards;
        _turnManager.EndPhase += OnEndTurn;
    }

    protected virtual void OnDestroy()
    {
        if (_turnManager == null)
        {
            Debug.LogWarning("TurnManager not found.");
            return;
        }

        // Unsubscribe from events
        _turnManager.StartPhase -= OnStartTurn;
        _turnManager.PlayPhase -= OnPlayPhase;
        _turnManager.ActionPhase -= OnActionPhase;
        _turnManager.AdvancePhase -= OnAdvance;
        _turnManager.DrawPhase -= OnDrawCards;
        _turnManager.EndPhase -= OnEndTurn;
    }

    protected virtual void OnStartTurn(PlayerTurn turn) { }
    protected virtual void OnPlayPhase(PlayerTurn turn) { }
    protected virtual void OnActionPhase(PlayerTurn turn) { }
    protected virtual void OnAdvance(PlayerTurn turn) { }
    protected virtual void OnDrawCards(PlayerTurn turn) { }
    protected virtual void OnEndTurn(PlayerTurn turn) { }
}
