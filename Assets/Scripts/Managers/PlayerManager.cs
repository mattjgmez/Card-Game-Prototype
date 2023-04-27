using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoSingleton<PlayerManager>
{
    [SerializeField] private int _maxSupply;

    private int _supply_Player1;
    private int _supply_Player2;
    private TMP_Text _supplyText_Player1;
    private TMP_Text _supplyText_Player2;

    protected override void Init()
    {
        base.Init();

        _supply_Player1 = _maxSupply;
        _supply_Player2 = _maxSupply;
        _supplyText_Player1 = GameObject.Find("UI_Supply1_Text").GetComponent<TMP_Text>();
        _supplyText_Player2 = GameObject.Find("UI_Supply2_Text").GetComponent<TMP_Text>();

        UpdateSupplyText(1);
        UpdateSupplyText(2);
    }

    public void UpdateSupplyText(int player)
    {
        TMP_Text text = player == 1 ? _supplyText_Player1 : _supplyText_Player2;
        int supply = player == 1 ? _supply_Player1 : _supply_Player2;

        text.text = supply.ToString();
    }

    public bool CanPlayCard(int cost, int player)
    {
        int supply = player == 1 ? _supply_Player1 : _supply_Player2;

        return supply >= cost;
    }

    public void LowerSupply(int amount, int player)
    {
        if (player == 1)
        {
            _supply_Player1 -= amount;
        }
        else
        {
            _supply_Player2 -= amount;
        }
    }

    public int GetCurrentSupply(int player)
    {
        return player == 1 ? _supply_Player1 : _supply_Player2;
    }
}
