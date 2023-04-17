using System;
using UnityEngine;
using TMPro;

public class SpellCardUI : CardUI
{
    [Header("Card Information")]
    [SerializeField] private SpellInfo _info;

    [Header("Canvas Components")]
    [SerializeField] private TMP_Text _descriptionText;

    public override void SetCardInfo(CardInfo info)
    {
        if (info is SpellInfo spellInfo)
        {
            _info = spellInfo;
            UpdateUI();
        }
    }

    protected override void UpdateUI()
    {
        _descriptionText.text = _info.Description;
        _nameText.text = _info.Name;
    }

    protected override CardInfo GetCardInfo()
    {
        return _info;
    }
}
