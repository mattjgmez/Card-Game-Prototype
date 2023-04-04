using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

/// <summary>
/// The SaveDeckSystem class is a utility class for managing card decks in Unity. 
/// It provides static methods for saving and loading decks as binary files in a "CustomDecks" directory within the Application.persistentDataPath. 
/// The class is responsible for converting between a Dictionary of CardInfo objects and counts to a List of CardInfoTokens for file serialization and deserialization. 
/// This enables the efficient storage, retrieval, and modification of card decks within the application.
/// </summary>
public static class SaveDeckSystem
{
    /// <summary>
    /// Generates a file path for the specified deck file name within a "CustomDecks" directory. If the "CustomDecks" directory does not exist, it creates the directory. The method combines the Application.persistentDataPath with the "CustomDecks" directory and the deck file name to return the complete file path.
    /// </summary>
    /// <param name="fileName">The name of the deck file, without an extension.</param>
    /// <returns>The complete file path for the specified deck file name within the "CustomDecks" directory.</returns>
    public static string DeckFilePath(string fileName)
    {
        string customDecksPath = Application.persistentDataPath + "/CustomDecks";

        if (!Directory.Exists(customDecksPath))
        {
            Directory.CreateDirectory(customDecksPath);
        }

        return Path.Combine(customDecksPath, $"{fileName}.deck");
    }

    /// <summary>
    /// Saves a deck dictionary to a binary file using the specified deckName. The method first converts the dictionary into a List of CardInfoTokens and then saves the list to a file with the deckName as its filename.
    /// </summary>
    /// <param name="deck">The deck dictionary to save.</param>
    /// <param name="deckName">The name of the deck to be used as the filename.</param>
    public static void SaveDeckToFile(Dictionary<CardInfo, int> deck, string deckName)
    {
        // Convert the "deck" Dictionary to a List of CardInfoTokens
        List<CardInfoToken> deckList = new List<CardInfoToken>();

        foreach (KeyValuePair<CardInfo, int> entry in deck)
        {
            for (int i = 0; i < entry.Value; i++)
            {
                deckList.Add(new CardInfoToken(entry.Key));
            }
        }

        // Save the List to a file with the deckName string as its filename
        string filePath = DeckFilePath(deckName);

        using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fileStream, deckList);
        }

        DebugDeck(deck, deckName, $"{deckName} saved to file.");
    }

    /// <summary>
    /// Loads a deck from a binary file using the specified deckName, converts the saved CardInfoToken List into a Dictionary of CardInfo objects and their counts, and returns the resulting deck Dictionary.
    /// </summary>
    /// <param name="deckName">The name of the deck to load.</param>
    /// <returns>A Dictionary of CardInfo objects and their counts representing the loaded deck.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the specified deck file is not found.</exception>
    public static Dictionary<CardInfo, int> LoadDeckFromFile(string deckName)
    {
        // Load the deck List file using the deckName string.
        string filePath = DeckFilePath(deckName);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Deck file not found: {filePath}");
        }

        List<CardInfoToken> deckList;

        using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            deckList = (List<CardInfoToken>)formatter.Deserialize(fileStream);
        }

        // Convert the loaded deck List into a Dictionary.
        Dictionary<CardInfo, int> deck = new Dictionary<CardInfo, int>();

        foreach (CardInfoToken token in deckList)
        {
            CardInfo cardInfo = token.ToCardInfo();

            if (deck.ContainsKey(cardInfo))
            {
                deck[cardInfo]++;
            }
            else
            {
                deck.Add(cardInfo, 1);
            }
        }

        DebugDeck(deck, deckName, $"{deckName} loaded from file.");
        return deck;
    }

    public static void DebugDeck(Dictionary<CardInfo, int> deck, string deckName, string consoleMessage)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(consoleMessage);
        sb.AppendLine($"Deck located at: {DeckFilePath(deckName)}");

        foreach (KeyValuePair<CardInfo, int> entry in deck)
        {
            sb.AppendLine($"Card: {entry.Key.Name}, Amount: {entry.Value}");
        }

        Debug.Log(sb.ToString());
    }
}
