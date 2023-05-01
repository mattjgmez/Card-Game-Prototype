using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the card collection UI and handles adding cards to the deck.
/// </summary>
public class CardCollectionManager : MonoSingleton<CardCollectionManager>
{
    [SerializeField] private int _deckLimit = 30;
    [SerializeField] private int _cardLimit = 3;
    [SerializeField] private GameObject _unitCardPrefab;
    [SerializeField] private GameObject _spellCardPrefab;
    [SerializeField] private GameObject _cardInDeckPrefab;
    [SerializeField] private Transform _cardContainer;
    [SerializeField] private Transform _cardInDeckContainer;
    [SerializeField] private List<UnitInfo> _availableUnitCards;
    [SerializeField] private List<SpellInfo> _availableSpellCards;
    [SerializeField] private TMP_InputField _deckNameInputField;
    [SerializeField] private TMP_Text _deckLimitText;
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private DeckCollectionUI _deckCollectionUI;
    [SerializeField] private GameObject _cardInfoUIPrefab;

    private CardInfoUI _cardInfoUI;
    private Dictionary<CardInfo, int> _currentDeck;
    private Dictionary<CardInfo, CardInDeckUI> _cardInDeckUIs = new Dictionary<CardInfo, CardInDeckUI>();

    protected override void Init()
    {
        _currentDeck = new Dictionary<CardInfo, int>();
        PopulateCardCollection();
    }

    /// <summary>
    /// Creates and populates the card collection with the available cards.
    /// </summary>
    private void PopulateCardCollection()
    {
        foreach (CardInfo cardInfo in _availableUnitCards)
        {
            AddCardToCollection(cardInfo);
        }

        foreach (CardInfo cardInfo in _availableSpellCards)
        {
            AddCardToCollection(cardInfo);
        }
    }

    private void AddCardToCollection(CardInfo cardInfo)
    {
        GameObject cardPrefab = null;

        if (cardInfo is UnitInfo)
        {
            cardPrefab = _unitCardPrefab;
        }
        else if (cardInfo is SpellInfo)
        {
            cardPrefab = _spellCardPrefab;
        }
        else
        {
            Debug.LogError("Invalid CardInfo type.");
            return;
        }

        GameObject card = Instantiate(cardPrefab, _cardContainer);
        CardUI cardUI = card.GetComponent<CardUI>();

        if (cardUI != null)
        {
            cardUI.SetCardInfo(cardInfo);
            cardUI.CardUILeftClicked += AddCardToDeck;
            cardUI.CardUIRightClicked += ToggleCardInfoUI;
        }
    }

    /// <summary>
    /// Adds the given card to the current deck or increments its count if it already exists.
    /// </summary>
    /// <param name="cardInfo">The card to add to the deck.</param>
    public void AddCardToDeck(CardInfo cardInfo)
    {
        if (CardsInDeck >= _deckLimit)
        {
            Debug.LogWarning("Deck limit reached.");
            return;
        }

        if (_currentDeck.ContainsKey(cardInfo))
        {
            if (_currentDeck.TryGetValue(cardInfo, out int value) && value < _cardLimit)
            {
                _currentDeck[cardInfo]++;
            }
        }
        else
        {
            _currentDeck.Add(cardInfo, 1);
        }
        AddOrUpdateCardInDeckUI(cardInfo);
    }

    /// <summary>
    /// Removes the given card to the current deck or increments its count if it already exists.
    /// </summary>
    /// <param name="cardInfo">The card to remove from the deck.</param>
    public void RemoveCardFromDeck(CardInfo cardInfo)
    {
        if (_currentDeck.ContainsKey(cardInfo))
        {
            _currentDeck[cardInfo]--;

            if (_currentDeck[cardInfo] <= 0)
            {
                _currentDeck.Remove(cardInfo);
                RemoveCardFromDeckUI(cardInfo);
            }
            else
            {
                AddOrUpdateCardInDeckUI(cardInfo);
            }
        }
    }

    public void ToggleCardInfoUI(CardInfo info)
    {
        if (CardInfoUI.Canvas.enabled)
        {
            CardInfoUI.DisableInfoUI();
        }
        else
        {
            CardInfoUI.EnableInfoUI(info);
        }
    }

