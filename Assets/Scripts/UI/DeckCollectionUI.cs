using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class DeckCollectionUI : MonoBehaviour
{
    [SerializeField] private GameObject _deckUI;
    [SerializeField] private Transform _deckContainer;
    [SerializeField] private bool _inCardCollection;

    private HashSet<string> _instantiatedDeckUIs = new HashSet<string>();

    private void Start()
    {
        InstantiateDeckUIObjects();
    }

    public void InstantiateDeckUIObjects()
    {
        if (_deckUI == null)
        {
            Debug.LogError("Missing DeckUI.");
            return;
        }
        if (_deckContainer == null)
        {
            Debug.LogError("Missing DeckContainer.");
            return;
        }

        List<(Dictionary<CardInfo, int>, string)> savedDecks = SaveDeckSystem.DecksInFolder("CustomDecks");

        foreach ((Dictionary<CardInfo, int>, string) savedDeck in savedDecks)
        {
            // Check if the deckUI object has already been instantiated for this deck name.
            if (_instantiatedDeckUIs.Contains(savedDeck.Item2))
            {
                continue;
            }

            //SaveDeckSystem.DebugDeck(savedDeck.Item1, savedDeck.Item2);

            DeckUI newDeckUI = Instantiate(_deckUI, _deckContainer).GetComponent<DeckUI>();
            TMP_Text deckNameText = newDeckUI.GetComponentInChildren<TMP_Text>();

            newDeckUI.Deck = savedDeck;
            newDeckUI.InCardCollection = _inCardCollection;

            if (deckNameText != null)
            {
                deckNameText.text = savedDeck.Item2;

                // Add the deck name to the HashSet to mark it as instantiated.
                _instantiatedDeckUIs.Add(savedDeck.Item2);
            }
            else
            {
                Debug.LogWarning("Failed to find TMP_Text component in the instantiated DeckUI.");
            }
        }
    }

}
