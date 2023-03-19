using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Action : MonoBehaviour
{
    [SerializeField] private ActionInfo _actionInfo;

    private int _cost;
    private bool _isPlayer_1;
    private bool _isSelected;
    private Image _image;
    private Card _card;
    private List<Tile> _validTiles;
    private Arrow _arrow;

    private void Awake()
    {
        _card = GetComponentInParent<Card>();
        _image = GetComponent<Image>();
        _arrow = GameObject.Find("Arrow").GetComponent<Arrow>();

        _isPlayer_1 = _card.IsPlayer_1;
    }

    private void Update()
    {
        if (_isSelected)
        {
            HandleTargeting();
        }
    }

    public void SelectAction()
    {
        if (GameManager.Instance.CurrentTurn == GameState.Player1Turn && _actionInfo.GetCost <= _card.GetEnergy && !_card.IsExhausted)
        {
            SetRange();
            _isSelected = true;
            GameManager.Instance.ObjectSelected = true;
        }
    }

    #region TARGETING METHODS
    private void HandleTargeting()
    {
        SetValidTilesColor(true);

        _arrow.IsSelected = true;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Board")))
            {
                Tile hitSpace = hit.transform.gameObject.GetComponent<Tile>();
                List<Tile> targetSpaces = CalculateTargetTiles(hitSpace);

                if (_validTiles.Contains(hitSpace) && hitSpace.ActiveCard)
                {
                    PerformAction(targetSpaces);
                }
            }

            _isSelected = false;
            _card.ActionHandler.Selected = false;
            _arrow.IsSelected = false;
            GameManager.Instance.ObjectSelected = false;

            SetValidTilesColor(false);
        }
    }

    /// <summary>
    /// Sets the color of valid tiles based on the provided flag.
    /// </summary>
    /// <param name="active">Whether to set the active color (true) or the default color (false).</param>
    private void SetValidTilesColor(bool active)
    {
        foreach (Tile tile in _validTiles)
        {
            tile.SetColor(active ? tile.ActiveColor : tile.DefaultColor);
        }
    }

    /// <summary>
    /// Calculates target tiles based on action keywords and the hit tilee.
    /// </summary>
    /// <param name="hitTile">The hit tile from the raycast.</param>
    /// <returns>A list of target tiles.</returns>
    private List<Tile> CalculateTargetTiles(Tile hitTile)
    {
        List<Tile> targetTiles = new() { hitTile };

        if (_actionInfo.HasKeyword(ActionKeywords.Cleave))
        {
            AddAdjacentTiles(hitTile, targetTiles);
        }

        if (_actionInfo.HasKeyword(ActionKeywords.Burst))
        {
            AddBurstTiles(hitTile, targetTiles);
        }

        return targetTiles;
    }

    /// <summary>
    /// Adds adjacent tiles along the y axis to the given list of tiles.
    /// </summary>
    /// <param name="hitTile">The hit tile from the raycast.</param>
    /// <param name="targetSpaces">The list of tiles to add on to.</param>
    private void AddAdjacentTiles(Tile hitTile, List<Tile> targetSpaces)
    {
        int x = hitTile.GridPosition.x;
        int maxRows = 5;

        if (hitTile.GridPosition.y + 1 < maxRows)
            targetSpaces.Add(GridManager.Instance.Grid[x, hitTile.GridPosition.y + 1]);
        if (hitTile.GridPosition.y - 1 > 0)
            targetSpaces.Add(GridManager.Instance.Grid[x, hitTile.GridPosition.y - 1]);
    }

    /// <summary>
    /// Adds the tiles behind the targeted tiles to the given list of tiles.
    /// </summary>
    /// <param name="hitTile">The hit tile from the raycast.</param>
    /// <param name="targetTiles">The list of tiles to add on to.</param>
    private void AddBurstTiles(Tile hitTile, List<Tile> targetTiles)
    {
        int y = hitTile.GridPosition.y;
        int direction = _card.IsPlayer_1 ? 1 : -1;
        int maxColumns = 6;

        for (int i = 1; i <= 2; i++)
        {
            int newX = hitTile.GridPosition.x + (direction * i);
            if (newX >= 0 && newX < maxColumns)
                targetTiles.Add(GridManager.Instance.Grid[newX, y]);
        }
    }
    #endregion

    #region ACTION METHODS
    /// <summary>
    /// Performs an action on the target tiles based on the action keywords.
    /// </summary>
    /// <param name="targetTiles">The list of target tiles.</param>
    void PerformAction(List<Tile> targetTiles)
    {
        bool targetSlain = false;

        foreach (Tile targetTile in targetTiles)
        {
            int targetHealth = targetTile.ActiveCard.GetHealth;

            if (_actionInfo.HasKeyword(ActionKeywords.Heal))
            {
                PerformHeal(targetTile);
            }

            if (_actionInfo.HasKeyword(ActionKeywords.Damage))
            {
                int damageDealt = PerformDamage(targetTile, targetHealth, ref targetSlain);
            }

            if (_actionInfo.HasKeyword(ActionKeywords.Provoke))
            {
                PerformProvoke(targetTile);
            }
        }

        if (!targetSlain || !_actionInfo.HasKeyword(ActionKeywords.Momentum))
        {
            _card.LowerEnergy(_cost);
        }
    }

    /// <summary>
    /// Performs a heal action on the target tile.
    /// </summary>
    /// <param name="targetTile">The target tile to heal.</param>
    void PerformHeal(Tile targetTile)
    {
        targetTile.ActiveCard.Heal(_card.GetPower);
    }

    /// <summary>
    /// Performs a damage action on the target tile and returns the damage dealt.
    /// </summary>
    /// <param name="targetTile">The target tile to damage.</param>
    /// <param name="targetHealth">The target's initial health.</param>
    /// <param name="targetSlain">Whether the target has been slain.</param>
    /// <returns>The amount of damage dealt.</returns>
    int PerformDamage(Tile targetTile, int targetHealth, ref bool targetSlain)
    {
        _card.GetAnimator.SetTrigger("Attack1");
        int damageDealt = 0;

        targetTile.ActiveCard.TakeDamage(_card.GetPower, _actionInfo.HasKeyword(ActionKeywords.DeathTouch));

        damageDealt += _card.GetPower;

        if ((_actionInfo.HasKeyword(ActionKeywords.Momentum) || _actionInfo.HasKeyword(ActionKeywords.Overkill))
                && targetHealth <= damageDealt)
        {
            targetSlain = true;

            if (targetHealth < damageDealt)
            {
                Card overkillTarget = GridManager.Instance.Grid[targetTile.GridPosition.x + 1, targetTile.GridPosition.y].ActiveCard;
                overkillTarget.TakeDamage(damageDealt - targetHealth, _actionInfo.HasKeyword(ActionKeywords.DeathTouch));
            }
        }

        if (_actionInfo.HasKeyword(ActionKeywords.Drain))
        {
            _card.Heal(damageDealt);
        }

        return damageDealt;
    }

    /// <summary>
    /// Performs a provoke action on the target tile.
    /// </summary>
    /// <param name="targetTile">The target tile to provoke.</param>
    void PerformProvoke(Tile targetTile)
    {
        targetTile.ActiveCard.SetProvoked(true, _card);
    }
    #endregion

    #region SETUP METHODS
    public void SetInfo(ActionInfo info)
    {
        _actionInfo = info;
        _image.sprite = _actionInfo.GetSprite;
        _cost = _actionInfo.GetCost;
    }

    public void SetRange()
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
