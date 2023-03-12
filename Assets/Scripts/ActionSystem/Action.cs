using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SetRange();
        }

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
        foreach (Tile tile in _validTiles)
        {
            tile.SetColor(tile.ActiveColor);
        }

        _arrow.IsSelected = true;

        if (Input.GetMouseButtonUp(0) && _actionInfo.GetCost <= _card.GetEnergy && !_card.IsExhausted)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _boardMask))
            {
                Tile hitSpace = hit.transform.gameObject.GetComponent<Tile>();
                List<Tile> targetSpaces = new() { hitSpace };

                if (_actionInfo.HasKeyword(ActionKeywords.Cleave))
                {
                    int x = hitSpace.GridPosition.x;

                    if (hitSpace.GridPosition.y + 1 < 5)
                        targetSpaces.Add(GridManager.Instance.Grid[x, hitSpace.GridPosition.y + 1]);
                    if (hitSpace.GridPosition.y - 1 > 0)
                        targetSpaces.Add(GridManager.Instance.Grid[x, hitSpace.GridPosition.y - 1]);
                }

                if (_actionInfo.HasKeyword(ActionKeywords.Burst))
                {
                    int y = hitSpace.GridPosition.y;
                    int direction = _card.IsPlayer_1 ? 1 : -1;

                    if (hitSpace.GridPosition.x + direction < 6)
                        targetSpaces.Add(GridManager.Instance.Grid[hitSpace.GridPosition.x + direction, y]);
                    if (hitSpace.GridPosition.x + (direction * 2) < 6)
                        targetSpaces.Add(GridManager.Instance.Grid[hitSpace.GridPosition.x + (direction * 2), y]);
                }

                if (_validTiles.Contains(hitSpace) && hitSpace.ActiveCard)
                {
                    PerformAction(targetSpaces);
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
    void PerformAction(List<Tile> targetSpaces)
    {
        bool targetSlain = false;

        foreach(Tile targetSpace in targetSpaces)
        {
            int targetHealth = targetSpace.ActiveCard.GetHealth;

            if (_actionInfo.HasKeyword(ActionKeywords.Heal))
            {
                targetSpace.ActiveCard.Heal(_card.GetPower);
            }

            if (_actionInfo.HasKeyword(ActionKeywords.Damage))
            {
                _card.GetAnimator.SetTrigger("Attack1");
                int damageDealt = 0;

                targetSpace.ActiveCard.TakeDamage(_card.GetPower, _actionInfo.HasKeyword(ActionKeywords.DeathTouch));

                damageDealt += _card.GetPower;

                if ((_actionInfo.HasKeyword(ActionKeywords.Momentum) || _actionInfo.HasKeyword(ActionKeywords.Overkill))
                        && targetHealth <= damageDealt)
                {
                    targetSlain = true;

                    if (targetHealth < damageDealt)
                        GridManager.Instance.Grid[targetSpace.GridPosition.x + 1, targetSpace.GridPosition.y].ActiveCard
                            .TakeDamage(damageDealt - targetHealth, _actionInfo.HasKeyword(ActionKeywords.DeathTouch));
                }

                if (_actionInfo.HasKeyword(ActionKeywords.Drain))
                    _card.Heal(damageDealt);
            }

            if (_actionInfo.HasKeyword(ActionKeywords.Provoke))
                targetSpace.ActiveCard.SetProvoked(true, _card);
        }

        if(!targetSlain || !_actionInfo.HasKeyword(ActionKeywords.Momentum))
            _card.LowerEnergy(_cost);
    }
    #endregion

    #region SETUP METHODS
    void InitializeAction(Card card)
    {
        _isPlayer_1 = _card.IsPlayer_1;
        _cost = _actionInfo.GetCost;
    }

    void DisableAction()
    {
        _card.PlayedFromHand -= InitializeAction;
    }

    void OnDisable()
    {
        DisableAction();
    }

    void SetRange()
    {
        _validTiles?.Clear();

        if (_card.IsProvoked)
        {
            _validTiles.Add(_card.GetProvokingCard.CurrentSpace);
            return;
        }

        switch (_actionInfo.GetRange)
        {
            case ActionRange.Melee:
                _validTiles = ActionRanges.Melee(_card.CurrentSpace.GridPosition, _isPlayer_1, _actionInfo.GetValidTargets);
                break;
            case ActionRange.Ranged:
                _validTiles = ActionRanges.Ranged(_card.CurrentSpace.GridPosition, _isPlayer_1, _actionInfo.GetValidTargets);
                break;
            case ActionRange.Reach:
                _validTiles = ActionRanges.Reach(_card.CurrentSpace.GridPosition, _isPlayer_1, _actionInfo.GetValidTargets);
                break;
            case ActionRange.Global:
                _validTiles = ActionRanges.Global(_card.CurrentSpace.GridPosition, _isPlayer_1, _actionInfo.GetValidTargets);
                break;
        }
    }
    #endregion
}
