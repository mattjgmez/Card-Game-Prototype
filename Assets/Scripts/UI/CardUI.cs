using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class CardUI : Interactable
{
    [Header("Canvas Components")]
    [SerializeField] protected TMP_Text _nameText;

    public event Action<CardInfo> OnCardClicked;

    public abstract void SetCardInfo(CardInfo info);

    protected abstract void UpdateUI();

    protected override void OnLeftClick()
    {
        OnCardClicked?.Invoke(GetCardInfo());
    }

    protected abstract CardInfo GetCardInfo();
}
