using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardInfoUI : Interactable
{
    [SerializeField] private Canvas _canvas;

    [SerializeField] private UnitCardUI _unitUI;
    [SerializeField] private GameObject[] _actionObjects;
    [SerializeField] private Image[] _actionIcons;
    [SerializeField] private TMP_Text[] _actionDescriptionTexts;

    private void Awake()
    {
        _canvas.worldCamera = Camera.main;
    }

    protected override void OnRightClick()
    {
        DisableInfoUI();
    }

    public void EnableInfoUI(CardInfo info)
    {
        _canvas.enabled = true;

        if (info is UnitInfo)
        {
            UnitInfo unitInfo = info as UnitInfo;
            List<ActionInfo> actions = unitInfo.Actions;

            _unitUI.SetCardInfo(unitInfo);

            for (int i = 0; i < actions.Count; i++)
            {
                _actionObjects[i].SetActive(true);
                _actionIcons[i].sprite = actions[i].Sprite;
                _actionDescriptionTexts[i].text = FormatText(actions[i]);
            }
        }
    }

    public void DisableInfoUI()
    {
        foreach(GameObject actionObject in _actionObjects) 
        {
            actionObject.SetActive(false);
        }

        _canvas.enabled = false;
    }

    private string FormatText(ActionInfo action)
    {
        string formattedText = $"<b>{action.Range}</b> ";

        foreach (ActionKeyword keyword in action.Keywords)
        {
            formattedText += $"<b>{keyword}</b> ";
        }
        formattedText += action.Description;

        return formattedText;
    }

    public Canvas Canvas { get { return _canvas; } }
}
