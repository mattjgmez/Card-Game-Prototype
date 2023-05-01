using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "ScriptableObjects/Unit"), Serializable]
public class UnitInfo : CardInfo
{
    [SerializeField] private int _health;
    [SerializeField] private int _power;
    [SerializeField] private int _cost;
    [SerializeField] private string[] _keywords;
    [SerializeField] private List<ActionInfo> _actions;
    [SerializeField] private List<CardTribe> _tribes;
    [SerializeField] private Sprite _sprite;

    public int Health
    {
        get { return _health; }
        set { _health = value; }
    }

    public int Power
    {
        get { return _power; }
        set { _power = value; }
    }

    public int Cost
    {
        get { return _cost; }
        set { _cost = value; }
    }

    public string[] Keywords
    {
        get { return _keywords; }
        set { _keywords = value; }
    }

    public List<ActionInfo> Actions
    {
        get { return _actions; }
        set { _actions = value; }
    }

    public List<CardTribe> Tribes
    {
        get { return _tribes; }
        set { _tribes = value; }
    }

    public Sprite Sprite
    {
        get { return _sprite; }
        set { _sprite = value; }
    }
}
