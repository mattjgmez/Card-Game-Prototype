using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Spell", menuName = "ScriptableObjects/Spell")]
public class SpellInfo : CardInfo
{
    [SerializeField] private string _description;
    [SerializeField] private int _power;
    [SerializeField] private List<SpellTags> _tags;
    [SerializeField] private List<SpecialSpellTags> _specialTags;
    [SerializeField] private Vector2Int _areaOfEffect;
    [SerializeField, Header("Order of bools: Enemies, Allies")]
    List<bool> _validTargets = new List<bool>
    {
        { true },
        { true },
    };

    public string Description
    {
        get { return _description; }
        set { _description = value; }
    }

    public int Power
    {
        get { return _power; }
        set { _power = value; }
    }

    public bool HasTag(SpellTags tag)
    {
        return _tags.Contains(tag);
    }

    public List<SpecialSpellTags> SpecialTags
    {
        get { return _specialTags; }
        set { _specialTags = value; }
    }

    public Vector2Int AreaOfEffect
    {
        get { return _areaOfEffect; }
        set { _areaOfEffect = value; }
    }

    public List<bool> ValidTargets
    {
        get { return _validTargets; }
        set { _validTargets = value; }
    }
}
