using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoSingleton<UIManager>
{
    /// <summary>
    /// Updates given TMP Text object to given string.
    /// </summary>
    /// <param name="text">Text object to update.</param>
    /// <param name="value">String to update to.</param>
    public void UpdateUIText(TMP_Text text, string value)
    {
        text.text = value;
    }
}
