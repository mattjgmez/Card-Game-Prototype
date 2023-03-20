using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandManager : MonoSingleton<HandManager>
{
    [SerializeField] private List<GameObject> _cardsInHand = new List<GameObject>(); // The list of cards to center
    [SerializeField] private Vector3 _handPosition;// The point to center the cards around
    [SerializeField] private float _spacing_X, _spacing_Y; // The spacing between each card

    private void Start()
    {
        CenterCardsInHand();
    }

    /// <summary>
    /// Centers the cards around the specified point, with the specified spacing.
    /// </summary>
    public void CenterCardsInHand()
    {
        if (_cardsInHand == null || _cardsInHand.Count == 0)
        {
            Debug.LogWarning("No cards provided to center.");
            return;
        }

        // Calculate the total width of all cards with their spacing
        float totalWidth = (_cardsInHand.Count - 1) * _spacing_X;

        // Calculate the starting position of the first card
        Vector3 startPosition = _handPosition - new Vector3(totalWidth / 2, 0, 0);

        // Set the position for each card
        for (int i = 0; i < _cardsInHand.Count; i++)
        {
            Vector3 cardPosition = startPosition + new Vector3(i * _spacing_X, i * _spacing_Y, 0);
            _cardsInHand[i].transform.position = cardPosition;
        }
    }

    public void RemoveCardFromHand(GameObject cardToRemove)
    {
        if (_cardsInHand.Contains(cardToRemove))
        {
            _cardsInHand.Remove(cardToRemove);
        }
        else
        {
            Debug.LogWarning("Card not found in hand.");
        }
    }
}
