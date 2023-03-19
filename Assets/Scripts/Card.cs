using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Card : MonoBehaviour
{
    [Header("Card Information")]
    [SerializeField] private CardInfo _info;
    [SerializeField] private bool _isPlayer_1;
    [SerializeField] private ActionHandler _actionHandler;

    [Header("Canvas Components")]
    [SerializeField] private GameObject _inHandVisuals;
    [SerializeField] private Animator _artAnimator;
    [SerializeField] private TMP_Text _attackText, _healthText, _energyText;
    [SerializeField] private LayerMask _boardMask, _cardMask;

    [Header("On Board Components")]
    [SerializeField] private SpriteRenderer _artRenderer, _shadowRenderer;
    [SerializeField] private Color _defaultColor, _exhaustedColor;

    private int _power, _health, _energy;
    private int _maxHealth, _maxEnergy;
    private bool _isExhausted;
    private bool _inHand = true;
    private bool _isSelected;
    private bool _isProvoked;
    private Tile _currentSpace;
    private Collider _cardCollider;
    private Card _provokingCard;

    private void Awake()
    {
        InitializeVariables();
        _cardCollider = GetComponent<Collider>();

        _artRenderer.enabled = false;

        if (!_isPlayer_1) _artRenderer.flipX = true;

        _actionHandler.Initialize(_info);
    }

    private void Update()
    {
        transform.rotation = Camera.main.transform.rotation;

        if (_isSelected && _currentSpace == null)
        {
            transform.position = GameCursor.WorldPosition + new Vector3(0, .5f, 0);
        }
    }

    void OnMouseDown()
    {
        if (GameManager.Instance.CurrentTurn == GameState.Player1Turn)
            _isSelected = true;
    }

    void OnMouseUp()
    {
        if (!_isSelected)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, _boardMask))
        {
            EnterPlay();

            Tile hitSpace = hit.transform.gameObject.GetComponent<Tile>();
            if (!hitSpace.HasCard)
                ChangeSpace(hitSpace);

            _isSelected = false;
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (_isSelected)
            {
                _isSelected = false;
            }
        }
    }

    #region ADVANCING/MOVEMENT METHODS
    void StartAdvance(GameState state)
    {
        StartCoroutine(AdvanceCoroutine());
    }

    IEnumerator AdvanceCoroutine()
    {
        if (IsOwnersTurn())
        {
            _artAnimator.SetInteger("AnimState", 1);
            yield return new WaitForSeconds(.6f);
            _artAnimator.SetInteger("AnimState", 0);
            yield break;
        }

        int direction = _isPlayer_1 ? 1 : -1;
        int x = _currentSpace.GridPosition.x + direction;
        int y = _currentSpace.GridPosition.y;
        Vector3 targetPosition = GridManager.Instance.Grid[x, y].transform.position;

        while (_isPlayer_1 ? transform.position.x < targetPosition.x : transform.position.x > targetPosition.x)
        {
            transform.Translate(2 * (_isPlayer_1 ? transform.right : -transform.right) * Time.deltaTime);
            yield return null;
        }

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, _boardMask))
        {
            Tile hitSpace = hit.transform.gameObject.GetComponent<Tile>();
            ChangeSpace(hitSpace);
        }
    }

    void ChangeSpace(Tile targetSpace)
    {
        if (_currentSpace != null)
        {
            _currentSpace.ResetTile();
        }

        _currentSpace = targetSpace;
        transform.position = _currentSpace.transform.position + new Vector3(0, .2f, 0);
        _currentSpace.SetCard(this);
    }
    #endregion

    void TriggerDeath()
    {
        PostCringe();//Unsubscribes events

        _currentSpace.ResetTile();
        _artAnimator.SetTrigger("Death");
        Destroy(gameObject, 1);
    }

    public void TakeDamage(int amount, bool isDeathTouch)
    {
        _health -= amount;
        UpdateText();

        if (_health <= 0 || isDeathTouch)
            TriggerDeath();
        else
            _artAnimator.SetTrigger("Hurt");
    }

    public void Heal(int amount)
    {
        _health += amount;
        UpdateText();

        if (_health > _maxHealth)
            _health = _maxHealth;
    }

    public void LowerEnergy(int amount)
    {
        _energy -= amount;
        Debug.Log($"Energy lowered to {_energy}");

        UpdateText();

        if (_energy <= 0)
        {
            SetExhausted(true);
        }
    }

    public void SetExhausted(bool isExhausted)
    {
        _isExhausted = isExhausted;
        Debug.Log($"Exhausted set to: {_isExhausted}");

        if (isExhausted)
        {
            _artRenderer.color = _exhaustedColor;
            _energy = 0;
            UpdateText();
        }
        else
        {
            _artRenderer.color = _defaultColor;
            _energy = _maxEnergy;
            UpdateText();
        }
    }

    public void SetProvoked(bool value, Card provokingCard)
    {
        _isProvoked = value;
        _provokingCard = provokingCard;
    }

    public void RefreshCard(GameState state)
    {
        if (!IsOwnersTurn())
            return;

        _energy = _maxEnergy;
        SetExhausted(false);
    }

    #region INITIALIZE METHODS
    void InitializeVariables()
    {
        _power = _info.GetAttack;
        _health = _info.GetHealth;
        _energy = _info.GetEnergy;

        _maxHealth = _info.GetHealth;
        _maxEnergy = _info.GetEnergy;

        UpdateText();
    }

    void EnterPlay()
    {
        _cardCollider.enabled = false;
        _inHand = false;
        _inHandVisuals.SetActive(false);
        _shadowRenderer.enabled = true;
        _artRenderer.enabled = true;
        _actionHandler.InHand = false;

        SubscribeEvents();
    }

    void EnterHand()
    {
        _cardCollider.enabled = true;
        _inHand = true;
        _inHandVisuals.SetActive(true);
        _shadowRenderer.enabled = false;
        _artRenderer.enabled = false;
        _actionHandler.InHand = true;

        PostCringe();
    }

    void UpdateText()
    {
        _healthText.text = $"{_health}";
        _attackText.text = $"{_power}";
        _energyText.text = $"{_energy}";
    }

    void SubscribeEvents()
    {
        GameManager.Instance.Advance += StartAdvance;
        GameManager.Instance.EndTurn += RefreshCard;
    }

    void PostCringe()
    {
        GameManager.Instance.Advance -= StartAdvance;
        GameManager.Instance.EndTurn -= RefreshCard;
    }
    #endregion

    bool IsOwnersTurn()
    {
        if ((_isPlayer_1 && GameManager.Instance.CurrentTurn == GameState.Player1Turn)
            || (!_isPlayer_1 && GameManager.Instance.CurrentTurn == GameState.Player2Turn))
            return true;
        else
            return false;
    }

    public int GetPower { get { return _power; } }
    public int GetHealth { get { return _health; } }
    public int GetEnergy { get { return _energy; } }
    public bool IsPlayer_1 { get { return _isPlayer_1; } }
    public bool IsExhausted { get { return _isExhausted; } }
    public bool IsProvoked { get { return _isProvoked; } }
    public CardInfo GetCardInfo { get { return _info; } }
    public Tile CurrentSpace { get { return _currentSpace; } }
    public Animator GetAnimator { get { return _artAnimator; } }
    public Card GetProvokingCard { get { return _provokingCard; } }
    public ActionHandler ActionHandler { get { return _actionHandler; } }
}
