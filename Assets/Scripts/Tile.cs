using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color _defaultColor, _activeColor;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private GameObject _highlight;
    [SerializeField] private Card _activeCard;

    private bool _hasCard = false;
    private bool _isPlayer_1;
    private Vector2Int _gridPosition;


    private void Start()
    {
        SetTileActive(false);

        ResetTile();
    }

    private void OnMouseDown()
    {
        if (_hasCard && !_activeCard.IsExhausted && !TurnManager.Instance.ObjectSelected)
        {
            _activeCard.ActionHandler.Selected = !_activeCard.ActionHandler.Selected;
        }
    }

    private void OnMouseEnter()
    {
        _highlight.SetActive(true);
    }

    private void OnMouseExit()
    {
        _highlight.SetActive(false);
    }

    public void SetGridPosition(int x, int y)
    {
        _gridPosition = new Vector2Int(x, y);
    }

    public void SetCard(Card card)
    {
        _activeCard = card;
        _hasCard = true;
    }

    public void ResetTile()
    {
        _activeCard = null;
        _hasCard = false;
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

    public Card ActiveCard { get { return _activeCard; } }
    public bool HasCard { get { return _hasCard; } }
    public bool GetIsPlayer1 { get { return _isPlayer_1; } }
    public Vector2Int GridPosition { get { return _gridPosition; } }
}
