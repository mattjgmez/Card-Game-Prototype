using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : Interactable
{
    [SerializeField] private Color _defaultColor;
    [SerializeField] private Color _activeColor;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private GameObject _highlight;
    [SerializeField] private UnitCard _activeCard = null;

    private bool _isPlayer_1;
    private Vector2Int _gridPosition;

    private void Start()
    {
        SetTileActive(false);
    }

    private void OnMouseEnter()
    {
        _highlight.SetActive(true);

        ToggleTargetTileHighlights(true);
    }

    private void OnMouseExit()
    {
        _highlight.SetActive(false);

        ToggleTargetTileHighlights(false);
    }

    protected override void OnRightClick()
    {
        if (HasCard)
        {
            UIManager.Instance.ToggleCardInfoUI(ActiveCard.CardInfo);
        }
    }

    private void ToggleTargetTileHighlights(bool value)
    {
        if (!HasCard)
        {
            return;
        }

        ActionInfo action = _activeCard.NextAction;
        List<UnitCard> targetCards = ActionSystem.TargetCards(_activeCard, action);

        foreach (UnitCard card in targetCards)
        {
            card.CurrentTile.SetTileActive(value);
        }
    }

    public void SetGridPosition(int x, int y)
    {
        _gridPosition = new Vector2Int(x, y);
    }

    /// <summary>
    /// Toggles the tile's color and collider based on if it should be "active".
    /// </summary>
    /// <param name="value">Whether or not the tile should be active.</param>
    public void SetTileActive(bool value)
    {
        _spriteRenderer.color = value ? _activeColor : _defaultColor;
    }

    public void SetIsPlayer1(bool value)
    {
        _isPlayer_1 = value;
    }

    public UnitCard ActiveCard { get { return _activeCard; } set { _activeCard = value; } }
    public bool HasCard { get { return _activeCard != null; } }
    public bool GetIsPlayer1 { get { return _isPlayer_1; } }
    public Vector2Int GridPosition { get { return _gridPosition; } }
}
