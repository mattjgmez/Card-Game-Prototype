using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Shadow : MonoBehaviour
{
    [SerializeField] SpriteRenderer _spriteRenderer, _parentSpriteRenderer;

    void Update()
    {
        _spriteRenderer.flipX = _parentSpriteRenderer.flipX;
        _spriteRenderer.sprite = _parentSpriteRenderer.sprite;
    }
}
