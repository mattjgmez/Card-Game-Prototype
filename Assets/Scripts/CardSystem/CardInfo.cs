using System;
using UnityEditor;
using UnityEngine;

public class CardInfo : ScriptableObject
{
    [SerializeField] protected string _name;

    public void Init()
    {
        string assetPath = AssetDatabase.GetAssetPath(this);
        string fileName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
        _name = fileName;
    }

    public string Name
    {
        get { return _name; }
        set { _name = value; }
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
