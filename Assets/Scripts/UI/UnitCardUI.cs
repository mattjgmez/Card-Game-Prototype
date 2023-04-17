using System;
using UnityEngine;
using TMPro;

public class UnitCardUI : CardUI
{
    [Header("Card Information")]
    [SerializeField] private UnitInfo _info;

    [Header("Canvas Components")]
    [SerializeField] private TMP_Text _powerText;
    [SerializeField] private TMP_Text _healthText;
    [SerializeField] private TMP_Text _costText;


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
    }

    protected override CardInfo GetCardInfo()
    {
        return _info;
    }
}
