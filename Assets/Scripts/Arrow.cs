using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    bool _isSelected;

    void Update()
    {
        if (_isSelected)
        {
            transform.position = GameCursor.WorldPosition + new Vector3(0, .5f, 0);
        }
        else transform.position = new Vector3(0, 20, 0);
    }

    public bool IsSelected { get { return _isSelected; } set { _isSelected = value; } }
}
