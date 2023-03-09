using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] Color _defaultColor, _activeColor;
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] GameObject _highlight;
    [SerializeField] Card _activeCard;

    bool _hasCard = false;
    public Vector2Int _gridPosition;

    private void Start()
    {
        SetColor(_defaultColor);

        ResetTile();
    }

    private void OnMouseDown()
    {
        if (_hasCard && !_activeCard.IsExhausted)
        {
            _activeCard.GetActionUIAnimator.SetBool("IsSelected", _activeCard.GetActionUIAnimator.GetBool("IsSelected") ? false : true);
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

    public void SetColor(Color color)
    {
        _spriteRenderer.color = color;
    }

    public Card ActiveCard { get { return _activeCard; } }
    public bool HasCard { get { return _hasCard; } }
    public Vector2Int GridPosition { get { return _gridPosition; } }
    public Color DefaultColor { get { return _defaultColor; } }
    public Color ActiveColor { get { return _activeColor; } }
}
