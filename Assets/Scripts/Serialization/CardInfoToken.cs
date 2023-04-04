using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor.Animations;

[Serializable]
public struct CardInfoToken
{
    public string Name;
    public int Health;
    public int Power;
    public int Energy;
    public int Cost;
    public string[] Keywords;
    public List<ActionInfoToken> Actions;
    public SerializableSprite Sprite;
    public string AnimController;

    public CardInfoToken(CardInfo cardInfo) : this()
    {
        Name = cardInfo.Name;
        Health = cardInfo.Health;
        Power = cardInfo.Power;
        Energy = cardInfo.Energy;
        Cost = cardInfo.Cost;
        Keywords = cardInfo.Keywords;
        Actions = ConvertActionsToTokens(cardInfo.Actions);
        Sprite = new SerializableSprite(cardInfo.Sprite);
        AnimController = cardInfo.AnimController.name;
    }

    public CardInfo ToCardInfo()
    {
        CardInfo cardInfo = ScriptableObject.CreateInstance<CardInfo>();
        cardInfo.Name = Name;
        cardInfo.Health = Health;
        cardInfo.Power = Power;
        cardInfo.Energy = Energy;
        cardInfo.Cost = Cost;
        cardInfo.Keywords = Keywords;
        cardInfo.Actions = ConvertTokensToActions(Actions);
        cardInfo.Sprite = Sprite.ToSprite();
        cardInfo.AnimController = GetAnimatorController();

        return cardInfo;
    }

    private List<ActionInfoToken> ConvertActionsToTokens(List<ActionInfo> actions)
    {
        List<ActionInfoToken> actionTokens = new List<ActionInfoToken>();
        foreach (ActionInfo action in actions)
        {
            actionTokens.Add(new ActionInfoToken(action));
        }
        return actionTokens;
    }

    private List<ActionInfo> ConvertTokensToActions(List<ActionInfoToken> actionTokens)
    {
        List<ActionInfo> actions = new List<ActionInfo>();
        foreach (ActionInfoToken actionToken in actionTokens)
        {
            ActionInfo actionInfo = ScriptableObject.CreateInstance<ActionInfo>();
            actionToken.CopyToActionInfo(actionInfo);
            actions.Add(actionInfo);
        }
        return actions;
    }

    private AnimatorController GetAnimatorController()
    {
        AnimatorController loadedAnimatorController = Resources.Load(AnimController) as AnimatorController;

        if (loadedAnimatorController != null)
        {
            return loadedAnimatorController;
        }
        else
        {
            Debug.LogError("Failed to load Animator Controller from Resources folder.");
            return null;
        }
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        CardInfoToken other = (CardInfoToken)obj;
        // Compare the properties of both tokens to determine equality
        return Name == other.Name;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + Name.GetHashCode();
            return hash;
        }
    }
}
