using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class CardUI : Interactable
{
    [Header("Canvas Components")]
    [SerializeField] protected TMP_Text _nameText;

    public event Action<CardInfo> CardUILeftClicked;
    public event Action<CardInfo> CardUIRightClicked;

    public abstract void SetCardInfo(CardInfo info);

    protected abstract void UpdateUI();

    protected override void OnLeftClick()
    {
        CardUILeftClicked?.Invoke(GetCardInfo());
    }

    protected override void OnRightClick()
    {
        Debug.Log("CardUI right clicked.");
        CardUIRightClicked?.Invoke(GetCardInfo());
    }

    protected abstract CardInfo GetCardInfo();
}
