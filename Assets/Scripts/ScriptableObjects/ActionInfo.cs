using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum ActionKeywords
{
    Damage = 1,
    Heal = 2,
    Cleave = 7,
    Drain = 4,
    DrawCard = 5,
    Momentum = 6,
    Provoke = 8
}

[CreateAssetMenu(fileName = "Action", menuName = "ScriptableObjects/Action")]
public class ActionInfo : ScriptableObject
{
    [SerializeField] int _cost;
    [SerializeField] string _name;
    [SerializeField] string _description;
    [SerializeField] Sprite _sprite;
    [SerializeField] List<ActionKeywords> _keywords;
    [SerializeField] List<Vector2Int> _range;

    public bool HasKeyword(ActionKeywords keyword)
    {
        return _keywords.Contains(keyword);
    }

    public int GetCost { get { return _cost; } }
    public string GetName { get { return _name; } }
    public string GetDescription { get { return _description; } }
    public Sprite GetSprite { get { return _sprite; } }
    public List<ActionKeywords> GetKeywords { get { return _keywords; } }
    public List <Vector2Int> GetRange { get { return _range; } }
}
