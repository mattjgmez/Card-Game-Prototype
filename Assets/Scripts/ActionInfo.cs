using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Action", menuName = "ScriptableObjects/Action")]
public class ActionInfo : ScriptableObject
{
    [SerializeField] int _cost;
    [SerializeField] string _name;
    [SerializeField] string _description;
    [SerializeField] Sprite _sprite;
    [SerializeField] List<ActionKeywords> _keywords;
    [SerializeField] ActionRange _range;
    [SerializeField, Header("Order of bools: Enemies, Allies, Self")] List<bool> _validTargets = new List<bool>
    {
        { false },
        { false },
        { false },
    };

    public bool HasKeyword(ActionKeywords keyword)
    {
        return _keywords.Contains(keyword);
    }

    public int Cost
    {
        get { return _cost; }
        set { _cost = value; }
    }

    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }

    public string Description
    {
        get { return _description; }
        set { _description = value; }
    }

    public Sprite Sprite
    {
        get { return _sprite; }
        set { _sprite = value; }
    }

    public List<ActionKeywords> Keywords
    {
        get { return _keywords; }
        set { _keywords = value; }
    }

    public ActionRange Range
    {
        get { return _range; }
        set { _range = value; }
    }

    public List<bool> ValidTargets
    {
        get { return _validTargets; }
        set { _validTargets = value; }
    }
}
