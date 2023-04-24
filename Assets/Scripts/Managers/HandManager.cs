using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoSingleton<HandManager>
{
    private Dictionary<int, List<GameObject>> _cardsInHand = new Dictionary<int, List<GameObject>>
    {
        { 1, new List<GameObject>() },  // Player 1's hand
        { 2, new List<GameObject>() }   // Player 2's hand
    };
    [SerializeField] private Vector3 _handPosition; // The point to center the cards around
    [SerializeField] private float _spacing_X, _spacing_Y; // The spacing between each card
    [SerializeField] private GameObject _unitCardPrefab;
    [SerializeField] private GameObject _spellCardPrefab;
    [SerializeField] private int _startingHandSize;
    [SerializeField] private int _cardsDrawnPerTurn;

    private void OnEnable()
    {
        TurnManager.Instance.DrawPhase += DrawPhase;
        TurnManager.Instance.StartPhase += DrawStartingCards;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            DrawCards(_startingHandSize, 1, transform);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            DrawCards(_cardsDrawnPerTurn, 1, transform);
        }
    }

    private void OnDisable()
    {
        TurnManager.Instance.DrawPhase -= DrawPhase;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        // Draw a sphere gizmo at the _handPosition
        float gizmoSize = 0.2f;
        Gizmos.DrawSphere(_handPosition, gizmoSize);
    }

    private void DrawStartingCards(PlayerTurn turn)
    {
        if (!TurnManager.Instance.IsFirstRound)
        {
            TurnManager.Instance.StartPhase -= DrawStartingCards;
            return;
        }

        DrawCards(_startingHandSize, 1, transform);
        DrawCards(_startingHandSize, 2, transform);
    }

    private void DrawPhase(PlayerTurn turn)
    {
        int player = turn == PlayerTurn.Player1 ? 1 : 2;
        DrawCards(_cardsDrawnPerTurn, player, transform);
    }

    /// <summary>
    /// Centers the cards around the specified point, with the specified spacing.
    /// </summary>
    public void CenterCardsInHand(int player)
    {
        if (!_cardsInHand.ContainsKey(player))
        {
            Debug.LogWarning($"Invalid player number: {player}");
            return;
        }

        List<GameObject> hand = _cardsInHand[player];

        if (hand == null || hand.Count == 0)
        {
            Debug.LogWarning("No cards provided to center.");
            return;
        }

        // Calculate the total width of all cards with their spacing
        float totalWidth = (hand.Count - 1) * _spacing_X;

        // Calculate the starting position of the first card
        Vector3 startPosition = _handPosition - new Vector3(totalWidth / 2, 0, 0);

        // Moves player 2's cards to an arbitrary position so that they are not visible
        if (player != 1)
        {
            startPosition = new(20, 20, 20);
        }

        // Set the position for each card
        for (int i = 0; i < hand.Count; i++)
        {
            Vector3 cardPosition = startPosition + new Vector3(i * _spacing_X, i * _spacing_Y, 0);
            hand[i].transform.position = cardPosition;
        }
    }

    public void RemoveCardFromHand(GameObject cardToRemove, int player)
    {
        if (!_cardsInHand.ContainsKey(player))
        {
            Debug.LogWarning($"Invalid player number: {player}");
            return;
        }

        List<GameObject> hand = _cardsInHand[player];

        if (hand.Remove(cardToRemove))
        {
            Debug.Log("Card removed successfully.");
        }
        else
        {
            Debug.LogWarning("Card not found in hand.");
        }
    }

    /// <summary>
    /// Draws cards from the GameManager's card queue and adds them to the hand.
    /// </summary>
    /// <param name="count">The number of cards to draw.</param>
    /// <param name="player">The player to draw cards.</param>
    public void DrawCards(int count, int player, Transform handTransform)
    {
        Queue<CardInfo> cardQueue = GameManager.Instance.GetCardQueue(player);

        if (cardQueue == null)
        {
            Debug.LogError($"cardQueue is null for Player {player}.");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            if (cardQueue.Count > 0)
            {
                CardInfo drawnCard = cardQueue.Dequeue();
                Card card = InstantiateCard(drawnCard, handTransform).GetComponent<Card>();
                _cardsInHand[player].Add(card.gameObject);
                card.IsPlayer1 = player == 1;
            }
            else
            {
                Debug.LogWarning($"No more cards left in player {player}'s deck.");
                break;
            }
        }
        CenterCardsInHand(player);
    }

    public GameObject InstantiateCard(CardInfo cardInfo, Transform parent)
    {
        if (cardInfo == null)
        {
            Debug.LogError("HandManager.InstantiateCard: CardInfo is null.");
        }
        if (parent == null)
        {
            Debug.LogError("HandManager.InstantiateCard: Parent is null.");
        }

        GameObject cardToSpawn;

        if (cardInfo is UnitInfo)
        {
            cardToSpawn = Instantiate(_unitCardPrefab, parent);
        }
        else if (cardInfo is SpellInfo)
        {
            cardToSpawn = Instantiate(_spellCardPrefab, parent);
        }
        else
        {
            Debug.LogError($"Invalid CardInfo type of {cardInfo.GetType()}.");
            return null;
        }

        Card cardComponent = cardToSpawn.GetComponent<Card>();
        if (cardComponent != null)
        {
            cardComponent.CardInfo = cardInfo;
        }
        else
        {
            Debug.LogError("Card component not found on instantiated card.");
            return null;
        }

        return cardToSpawn;
    }

    public List<Card> GetHand(int player)
    {
        if (!_cardsInHand.ContainsKey(player))
        {
            Debug.LogWarning($"Invalid player number: {player}");
            return null;
        }

        List<GameObject> hand = _cardsInHand[player];
        List<Card> cardsInHand = new List<Card>();

        foreach (GameObject gameObject in hand)
        {
            cardsInHand.Add(gameObject.GetComponent<Card>());
        }

        return cardsInHand;
    }
}