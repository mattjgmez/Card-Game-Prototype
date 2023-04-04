using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardUI : Interactable
{
    [Header("Card Information")]
    [SerializeField] private CardInfo _info;
    [SerializeField] private ActionHandler _actionHandler;

    [Header("Canvas Components")]
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _costText;
    [SerializeField] private TMP_Text _powerText;
    [SerializeField] private TMP_Text _healthText;
    [SerializeField] private TMP_Text _energyText;

    public event Action<CardInfo> OnCardClicked;

    public void SetCardInfo(CardInfo info)
    {
        _info = info;
        UpdateUI();
    }

    private void UpdateUI()
    {
        _healthText.text = _info.Health.ToString();
        _powerText.text = _info.Power.ToString();
        _energyText.text = _info.Energy.ToString();
        _costText.text = _info.Cost.ToString();

        _nameText.text = _info.Name;
    }

    protected override void OnLeftClick()
    {
        OnCardClicked?.Invoke(_info);
    }
}
