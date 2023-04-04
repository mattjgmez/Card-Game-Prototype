using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    private (Dictionary<CardInfo, int>, string) _currentDeck = default;
    private Queue<CardInfo> _cardQueue;

    public Action<(Dictionary<CardInfo, int>, string)> CurrentDeckChanged;

    public (Dictionary<CardInfo, int>, string) CurrentDeck
    {
        get
        {
            return _currentDeck;
        }
        set
        {
            _currentDeck = value;

            CurrentDeckChanged?.Invoke(_currentDeck);

            string deckName = value == default ? "NULL" : value.Item2;
            Debug.Log($"Current Deck set to {deckName}.");

            if (value != default)
            {
                _cardQueue = CreateCardQueue(value.Item1);
            }
        }
    }

    protected override void Init()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void StartGame()
    {
        if (_currentDeck != default)
        {
            ChangeScene("Gameplay");
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
    public void ShuffleCardQueue()
    {
        List<CardInfo> cardList = _cardQueue.ToList();
        System.Random random = new System.Random();

        for (int i = cardList.Count - 1; i > 0; i--)
        {
            int randomIndex = random.Next(0, i + 1);
            CardInfo temp = cardList[i];
            cardList[i] = cardList[randomIndex];
            cardList[randomIndex] = temp;
        }

        _cardQueue.Clear();

        foreach (CardInfo card in cardList)
        {
            _cardQueue.Enqueue(card);
        }
    }

    public Queue<CardInfo> GetCardQueue()
    {
        return _cardQueue;
    }
}
