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
    [SerializeField] StringBoolDictionary _validTargets = new StringBoolDictionary
    {
        {"Targets Enemies", false },
        {"Targets Allies", false },
        {"Targets Self", false },
    };

    public bool HasKeyword(ActionKeywords keyword)
    {
        return _keywords.Contains(keyword);
    }

    public int GetCost { get { return _cost; } }
    public string GetName { get { return _name; } }
    public string GetDescription { get { return _description; } }
    public Sprite GetSprite { get { return _sprite; } }
    public List<ActionKeywords> GetKeywords { get { return _keywords; } }
    public ActionRange GetRange { get { return _range; } }
    public StringBoolDictionary GetValidTargets { get { return _validTargets; } }
}

[Serializable]
public class StringBoolDictionary : SerializableDictionary<string, bool> { }