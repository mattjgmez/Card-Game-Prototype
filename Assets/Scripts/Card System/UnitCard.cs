using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UnitCard : Card
{
    #region PRIVATE VARIABLES
    [Header("Canvas Components")]
    [SerializeField] private GameObject _inHandVisuals;
    [SerializeField] private Animator _artAnimator;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _costText;
    [SerializeField] private TMP_Text _attackText;
    [SerializeField] private TMP_Text _healthText;
    [SerializeField] private List<Image> _actionImages; 

    [Header("On Board Components")]
    [SerializeField] private Vector3 _tileOffset;
    [SerializeField] private SpriteRenderer _artRenderer;
    [SerializeField] private SpriteRenderer _shadowRenderer;
    [SerializeField] private Color _defaultColor;
    [SerializeField] private Color _exhaustedColor;

    //private string _name;
    private int _power;
    private int _health;
    private int _cost;
    private int _maxHealth;
    private List<ActionInfo> _actions;
    private int _nextAction = 0;
    private bool _isProvoked;
    private Tile _currentTile;
    private Collider _cardCollider;
    private UnitCard _provokingCard;
    private Arrow _arrow;
    #endregion

    protected override void Awake()
    {
        base.Awake();

        _cardCollider = GetComponent<Collider>();
        _arrow = GameObject.Find("Arrow").GetComponent<Arrow>();
    }

    protected override void Update()
    {
        base.Update();

        if (_isSelected)
        {
            _arrow.IsSelected = true;
            GridManager.Instance.TogglePlayerSpaces(true);

            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
            {
                Tile targetTile = RaycastToBoard(Camera.main.ScreenPointToRay(Input.mousePosition));

                PlayToTile(targetTile);
            }

            if (Input.GetMouseButtonDown(1))
            {
                Debug.Log("Card deselected");
                _isSelected = false;
                _arrow.IsSelected = false;
                GridManager.Instance.TogglePlayerSpaces(false);
            }
        }
    }

    #region EVENTS
    protected override void OnAdvance(PlayerTurn turn)
    {
        if (!_inHand)
        {
            StartCoroutine(AdvanceCoroutine());
        }
    }
    #endregion

    #region PLACEMENT/ADVANCING
    public void PlayToTile(Tile targetTile)
    {
        if (!PlayerManager.Instance.CanPlayCard(_cost, _isPlayer_1 ? 1 : 2))
        {
            GridManager.Instance.TogglePlayerSpaces(false);
            Debug.LogWarning("Not enough supply to play this card.");
            return;
        }

        if (targetTile != null && !targetTile.HasCard)
        {
            ChangeTile(targetTile);
            EnterPlay();

            _isSelected = false;
            _arrow.IsSelected = false;
            GridManager.Instance.TogglePlayerSpaces(false);
        }
    }

    /// <summary>
    /// Coroutine that advances the card if it's the owner's turn.
    /// </summary>
    /// <returns>An IEnumerator to be used in a coroutine.</returns>
    private IEnumerator AdvanceCoroutine()
    {
        if (IsOwnersTurn())
        {
            PlayAnimation(1);
            yield return new WaitForSeconds(0.6f);
            PlayAnimation(0);
            yield break;
        }

        Tile targetTile = GetTargetTile();
        if (targetTile != null)
        {
            while (ShouldKeepMoving(targetTile.transform.position))
            {
                MoveCard(2 * Time.deltaTime);
                yield return null;
            }

            ChangeTile(targetTile);
        }
    }

    /// <summary>
    /// Calculates the target position for the card.
    /// </summary>
    /// <returns>The target position as a Vector3.</returns>
    private Tile GetTargetTile()
    {
        int direction = _isPlayer_1 ? 1 : -1;
        int x = _currentTile.GridPosition.x + direction;
        int y = _currentTile.GridPosition.y;
        return GridManager.Instance.Grid[x, y];
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
            _currentTile.ActiveCard = null;
        }

        _currentTile = targetTile;
        transform.position = _currentTile.transform.position + _tileOffset;
        _currentTile.ActiveCard = this;
    }
    #endregion

    #region ACTION SYSTEM
    public float TriggerAction()
    {
        ActionInfo action = _actions[_nextAction];
        List<UnitCard> targetCards = ActionSystem.TargetCards(this, action);

        if (targetCards.Count <= 0)
        {
            return 0;
        }

        float actionAnimLength = PlayActionAnimation(_nextAction) + .5f;
        ActionSystem.PerformAction(this, action, targetCards);

        _nextAction++;
        if (_nextAction >= _actions.Count)
        {
            _nextAction = 0;
        }

        return actionAnimLength;
    }
    #endregion

    #region STAT METHODS
    /// <summary>
    /// Triggers the card's death behavior.
    /// </summary>
    void TriggerDeath()
    {
        _currentTile.ActiveCard = null;
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

    public void SetProvoked(bool value, UnitCard provokingCard)
    {
        _isProvoked = value;
        _provokingCard = provokingCard;
    }
    #endregion

    #region INITIALIZATION
    protected override void InitializeInfo()
    {
        base.InitializeInfo();

        _artRenderer.enabled = false;

        UnitInfo info = CardInfo as UnitInfo;

        _power = info.Power;
        _health = info.Health;
        _cost = info.Cost;
        _actions = info.Actions;

        _maxHealth = info.Health;

        _nameText.text = _name;

        SetActionIcons();
        UpdateText();

        //Debug.Log($"UnitCard.InitializeInfo: Name={_name}, Power={_power}, Health={_health}, Cost={_cost}, MaxHealth={_maxHealth}, Actions={_actions}");
    }

    private void EnterPlay()
    {
        PlayerManager playerManager = PlayerManager.Instance;
        HandManager handManager = HandManager.Instance;

        handManager.RemoveCardFromHand(gameObject, _isPlayer_1 ? 1 : 2);

        _isPlayer_1 = _currentTile.GridPosition.x < 3;
        _artRenderer.flipX = !_isPlayer_1;

        ToggleCardCanvas(false);

        int player = _isPlayer_1 ? 1 : 2;

        playerManager.LowerSupply(_cost, player);
        playerManager.UpdateSupplyText(player);

        if (_isPlayer_1)
        {
            handManager.CenterCardsInHand(1);
        }
    }

    private void ToggleCardCanvas(bool value)
    {
        _cardCollider.enabled = value;
        _inHand = value;
        _inHandVisuals.SetActive(value);
        _shadowRenderer.enabled = !value;
        _artRenderer.enabled = !value;
    }

    private void UpdateText()
    {
        _healthText.text = _health.ToString();
        _attackText.text = _power.ToString();
        _costText.text = _cost.ToString();
    }

    private void SetActionIcons()
    {
        for (int i = 0; i < _actionImages.Count; i++)
        {
            if (i < _actions.Count)
            {
                _actionImages[i].sprite = _actions[i].Sprite;
            }
            else
            {
                _actionImages[i].enabled = false;
            }
        }
    }
    #endregion

    #region ANIMATION
    /// <summary>
    /// Plays the specified animation.
    /// </summary>
    /// <param name="animationState">The animation state to set.</param>
    private void PlayAnimation(int animationState)
    {
        if (_artAnimator != null)
        {
            _artAnimator.SetInteger("AnimState", animationState);
        }
        else
        {
            Debug.LogWarning("Animator is null. Cannot play animation.");
        }
    }

    private void PlayDeathAnimation()
    {
        if (_artAnimator != null)
        {
            _artAnimator.SetTrigger("Death");
        }
        else
        {
            Debug.LogWarning("Animator is null. Cannot play death animation.");
        }
    }

    private void PlayHurtAnimation()
    {
        if (_artAnimator != null)
        {
            _artAnimator.SetTrigger("Hurt");
        }
        else
        {
            Debug.LogWarning("Animator is null. Cannot play hurt animation.");
        }
    }

    private void SetCardColor(Color color)
    {
        if (_artRenderer != null)
        {
            _artRenderer.color = color;
        }
        else
        {
            Debug.LogWarning("Renderer is null. Cannot set card color.");
        }
    }

    private float PlayActionAnimation(int animationState)
    {
        if (_artAnimator != null)
        {
            animationState++;
            _artAnimator.SetInteger("ActionState", animationState);

            float animationLength = GetAnimationLength($"Action{animationState}");
            StartCoroutine(ResetActionState(animationLength));

            return animationLength;
        }
        else
        {
            Debug.LogWarning("Animator is null. Cannot play action animation.");
            return 0f;
        }
    }

    private IEnumerator ResetActionState(float duration)
    {
        yield return new WaitForSeconds(duration);

        if (_artAnimator != null)
        {
            _artAnimator.SetInteger("ActionState", 0);
        }
        else
        {
            Debug.LogWarning("Animator is null. Cannot reset action state.");
        }
    }

    private float GetAnimationLength(string animationClipName)
    {
        if (_artAnimator != null)
        {
            AnimatorControllerParameter[] parameters = _artAnimator.parameters;
            RuntimeAnimatorController runtimeAnimatorController = _artAnimator.runtimeAnimatorController;
            AnimationClip[] animationClips = runtimeAnimatorController.animationClips;

            foreach (AnimationClip clip in animationClips)
            {
                if (clip.name == animationClipName)
                {
                    return clip.length;
                }
            }
        }
        else
        {
            Debug.LogWarning("Animator is null. Cannot get animation length.");
        }

        Debug.LogWarning($"Animation clip '{animationClipName}' not found.");
        return 0f;
    }
    #endregion

    #region PUBLIC VARIABLES
    //public string GetName { get { return _name; } }
    public int Power { get { return _power; } }
    public int Health { get { return _health; } }
    public int MaxHealth { get { return _maxHealth; } }
    public int Cost { get { return _cost; } }
    public bool IsProvoked { get { return _isProvoked; } }
    public List<ActionInfo> Actions { get { return _actions; } }
    public ActionInfo NextAction { get { return _actions[_nextAction]; } }
    public Tile CurrentTile { get { return _currentTile; } set { _currentTile = value; } }
    public Animator ArtAnimator { get { return _artAnimator; } }
    public UnitCard ProvokingCard { get { return _provokingCard; } }
    public Canvas Canvas { get { return _canvas; } }
    #endregion

    private bool IsOwnersTurn()
    {
        TurnManager turnManager = TurnManager.Instance;

        if ((_isPlayer_1 && turnManager.CurrentTurn == PlayerTurn.Player1) || (!_isPlayer_1 && turnManager.CurrentTurn == PlayerTurn.Player2))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override string ToString()
    {
        return $"UnitCard: Name={GetName}, Power={Power}, Health={Health}, Cost={Cost}, MaxHealth={MaxHealth}, Actions={(Actions != null ? string.Join(", ", Actions) : "null")}, NextAction={NextAction}, IsProvoked={IsProvoked}, CurrentTile={(CurrentTile != null ? CurrentTile.ToString() : "null")}";
    }
}
