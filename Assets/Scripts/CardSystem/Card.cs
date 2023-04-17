using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Card : EventReceiver
{
    [SerializeField] protected Canvas _canvas;

    protected CardInfo _cardInfo;
    protected bool _isPlayer_1;
    protected bool _inHand = true;
    protected bool _isSelected;
    protected Vector3 _initialPosition;
    protected int _initialSortingOrder = 1;
    protected int _hoverSortingOrder = 5;

    protected virtual void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
    }

    protected override void OnLeftClick()
    {
        if (TurnManager.Instance.CurrentTurn == PlayerTurn.Player1 && _inHand)
        {
            _isSelected = !_isSelected;
        }
    }

    protected override void OnRightClick()
    {
        // Show extended information.
    }

    public void OnMouseEnter()
    {
        _initialPosition = transform.position;

        // Raise the card and increase its canvas sorting order when hovered over
        transform.position = _initialPosition + new Vector3(0, .5f, 0);
        _canvas.sortingOrder = _initialSortingOrder + _hoverSortingOrder;
    }

    public void OnMouseExit()
    {
        // Reset the card's position and canvas sorting order when no longer hovered over
        transform.position = _initialPosition;
        _canvas.sortingOrder = _initialSortingOrder;
    }

    public CardInfo CardInfo { get { return _cardInfo; } set { _cardInfo = value; } }
    public bool IsPlayer1 { get { return _isPlayer_1; } }
}
