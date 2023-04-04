using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckUI : Interactable
{
    public (Dictionary<CardInfo, int>, string) Deck { get; set; }
    public bool InCardCollection { get; set; }

    protected override void OnLeftClick()
    {
        GameManager.Instance.CurrentDeck = Deck;

        if (!InCardCollection)
            return;

        CardCollectionManager.Instance.LoadDeck(Deck);
    }
}
