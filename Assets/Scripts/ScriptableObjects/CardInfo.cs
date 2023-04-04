using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "ScriptableObjects/Unit"), Serializable]
public class CardInfo : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private int _health;
    [SerializeField] private int _power;
    [SerializeField] private int _energy;
    [SerializeField] private int _cost;
    [SerializeField] private string[] _keywords;
    [SerializeField] private List<ActionInfo> _actions;
    [SerializeField] private List<CardTribe> _tribes;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private AnimatorController _animController;

    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }

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

    public int Energy
    {
        get { return _energy; }
        set { _energy = value; }
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

    public AnimatorController AnimController
    {
        get { return _animController; }
        set { _animController = value; }
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        CardInfo other = (CardInfo)obj;
        // Compare the properties of both cardInfo objects to determine equality
        return Name == other.Name;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + Name.GetHashCode();
            return hash;
        }
    }
}
