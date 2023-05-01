using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class CardInfo : ScriptableObject
{
    [SerializeField] protected string _name;

    public void Init()
    {
    #if UNITY_EDITOR
        string assetPath = AssetDatabase.GetAssetPath(this);
        string fileName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
        _name = fileName;
    #endif
    }

    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }
}
