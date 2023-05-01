using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private List<Canvas> _menus;

    private enum MenuIndex
    {
        MainMenu = 0,
        DeckSelection = 2,
        Collection = 1
    }

    public void NewGame()
    {
        SetActiveCanvas(MenuIndex.DeckSelection);
    }

    public void OpenCollection()
    {
        SetActiveCanvas(MenuIndex.Collection);
    }

    public void OpenMainMenu()
    {
        SetActiveCanvas(MenuIndex.MainMenu);
    }

    private void SetActiveCanvas(MenuIndex activeMenuIndex)
    {
        for (int i = 0; i < _menus.Count; i++)
        {
            _menus[i].enabled = i == (int)activeMenuIndex;
        }

        GameManager.Instance.CurrentDeck_Player1 = default;
        GameManager.Instance.CurrentDeck_Player2 = default;
    }

    public void CallStartGame()
    {
        GameManager.Instance.StartGame();
    }
}
