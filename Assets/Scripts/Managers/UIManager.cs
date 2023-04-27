using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] Canvas _victoryCanvas;
    [SerializeField] Canvas _lossCanvas;

    [SerializeField] Slider _scaleSlider;

    [SerializeField] GameObject _cardInfoUIPrefab;
    private CardInfoUI _cardInfoUI;

    [SerializeField] Canvas _pauseMenuCanvas;

    [SerializeField] GameObject _endTurnButton;

    private void OnEnable()
    {
        TurnManager.Instance.PlayPhase += EnableEndTurnButton;
    }

    private void OnDisable()
    {
        TurnManager.Instance.PlayPhase -= EnableEndTurnButton;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    // Win/Loss UI Method
    public void EnableGameOverUI(bool isVictory)
    {
        if (_victoryCanvas == null ||  _lossCanvas == null)
        {
            Debug.LogError("UIManager.EnableGameOverUI: Game Over Canvas not found.");
            return;
        }

        // Pause game
        Time.timeScale = 0f;

        // Enable appropriate canvas
        _victoryCanvas.enabled = isVictory;
        _lossCanvas.enabled = !isVictory;
    }

    // Card Info UI Method
    public void ToggleCardInfoUI(CardInfo info)
    {
        if (CardInfoUI.Canvas.enabled)
        {
            CardInfoUI.DisableInfoUI();
        }
        else
        {
            CardInfoUI.EnableInfoUI(info);
        }
    }

    // Adjust Scale
    public void AdjustScaleSlider(int amount)
    {
        if (_scaleSlider == null)
        {
            Debug.LogError("UIManager.AdjustScaleSlider: Scale Slider not found.");
            return;
        }

        _scaleSlider.value = amount;
    }

    // Pause Menu
    public void TogglePauseMenu()
    {
        bool value = _pauseMenuCanvas.enabled;

        Time.timeScale = value ? 0 : 1;
        _pauseMenuCanvas.enabled = !value;
    }

    // Toggle End Turn button?
    public void EnableEndTurnButton(PlayerTurn turn)
    {
        if (turn == PlayerTurn.Player1)
        {
            _endTurnButton.SetActive(true);
        }
    }

    public void DisableEndTurnButton()
    {
        _endTurnButton.SetActive(false);
    }

    #region BUTTON METHODS
    public void QuitGame()
    {
        GameManager.Instance.QuitGame();
    }

    public void BackToMenu()
    {
        GameManager.Instance.ChangeScene("Menus");
    }
    #endregion

    public CardInfoUI CardInfoUI
    {
        get
        {
            if (_cardInfoUI == null)
            {
                _cardInfoUI = Instantiate(_cardInfoUIPrefab).GetComponent<CardInfoUI>();
            }

            return _cardInfoUI;
        }
        set
        {
            _cardInfoUI = value;
        }
    }
}
