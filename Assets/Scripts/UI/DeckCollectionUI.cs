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
        if (_deckUI == null || _deckContainer == null)
        {
            Debug.LogError("Missing reference to either _deckUI or _deckContainer.");
            return;
        }

        List<(Dictionary<CardInfo, int>, string)> savedDecks = SavedDecks();

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
                Debug.LogWarning("Failed to find TMP_Text component in the instantiated _deckUI.");
            }
        }
    }

    private List<(Dictionary<CardInfo, int>, string)> SavedDecks()
    {
        // Create a list to store the saved decks and their names.
        List<(Dictionary<CardInfo, int>, string)> savedDecks = new List<(Dictionary<CardInfo, int>, string)>();

        // Get the CustomDecks directory path.
        string customDecksPath = Application.persistentDataPath + "/CustomDecks";

        // Check if the CustomDecks directory exists.
        if (Directory.Exists(customDecksPath))
        {
            // Get all deck files in the CustomDecks directory.
            string[] deckFiles = Directory.GetFiles(customDecksPath, "*.json");

            // Iterate through each deck file.
            foreach (string deckFile in deckFiles)
            {
                // Get the deck name from the file name without the extension.
                string deckName = Path.GetFileNameWithoutExtension(deckFile);

                // Try to load the deck and add it to the list.
                try
                {
                    // Load the deck dictionary using the SaveDeckSystem.
                    Dictionary<CardInfo, int> deck = SaveDeckSystem.LoadDeckFromFile(deckName);

                    // Add the loaded deck and its name to the savedDecks list.
                    savedDecks.Add((deck, deckName));
                }
                catch (FileNotFoundException)
                {
                    // Log an error if the deck file is not found.
                    Debug.LogError($"Deck file not found: {deckFile}");
                }
            }
        }

        // Return the list of saved decks and their names.
        return savedDecks;
    }
}
