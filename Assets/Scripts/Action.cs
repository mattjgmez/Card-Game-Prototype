using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action : MonoBehaviour
{
    [SerializeField] ActionInfo _actionInfo;

    int _cost;
    bool _isPlayer_1;
    bool _isSelected;
    SpriteRenderer _spriteRenderer;
    Card _card;
    List<Tile> _validTiles;
    Arrow _arrow;
    LayerMask _boardMask;

    void Start()
    {
        _card = GetComponentInParent<Card>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _arrow = GameObject.Find("Arrow").GetComponent<Arrow>();
        _boardMask = LayerMask.GetMask("Board");

        _spriteRenderer.sprite = _actionInfo.GetSprite;

        _card.PlayedFromHand += InitializeAction;
    }

    void Update()
    {
        if (_isSelected)
        {
            HandleTargetting();
        }
    }

    void OnMouseDown()
    {
        if (GameManager.Instance.CurrentTurn == GameState.Player1Turn)
            _isSelected = true;
    }

    void HandleTargetting()
    {
        if (_actionInfo.HasKeyword(ActionKeywords.DrawCard))
        {
            _card.LowerEnergy(_cost);
            //draw card
            return;
        }

        foreach (Tile tile in GridManager.Instance.GetTilesInRange
            (_card.CurrentSpace.GridPosition, _actionInfo.GetRange, _isPlayer_1))
            tile.SetColor(tile.ActiveColor);

        _arrow.IsSelected = true;

        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _boardMask))
            {
                Tile hitSpace = hit.transform.gameObject.GetComponent<Tile>();

                if (_validTiles.Contains(hitSpace) && hitSpace.ActiveCard && _actionInfo.GetCost <= _card.GetEnergy && !_card.IsExhausted)
                {
                    PerformAction(hitSpace);
                    Debug.Log("Action performed");
                }
            }

            _isSelected = false;
            _card.GetActionUIAnimator.SetBool("IsSelected", false);
            _arrow.IsSelected = false;

            foreach (Tile tile in _validTiles)
                tile.SetColor(tile.DefaultColor);
        }
    }

    #region ACTION METHODS
    void PerformAction(Tile targetSpace)
    {
        if (_actionInfo.HasKeyword(ActionKeywords.Heal))
        {
            if (_actionInfo.HasKeyword(ActionKeywords.Cleave))
                foreach (Tile tile in _validTiles)
                    tile.ActiveCard?.Heal(_card.GetPower);

            targetSpace.ActiveCard.Heal(_card.GetPower);
        }
        else if (_actionInfo.HasKeyword(ActionKeywords.Damage))
        {
            _card.GetAnimator.SetTrigger("Attack1");
            int damageDealt = 0;

            if (_actionInfo.HasKeyword(ActionKeywords.Cleave))
            {
                foreach (Tile tile in _validTiles)
                {
                    tile.ActiveCard?.TakeDamage(_card.GetPower);
                    damageDealt += _card.GetPower;
                }
            }
            else
            {
                targetSpace.ActiveCard.TakeDamage(_card.GetPower);
                damageDealt += _card.GetPower;
            }

            if (_actionInfo.HasKeyword(ActionKeywords.Drain))
                _card.Heal(damageDealt);
        }

        if (_actionInfo.HasKeyword(ActionKeywords.Provoke))
            targetSpace.ActiveCard.SetProvoked(true, _card);

        if (_actionInfo.HasKeyword(ActionKeywords.Momentum) && targetSpace.ActiveCard.GetHealth <= 0)
            return;

        _card.LowerEnergy(_cost);
    }
    #endregion

    #region SETUP METHODS
    void InitializeAction(Card card)
    {
        _isPlayer_1 = _card.IsPlayer_1;
        _cost = _actionInfo.GetCost;
        _card.OnMovement += SetRange;
        _card.OnProvoke += SetRange;
    }

    void DisableAction()
    {
        _card.PlayedFromHand -= InitializeAction;
        _card.OnMovement -= SetRange;
        _card.OnProvoke -= SetRange;
    }

    void OnDisable()
    {
        DisableAction();
    }

    void SetRange(Card card)
    {
        _validTiles?.Clear();

        if (card.IsProvoked)
        {
            _validTiles.Add(card.GetProvokingCard.CurrentSpace);
            return;
        }

        _validTiles = GridManager.Instance.GetTilesInRange(card.CurrentSpace.GridPosition, _actionInfo.GetRange, _isPlayer_1);
    }
    #endregion
}
