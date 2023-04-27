using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Card : EventReceiver
{
    [SerializeField, Header("Card Components")] 
    protected bool _isPlayer_1;
    [SerializeField] protected Canvas _canvas;

    private CardInfo _cardInfo;
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
        CardInfo.Init();

        if (CardInfo == null)
        {
            Debug.LogError("Card: CardInfo is null.");
        }

        _name = CardInfo.Name;
        Debug.Log($"Card.InitializeInfo: Name={_name}, Info={CardInfo}");
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
        UIManager.Instance.ToggleCardInfoUI(CardInfo);
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
            if (_cardInfo is UnitInfo unitInfo)
            {
                return unitInfo;
            }
            else if (_cardInfo is SpellInfo spellInfo)
            {
                return spellInfo;
            }
            else
            {
                return _cardInfo;
            }
        }
        set
        {
            _cardInfo = value;
        }
    }

    public string GetName { get { return _name; } }
    public bool IsPlayer1 { get { return _isPlayer_1; } set { _isPlayer_1 = value; } }

    public override string ToString()
    {
        if (this is UnitCard card)
        {
            return $"Card as UnitCard: Name={GetName}, Power={card.Power}, Health={card.Health}, Cost={card.Cost}, MaxHealth={card.MaxHealth}, Actions={string.Join(", ", card.Actions)}, NextAction={card.NextAction}, IsProvoked={card.IsProvoked}, CurrentTile={(card.CurrentTile != null ? card.CurrentTile.ToString() : "null")}";
        }
        return base.ToString();
    }
}
