using System;
using UnityEngine;

[Serializable]
public class SerializableSprite
{
    public string textureName;
    public SerializableVector2 textureSize;
    public SerializableVector2 pivot;
    public SpriteMeshType meshType = SpriteMeshType.FullRect;
    public SerializableVector4 border;
    public float pixelsPerUnit;

    public SerializableSprite(Sprite sprite)
    {
        if (sprite != null)
        {
            textureName = sprite.texture.name;
            textureSize = new SerializableVector2(sprite.texture.width, sprite.texture.height);
            pivot = new SerializableVector2(sprite.pivot);
            border = new SerializableVector4(sprite.border);
            pixelsPerUnit = sprite.pixelsPerUnit;
        }
        else
        {
            Debug.LogWarning($"The sprite was null on Serialization.");
            textureName = "";
            textureSize = new SerializableVector2(0, 0);
            pivot = new SerializableVector2(0, 0);
            border = new SerializableVector4(0, 0, 0, 0);
            pixelsPerUnit = 100;
        }
    }

    public Sprite ToSprite()
    {
        if (!string.IsNullOrEmpty(textureName))
        {
            Texture2D texture = Resources.Load<Texture2D>(textureName);
            Rect rect = new Rect(0, 0, textureSize.x, textureSize.y);
            Sprite sprite = Sprite.Create(texture, rect, pivot.ToVector2(), pixelsPerUnit, 0, meshType, border.ToVector4());
            return sprite;
        }
        else
        {
            return null;
        }
    }
}
