using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpellCard : Card
{
    #region PRIVATE VARIABLES
    [Header("Card Information")]
    [SerializeField] private SpellInfo _info;

    [Header("Canvas Components")]
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _descriptionText;

    private List<Tile> _targetTiles = new();
    private Arrow _arrow;
    #endregion

    protected override void Awake()
    {
        base.Awake();

        _arrow = GameObject.Find("Arrow").GetComponent<Arrow>();
    }

    private void Start()
    {
        _info = (SpellInfo)_cardInfo;
    }

    protected override void Update()
    {
        base.Update();

        if (_isSelected)
        {
            _arrow.IsSelected = true;

            Tile targetTile = RaycastToBoard(Camera.main.ScreenPointToRay(Input.mousePosition));
            if (targetTile == null)
            {
                return;
            }

            _targetTiles = SpellSystem.GetTargetTiles(targetTile, _info.AreaOfEffect, _isPlayer_1, _info.ValidTargets);

            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
            {
                PlayToTile(targetTile);
            }

            if (Input.GetMouseButtonDown(1))
            {
                Debug.Log("Card deselected");
                _isSelected = false;
                _arrow.IsSelected = false;

                DisableTargetTiles();
            }
        }
    }

    #region PLACEMENT
    private void PlayToTile(Tile targetTile)
    {
        List<UnitCard> targetCards = new List<UnitCard>();

        foreach (Tile tile in  _targetTiles)
        {
            if (tile.ActiveCard != null)
            {
                targetCards.Add(tile.ActiveCard);
            }
        }

        if (targetTile != null)
        {
            // Spawn VFX
            SpellSystem.PerformSpell(this, targetCards);

            _isSelected = false;
            _arrow.IsSelected = false;

            DisableTargetTiles();

            HandManager.Instance.RemoveCardFromHand(gameObject);
            Destroy(gameObject);
        }
    }

    private static void DisableTargetTiles()
    {
        for (int x = 0; x < GridManager.Instance.GridWidth; x++)
        {
            for (int y = 0; y < GridManager.Instance.GridHeight; y++)
            {
                GridManager.Instance.Grid[x, y].SetTileActive(false);
            }
        }
    }

    private Tile RaycastToBoard(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Board")))
        {
            return hit.transform.gameObject.GetComponent<Tile>();
        }

        return null;
    }
    #endregion

    public SpellInfo SpellInfo { get { return _info; } }
}
