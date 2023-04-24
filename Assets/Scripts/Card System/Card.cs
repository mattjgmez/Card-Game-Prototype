using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Card : EventReceiver
{
    [SerializeField] protected Canvas _canvas;

    [SerializeField] protected CardInfo _info;
    [SerializeField] protected bool _isPlayer_1;
    protected string _name;
    protected bool _inHand = true;
    protected bool _isSelected;
    protected Vector3 _initialPosition;
    protected int _initialSortingOrder = 1;
    protected int _hoverSortingOrder = 5;

    private void Start()
    {
        InitializeInfo();
    }

    protected virtual void InitializeInfo()
    {
        _info.Init();

        if (_info == null)
        {
            Debug.LogError("Card: CardInfo is null.");
        }

        _name = _info.Name;
        Debug.Log($"Card.InitializeInfo: Name={_name}, Info={_info}");
    }

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

    public CardInfo CardInfo
    {
        get
        {
            return _info;
        }
        set
        {
            _info = value;
        }
    }

    public string GetName { get { return _name; } }
    public bool IsPlayer1 { get { return _isPlayer_1; } set { _isPlayer_1 = value; } }

    public override string ToString()
    {
        if (this is UnitCard card)
        {
            return $"Card as UnitCard: Name={GetName}, Power={card.GetPower}, Health={card.GetHealth}, Cost={card.GetCost}, MaxHealth={card.GetMaxHealth}, Actions={string.Join(", ", card.GetActions)}, NextAction={card.GetNextAction}, IsProvoked={card.IsProvoked}, CurrentTile={(card.CurrentTile != null ? card.CurrentTile.ToString() : "null")}";
        }
        return base.ToString();
    }
}
