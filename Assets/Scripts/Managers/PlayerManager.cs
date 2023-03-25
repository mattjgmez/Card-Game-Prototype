using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoSingleton<PlayerManager>
{
    [SerializeField] private int _maxSupply;

    private int _currentSupply;
    private TMP_Text _supplyText_UI;

    protected override void Init()
    {
        base.Init();

        _currentSupply = _maxSupply;
        _supplyText_UI = GameObject.Find("UI_Supply_Text").GetComponent<TMP_Text>();

        UpdateSupplyText();
    }

    public void UpdateSupplyText()
    {
        UIManager.Instance.UpdateUIText(_supplyText_UI, _currentSupply.ToString());
    }

    public bool CanPlayCard(int cost)
    {
        return _currentSupply >= cost;
    }

    public void LowerSupply(int amount)
    {
        _currentSupply -= amount;
    }

    public int GetCurrentSupply { get { return _currentSupply; } }
}
