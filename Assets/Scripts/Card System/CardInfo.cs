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
}
