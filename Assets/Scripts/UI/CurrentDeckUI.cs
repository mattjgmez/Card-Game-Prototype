using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrentDeckUI : MonoBehaviour
{
    [SerializeField] TMP_Text _textUI;

    private void OnEnable()
    {
        GameManager.Instance.CurrentDeckChanged += UpdateText;
    }

    private void OnDisable()
    {
        GameManager.Instance.CurrentDeckChanged -= UpdateText;
    }

    public void UpdateText((Dictionary<CardInfo, int>, string) currentDeck)
    {
        if (currentDeck == default)
        {
            _textUI.text = "Choose a Deck";
        }
        else
        {
            _textUI.text = currentDeck.Item2;
        }
    }
}
