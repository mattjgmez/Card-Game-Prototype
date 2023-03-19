using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private Vector3 _offset;

    private void Update()
    {
        if (IsSelected) transform.position = GameCursor.WorldPosition + _offset;
        else transform.position = new(0, 30, 0);
    }

    public bool IsSelected { get; set; }
}
