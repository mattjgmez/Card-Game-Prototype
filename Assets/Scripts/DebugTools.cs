using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class DebugTools
{
    public static string ListToString<T>(List<T> list)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine();
        sb.Append("[");
        for (int i = 0; i < list.Count; i++)
        {
            sb.Append(list[i].ToString());
            if (i < list.Count - 1)
            {
                sb.Append(", ");
                sb.AppendLine();
            }
        }
        sb.Append("]");

        return sb.ToString();
    }

    public static string ListToString(List<CardData> list)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine();
        sb.Append("[");

        for (int i = 0; i < list.Count; i++)
        {
            CardData card = list[i];
            if (card.Name == null || card.Name.Length <= 0)
            {
                Debug.LogWarning($"ListToString: Card Name missing for Card Type: {card.GetType()}.");
            }

            sb.Append(card.Name);

            if (card is UnitCardData)
            {
                if ((card as UnitCardData).CurrentTile != null)
                {
                    sb.Append($"{(card as UnitCardData).CurrentTile.GridPosition}");
                }
            }

            if (i < list.Count - 1)
            {
                sb.Append(", ");
                sb.AppendLine();
            }
        }
        sb.Append("]");

        return sb.ToString();
    }

    public static string ListToString(List<UnitCardData> list)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine();
        sb.Append("[");

        for (int i = 0; i < list.Count; i++)
        {
            UnitCardData card = list[i];
            if (card.Name == null || card.Name.Length <= 0)
            {
                Debug.LogWarning($"ListToString: Card Name missing for Card Type: {card.GetType()}.");
            }

            sb.Append(card.Name);

            if (card.CurrentTile != null)
            {
                sb.Append($": {card.CurrentTile.GridPosition}");
            }

            if (i < list.Count - 1)
            {
                sb.Append(", ");
                sb.AppendLine();
            }
        }
        sb.Append("]");

        return sb.ToString();
    }

    public static string ListToString(List<IGameAction> list)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine();
        sb.Append("[");

        for (int i = 0; i < list.Count; i++)
        {
            IGameAction action = list[i];
            sb.Append(ActionToString(action));

            if (i < list.Count - 1)
            {
                sb.Append(", ");
                sb.AppendLine();
            }
        }
        sb.Append("]");

        return sb.ToString();
    }

    private static string ActionToString(IGameAction action)
    {
        StringBuilder sb = new StringBuilder();

        if (action is PlayUnitCardAction)
        {
            PlayUnitCardAction unitAction = action as PlayUnitCardAction;
            sb.Append(unitAction.ToString());
        }
        if (action is PlaySpellCardAction)
        {
            PlaySpellCardAction spellAction = action as PlaySpellCardAction;
            sb.Append(spellAction.ToString());
        }
        else
        {
            sb.Append(action);
        }

        return sb.ToString();
    }
}
