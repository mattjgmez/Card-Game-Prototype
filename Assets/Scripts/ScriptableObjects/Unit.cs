using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "ScriptableObjects/Unit")]
public class Unit : ScriptableObject
{
    [SerializeField] string _name;
    [SerializeField] int _health, _attack, _energy;
    [SerializeField] int _cost;
    [SerializeField] string[] _keywords;
    [SerializeField] List<ActionInfo> _actions;
    [SerializeField] Sprite _sprite;
    [SerializeField] AnimatorController _animController;
    [SerializeField] Realm _realm;

    public string GetName { get { return _name; } }
    public int GetHealth { get { return _health; } }
    public int GetAttack { get { return _attack; } }
    public int GetEnergy { get { return _energy; } }
    public int GetCost { get { return _cost; } }
    public string[] GetKeywords { get { return _keywords; } }
    public List<ActionInfo> GetActions { get { return _actions; } }
    public Sprite GetSprite { get { return _sprite; } }
    public AnimatorController GetAnimController { get { return _animController; } }
}
