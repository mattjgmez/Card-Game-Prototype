using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitCardUI : CardUI
{
    [Header("Card Information")]
    [SerializeField] private UnitInfo _info;

    [Header("Canvas Components")]
    [SerializeField] private TMP_Text _powerText;
    [SerializeField] private TMP_Text _healthText;
    [SerializeField] private TMP_Text _costText;
    [SerializeField] private List<Image> _actionImages;


    public override void SetCardInfo(CardInfo info)
    {
        if (info is UnitInfo unitInfo)
        {
            _info = unitInfo;
            UpdateUI();
        }
    }

    protected override void UpdateUI()
    {
        _healthText.text = _info.Health.ToString();
        _powerText.text = _info.Power.ToString();
        _costText.text = _info.Cost.ToString();
        _nameText.text = _info.Name;
        SetActionIcons();
    }

    private void SetActionIcons()
    {
        for (int i = 0; i < _actionImages.Count; i++)
        {
            if (i < _info.Actions.Count)
            {
                _actionImages[i].sprite = _info.Actions[i].Sprite;
            }
            else
            {
                _actionImages[i].enabled = false;
            }
        }
    }

    protected override CardInfo GetCardInfo()
    {
        return _info;
    }
}
