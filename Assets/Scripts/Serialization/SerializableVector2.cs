using System;
using UnityEngine;

[Serializable]
public struct SerializableVector2
{
    public float x;
    public float y;

    public SerializableVector2(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public SerializableVector2(Vector2 vector)
    {
        x = vector.x;
        y = vector.y;
    }

    public Vector2 ToVector2()
    {
        return new Vector2(x, y);
    }
}
