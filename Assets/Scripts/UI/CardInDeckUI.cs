using TMPro;
using UnityEngine;

public class CardInDeckUI : MonoBehaviour
{
    public CardInfo CardInfo { get; set; }

    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _amountText;

    public void UpdateText(string name, int amount)
    {
        _nameText.text = name;
        _amountText.text = amount.ToString();
    }

    public void RemoveCardFromDeck()
    {
        CardCollectionManager.Instance.RemoveCardFromDeck(CardInfo);
    }
}