    /// <summary>
    /// Remove the CardInDeckUI instance from the dictionary and destroy the corresponding game object.
    /// </summary>
    /// <param name="cardInfo">The CardInfo instance representing the card to be removed in the deck view.</param>
    private void RemoveCardFromDeckUI(CardInfo cardInfo)
    {
        if (_cardInDeckUIs.TryGetValue(cardInfo, out CardInDeckUI cardInDeckUI))
        {
            _cardInDeckUIs.Remove(cardInfo);
            Destroy(cardInDeckUI.gameObject);
        }

        UpdateDeckLimitText();
    }

    /// <summary>
    /// Adds or updates a CardInDeckUI instance for the given card.
    /// If a CardInDeckUI instance for the card already exists, it updates the text with the new card count.
    /// If a CardInDeckUI instance doesn't exist, it creates a new instance, adds it to the _cardInDeckUIs dictionary, and sets the initial card count.
    /// </summary>
    /// <param name="cardInfo">The CardInfo instance representing the card to be added or updated in the deck view.</param>
    private void AddOrUpdateCardInDeckUI(CardInfo cardInfo)
    {
        if (!_cardInDeckUIs.TryGetValue(cardInfo, out CardInDeckUI cardInDeckUI))
        {
            GameObject card = Instantiate(_cardInDeckPrefab, _cardInDeckContainer);
            cardInDeckUI = card.GetComponent<CardInDeckUI>();
            _cardInDeckUIs.Add(cardInfo, cardInDeckUI);
        }

        if (cardInDeckUI != null)
        {
            int amount = _currentDeck[cardInfo];
            cardInDeckUI.UpdateText(cardInfo.Name, amount);
            cardInDeckUI.CardInfo = cardInfo;
        }

        UpdateDeckLimitText();
    }

    public void UpdateDeckLimitText()
    {
        if (_deckLimitText != null)
        {
            _deckLimitText.text = $"{CardsInDeck}/{_deckLimit}";
        }
    }

    public void SaveDeck()
    {
        if (_currentDeck != null && CardsInDeck == _deckLimit)
        {
            SaveDeckSystem.SaveDeckToFile(GetCurrentDeck(), DeckName);
            _deckCollectionUI.InstantiateDeckUIObjects();
            Debug.Log($"Deck saved as {DeckName}.");
        }
        else
        {
            Debug.LogWarning("Deck total is under limit.");
        }
    }

    public void LoadDeck((Dictionary<CardInfo, int>, string) deckToLoad)
    {
        // Clear the current deck
        _currentDeck.Clear();

        // Clear the CardInDeckUIs
        foreach (var cardInDeckUI in _cardInDeckUIs.Values)
        {
            Destroy(cardInDeckUI.gameObject);
        }
        _cardInDeckUIs.Clear();

        // Load the new deck
        foreach (KeyValuePair<CardInfo, int> cardEntry in deckToLoad.Item1)
        {
            CardInfo cardInfo = cardEntry.Key;
            int amount = cardEntry.Value;

            // Add the card to the current deck with the correct amount
            _currentDeck[cardInfo] = amount;
            AddOrUpdateCardInDeckUI(cardInfo);
        }

        _deckNameInputField.text = deckToLoad.Item2;
    }

    /// <summary>
    /// Returns the current deck as a dictionary containing card information and their respective counts.
    /// </summary>
    /// <returns>The current deck.</returns>
    public Dictionary<CardInfo, int> GetCurrentDeck()
    {
        return _currentDeck;
    }

    public int CardsInDeck
    {
        get
        {
            int cardsInDeck = 0;

            foreach (int value in _currentDeck.Values)
            {
                cardsInDeck += value;
            }

            return cardsInDeck;
        }
    }

    public string DeckName
    {
        get
        {
            if (_deckNameInputField.text.Length > 0)
            {
                return _deckNameInputField.text;
            }
            else
            {
                return "New Deck";
            }
        }
    }

    public CardInfoUI CardInfoUI
    {
        get
        {
            if (_cardInfoUI == null)
            {
                _cardInfoUI = Instantiate(_cardInfoUIPrefab).GetComponent<CardInfoUI>();
            }

            return _cardInfoUI;
        }
        set
        {
            _cardInfoUI = value;
        }
    }
}
