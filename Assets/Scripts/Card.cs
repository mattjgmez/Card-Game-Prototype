using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Card : Interactable
{
    #region PRIVATE VARIABLES
    [Header("Card Information")]
    [SerializeField] private CardInfo _info;
    [SerializeField] private bool _isPlayer_1;
    [SerializeField] private ActionHandler _actionHandler;

    [Header("Canvas Components")]
    [SerializeField] private Canvas _canvas;
    [SerializeField] private GameObject _inHandVisuals;
    [SerializeField] private Animator _artAnimator;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _costText;
    [SerializeField] private TMP_Text _attackText;
    [SerializeField] private TMP_Text _healthText;
    [SerializeField] private TMP_Text _energyText;

    [Header("On Board Components")]
    [SerializeField] private Vector3 _tileOffset;
    [SerializeField] private SpriteRenderer _artRenderer;
    [SerializeField] private SpriteRenderer _shadowRenderer;
    [SerializeField] private Color _defaultColor;
    [SerializeField] private Color _exhaustedColor;

    private int _power;
    private int _health;
    private int _energy;
    private int _cost;
    private int _maxHealth;
    private int _maxEnergy;
    private bool _isExhausted;
    private bool _inHand = true;
    private bool _isSelected;
    private bool _isProvoked;
    private Tile _currentTile;
    private Collider _cardCollider;
    private Card _provokingCard;
    private Arrow _arrow;
    private Vector3 _initialPosition;
    private int _initialSortingOrder = 1;
    private int _hoverSortingOrder = 5;
    #endregion

    private void Awake()
    {
        _cardCollider = GetComponent<Collider>();
        _arrow = GameObject.Find("Arrow").GetComponent<Arrow>();
    }

    private void Start()
    {
        InitializeVariables();

        _artRenderer.enabled = false;
        _artRenderer.flipX = !_isPlayer_1;

        _actionHandler.Initialize(_info);
    }

    private void Update()
    {
        transform.rotation = Camera.main.transform.rotation;

        if (_isSelected)
        {
            _arrow.IsSelected = true;
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
            {
                PlayToTile();
            }

            if (Input.GetMouseButtonDown(1))
            {
                Debug.Log("Card deselected");
                _isSelected = false;
                _arrow.IsSelected = false;
            }
        }
    }

    protected override void OnLeftClick()
    {
        if (TurnManager.Instance.CurrentTurn == GameState.Player1Turn && _inHand && PlayerManager.Instance.CanPlayCard(_cost))
        {
            _isSelected = !_isSelected;
            Debug.Log($"{_info.Name} is selected.");
        }
    }

    protected override void OnRightClick()
    {
        //Show extended information.
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

    private void PlayToTile()
    {
        if (!PlayerManager.Instance.CanPlayCard(_cost))
        {
            Debug.LogWarning("Not enough supply to play this card.");
            return;
        }

        Tile targetTile = RaycastToBoard(Camera.main.ScreenPointToRay(Input.mousePosition));
        if (targetTile != null)
        {
            EnterPlay();

            if (!targetTile.HasCard)
                ChangeTile(targetTile);

            _isSelected = false;
            _arrow.IsSelected = false;
        }
    }

    #region ADVANCING/MOVEMENT METHODS
    void StartAdvance(GameState state)
    {
        StartCoroutine(AdvanceCoroutine());
    }

    /// <summary>
    /// Coroutine that advances the card if it's the owner's turn.
    /// </summary>
    /// <returns>An IEnumerator to be used in a coroutine.</returns>
    IEnumerator AdvanceCoroutine()
    {
        if (IsOwnersTurn())
        {
            PlayAnimation(1);
            yield return new WaitForSeconds(0.6f);
            PlayAnimation(0);
            yield break;
        }

        Vector3 targetPosition = CalculateTargetPosition();
        while (ShouldKeepMoving(targetPosition))
        {
            MoveCard(2 * Time.deltaTime);
            yield return null;
        }

        Tile hitSpace = RaycastToBoard(new Ray(transform.position, transform.forward));
        if (hitSpace != null)
        {
            ChangeTile(hitSpace);
        }
    }

    /// <summary>
    /// Calculates the target position for the card.
    /// </summary>
    /// <returns>The target position as a Vector3.</returns>
    private Vector3 CalculateTargetPosition()
    {
        int direction = _isPlayer_1 ? 1 : -1;
        int x = _currentTile.GridPosition.x + direction;
        int y = _currentTile.GridPosition.y;
        return GridManager.Instance.Grid[x, y].transform.position;
    }

    /// <summary>
    /// Determines whether the card should keep moving towards the target position.
    /// </summary>
    /// <param name="targetPosition">The target position to compare with.</param>
    /// <returns>True if the card should keep moving, false otherwise.</returns>
    private bool ShouldKeepMoving(Vector3 targetPosition)
    {
        return _isPlayer_1 ? transform.position.x < targetPosition.x : transform.position.x > targetPosition.x;
    }

    /// <summary>
    /// Moves the card in the specified direction.
    /// </summary>
    /// <param name="speed">The speed at which to move the card.</param>
    private void MoveCard(float speed)
    {
        transform.Translate(speed * (_isPlayer_1 ? transform.right : -transform.right));
    }

    /// <summary>
    /// Performs a raycast to the board and returns the hit space.
    /// </summary>
    /// <returns>The hit space as a Tile, or null if no space is hit.</returns>
    private Tile RaycastToBoard(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Board")))
        {
            return hit.transform.gameObject.GetComponent<Tile>();
        }

        return null;
    }

    /// <summary>
    /// Changes the tile's space to the target tile.
    /// </summary>
    /// <param name="targetTile">The target tile to move the card to.</param>
    void ChangeTile(Tile targetTile)
    {
        if (_currentTile != null)
        {
            _currentTile.ResetTile();
        }

        _currentTile = targetTile;
        transform.position = _currentTile.transform.position + _tileOffset;
        _currentTile.SetCard(this);
    }
    #endregion

    #region STAT METHODS
    /// <summary>
    /// Triggers the card's death behavior.
    /// </summary>
    void TriggerDeath()
    {
        UnsubscribeEvents();

        _currentTile.ResetTile();
        PlayDeathAnimation();
        Destroy(gameObject, 1);
    }

    public void TakeDamage(int amount, bool isDeathTouch)
    {
        _health -= amount;
        UpdateText();

        if (_health <= 0 || isDeathTouch)
        {
            TriggerDeath();
        }
        else
        {
            PlayHurtAnimation();
        }
    }

    public void Heal(int amount)
    {
        _health += amount;
        _health = Mathf.Clamp(_health, 0, _maxHealth);
        UpdateText();
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
            SetCardColor(_exhaustedColor);
            _energy = 0;
        }
        else
        {
            SetCardColor(_defaultColor);
            _energy = _maxEnergy;
        }

        UpdateText();
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
    #endregion

    #region INITIALIZE METHODS
    void InitializeVariables()
    {
        _power = _info.Power;
        _health = _info.Health;
        _energy = _info.Energy;
        _cost = _info.Cost;

        _maxHealth = _info.Health;
        _maxEnergy = _info.Energy;

        _nameText.text = _info.Name;

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

        HandManager.Instance.RemoveCardFromHand(gameObject);
        HandManager.Instance.CenterCardsInHand();

        PlayerManager.Instance.LowerSupply(_cost);
        PlayerManager.Instance.UpdateSupplyText();

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

        UnsubscribeEvents();
    }

    void UpdateText()
    {
        _healthText.text = _health.ToString();
        _attackText.text = _power.ToString();
        _energyText.text = _energy.ToString();
        _costText.text = _cost.ToString();
    }

    void SubscribeEvents()
    {
        TurnManager.Instance.Advance += StartAdvance;
        TurnManager.Instance.EndTurn += RefreshCard;
    }

    void UnsubscribeEvents()
    {
        TurnManager.Instance.Advance -= StartAdvance;
        TurnManager.Instance.EndTurn -= RefreshCard;
    }
    #endregion

    #region ANIMATION METHODS
    /// <summary>
    /// Plays the specified animation.
    /// </summary>
    /// <param name="animationState">The animation state to set.</param>
    private void PlayAnimation(int animationState)
    {
        _artAnimator.SetInteger("AnimState", animationState);
    }

    private void PlayDeathAnimation()
    {
        _artAnimator.SetTrigger("Death");
    }

    private void PlayHurtAnimation()
    {
        _artAnimator.SetTrigger("Hurt");
    }

    private void SetCardColor(Color color)
    {
        _artRenderer.color = color;
    }
    #endregion

    bool IsOwnersTurn()
    {
        if ((_isPlayer_1 && TurnManager.Instance.CurrentTurn == GameState.Player1Turn)
            || (!_isPlayer_1 && TurnManager.Instance.CurrentTurn == GameState.Player2Turn))
            return true;
        else
            return false;
    }

    #region PUBLIC GET VARIABLES
    public int GetPower { get { return _power; } }
    public int GetHealth { get { return _health; } }
    public int GetEnergy { get { return _energy; } }
    public bool IsPlayer_1 { get { return _isPlayer_1; } }
    public bool IsExhausted { get { return _isExhausted; } }
    public bool IsProvoked { get { return _isProvoked; } }
    public CardInfo CardInfo { get { return _info; } set { _info = value; } }
    public Tile CurrentSpace { get { return _currentTile; } }
    public Animator GetAnimator { get { return _artAnimator; } }
    public Card GetProvokingCard { get { return _provokingCard; } }
    public ActionHandler ActionHandler { get { return _actionHandler; } }
    public Canvas GetCanvas { get { return _canvas; } }
    #endregion
}
