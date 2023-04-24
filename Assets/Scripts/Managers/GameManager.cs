using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class GameManager : MonoSingleton<GameManager>
{
    private (Dictionary<CardInfo, int>, string) _currentDeck_Player1 = default;
    private (Dictionary<CardInfo, int>, string) _currentDeck_Player2 = default;
    private Queue<CardInfo> _cardQueue_Player1;
    private Queue<CardInfo> _cardQueue_Player2;

    public Action<(Dictionary<CardInfo, int>, string)> CurrentDeckChanged;

    public (Dictionary<CardInfo, int>, string) CurrentDeck_Player1
    {
        get
        {
            return _currentDeck_Player1;
        }
        set
        {
            _currentDeck_Player1 = value;

            CurrentDeckChanged?.Invoke(_currentDeck_Player1);

            string deckName = value == default ? "NULL" : value.Item2;
            //Debug.Log($"Current Deck set to {deckName}.");

            if (value != default)
            {
                _cardQueue_Player1 = CreateCardQueue(value.Item1);
            }
        }
    }

    public (Dictionary<CardInfo, int>, string) CurrentDeck_Player2
    {
        get
        {
            return _currentDeck_Player2;
        }
        set
        {
            _currentDeck_Player2 = value;

            if (value != default)
            {
                _cardQueue_Player2 = CreateCardQueue(value.Item1);
            }
        }
    }

    protected override void Init()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void StartGame()
    {
        if (_currentDeck_Player1 != default)
        {
            ChangeScene("Gameplay");
            ShuffleCardQueue(1);
            ShuffleCardQueue(2);
        }
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Creates a queue of CardInfo objects based on the provided deck.
    /// </summary>
    /// <param name="deck">A Dictionary where the key is a CardInfo object and the value is an integer representing the amount of each card in the deck.</param>
    /// <returns>A Queue of CardInfo objects that represents the deck with the specified card counts.</returns>
    private Queue<CardInfo> CreateCardQueue(Dictionary<CardInfo, int> deck)
    {
        Queue<CardInfo> cardQueue = new Queue<CardInfo>();

        foreach (KeyValuePair<CardInfo, int> entry in deck)
        {
            for (int i = 0; i < entry.Value; i++)
            {
                cardQueue.Enqueue(entry.Key);
            }
        }

        return cardQueue;
    }

    /// <summary>
    /// Shuffles the CardInfo queue stored as a class variable.
    /// </summary>
    /// <remarks>
    /// This method shuffles the CardInfo queue in place by converting it to a list, shuffling the list using the Fisher-Yates shuffle algorithm, and then re-enqueuing the shuffled cards back into the original queue.
    /// </remarks>
    /// <param name="player">The player's deck to shuffle.</param>
    public void ShuffleCardQueue(int player)
    {
        Queue<CardInfo> cardQueue = player == 1 ? _cardQueue_Player1 : _cardQueue_Player2;

        if (cardQueue == default)
        {
            cardQueue = CreateCardQueue(SelectAIDeckFromFolder().Item1);
        }

        List<CardInfo> cardList = cardQueue.ToList();
        System.Random random = new System.Random();

        for (int i = cardList.Count - 1; i > 0; i--)
        {
            int randomIndex = random.Next(0, i + 1);
            CardInfo temp = cardList[i];
            cardList[i] = cardList[randomIndex];
            cardList[randomIndex] = temp;
        }

        cardQueue.Clear();

        foreach (CardInfo card in cardList)
        {
            cardQueue.Enqueue(card);
        }
    }

    public Queue<CardInfo> GetCardQueue(int player)
    {
        return player == 1 ? _cardQueue_Player1 : _cardQueue_Player2;
    }

    private (Dictionary<CardInfo, int>, string) SelectAIDeckFromFolder()
    {
        List<(Dictionary<CardInfo, int>, string)> deckList = SaveDeckSystem.DecksInFolder("AIDecks");

        if (deckList.Count == 0)
        {
            Debug.LogError("No decks found in 'AIDecks' folder.");
            return default;
        }

        System.Random random = new System.Random();
        int randomIndex = random.Next(deckList.Count);

        return deckList[randomIndex];
    }
}
