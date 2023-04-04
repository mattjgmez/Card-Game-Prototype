using System;
using UnityEngine;

[Serializable]
public struct SerializableVector4
{
    public float x;
    public float y;
    public float z;
    public float w;

    public SerializableVector4(float x, float y, float z, float w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    public SerializableVector4(Vector4 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
        w = vector.w;
    }

    public Vector4 ToVector4()
    {
        return new Vector4(x, y, z, w);
    }
}
