using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpellCard : Card
{
    [Header("Canvas Components")]
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _descriptionText;

    [SerializeField] private string _description;
    [SerializeField] private int _power;
    [SerializeField] private Vector2Int _areaOfEffect;

    private List<Tile> _targetTiles = new();
    private Arrow _arrow;

    protected override void Awake()
    {
        base.Awake();

        _arrow = GameObject.Find("Arrow").GetComponent<Arrow>();
    }

    protected override void Update()
    {
        base.Update();

        if (_isSelected)
        {
            _arrow.IsSelected = true;

            Tile targetTile = RaycastToBoard(Camera.main.ScreenPointToRay(Input.mousePosition));
            if (targetTile != null)
            {
                _targetTiles = SpellSystem.GetTargetTiles(targetTile, _areaOfEffect, _isPlayer_1, SpellInfo.ValidTargets);
            }
            else
            {
                DisableTargetTiles();
            }


            if (Input.GetMouseButtonDown(0))
            {
                PlayToTile(targetTile);
                _isSelected = false;
                _arrow.IsSelected = false;

                DisableTargetTiles();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                Debug.Log("Card deselected");
                _isSelected = false;
                _arrow.IsSelected = false;

                DisableTargetTiles();
            }
        }
    }

    #region PLACEMENT
    public void PlayToTile(Tile targetTile)
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
            HandManager handManager = HandManager.Instance;

            // Spawn VFX
            SpellSystem.PerformSpell(this, targetCards);

            _isSelected = false;
            _arrow.IsSelected = false;

            DisableTargetTiles();

            int player = _isPlayer_1 ? 1 : 2;

            HandManager.Instance.RemoveCardFromHand(gameObject, player);

            if (_isPlayer_1)
            {
                handManager.CenterCardsInHand(1);
            }

            Destroy(gameObject);
        }
    }

    private static void DisableTargetTiles()
    {
        for (int x = 0; x < GridManager.GridWidth; x++)
        {
            for (int y = 0; y < GridManager.GridHeight; y++)
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

    #region INITIALIZATION
    protected override void InitializeInfo()
    {
        base.InitializeInfo();

        SpellInfo info = _info as SpellInfo;

        _power = info.Power;
        _areaOfEffect = info.AreaOfEffect;
        _description = info.Description;

        UpdateText();

        Debug.Log($"SpellCard.InitializeInfo: Name={_name}, Power={_power}, AreaOfEffect={_areaOfEffect}");
    }

    private void UpdateText()
    {
        _nameText.text = _name.ToString();
        _descriptionText.text = _description.ToString();
    }
    #endregion

    public SpellInfo SpellInfo { get { return _info as SpellInfo; } }
    public int Power { get { return _power; } set { _power = value; } }
    public Vector2Int AreaOfEffect { get { return _areaOfEffect; } set { _areaOfEffect = value; } }
}
