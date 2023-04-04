using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ActionInfoToken
{
    public int Cost;
    public string Name;
    public string Description;
    public SerializableSprite Sprite;
    public List<ActionKeywords> Keywords;
    public ActionRange Range;
    public List<bool> ValidTargets;

    public ActionInfoToken(ActionInfo actionInfo)
    {
        Cost = actionInfo.Cost;
        Name = actionInfo.Name;
        Description = actionInfo.Description;
        Sprite = new SerializableSprite(actionInfo.Sprite);
        Keywords = actionInfo.Keywords;
        Range = actionInfo.Range;
        ValidTargets = actionInfo.ValidTargets;
    }

    public void CopyToActionInfo(ActionInfo actionInfo)
    {
        // Copy the properties from the token to the ActionInfo object.
        actionInfo.Cost = Cost;
        actionInfo.Name = Name;
        actionInfo.Description = Description;
        actionInfo.Sprite = Sprite.ToSprite();
        actionInfo.Keywords = Keywords;
        actionInfo.Range = Range;
        actionInfo.ValidTargets = ValidTargets;
    }
}
