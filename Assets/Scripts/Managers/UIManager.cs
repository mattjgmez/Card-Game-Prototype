using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] Canvas _pauseMenuCanvas;
    [SerializeField] TMP_Text _pauseText;

    [SerializeField] Slider _scaleSlider;

    [SerializeField] GameObject _cardInfoUIPrefab;
    private CardInfoUI _cardInfoUI;

    [SerializeField] GameObject _endTurnButton;

    private void OnEnable()
    {
        TurnManager.Instance.PlayPhase += EnableEndTurnButton;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetPauseMenu(!_pauseMenuCanvas.enabled, "Game Paused");
        }
    }

    // Win/Loss UI Method
    public void SetPauseMenu(bool value, string text)
    {
        if (_pauseMenuCanvas == null)
        {
            Debug.LogError("UIManager.SetPauseMenu: Pause Canvas not found.");
            return;
        }

        // Pause or Unpause game
        Time.timeScale = value ? 0 : 1;
        _pauseMenuCanvas.enabled = value;

        // Set text
        _pauseText.text = text;
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

    // Toggle End Turn button?
    public void EnableEndTurnButton(PlayerTurn turn)
    {
        if (turn == PlayerTurn.Player1)
        {
            _endTurnButton.SetActive(true);
        }
    }

    #region BUTTON METHODS
    public void DisableEndTurnButton()
    {
        _endTurnButton.SetActive(false);
    }

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
