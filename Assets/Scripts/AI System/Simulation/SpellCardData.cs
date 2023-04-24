using System.Text;
using UnityEngine;

public class SpellCardData : CardData
{
    public SpellInfo SpellInfo { get; set; }
    public int Power { get; set; }
    public Vector2Int AreaOfEffect { get; set; }

    public SpellCardData() : base() { }

    public SpellCardData(SpellInfo spellInfo)
    {
        CardInfo = spellInfo;
        SpellInfo = spellInfo;
        Name = spellInfo.Name;
        Power = spellInfo.Power;
        AreaOfEffect = spellInfo.AreaOfEffect;
    }

    public static SpellCardData FromSpellCard(SpellCard spellCard)
    {
        Debug.Log($"FromSpellCard: spellCard.CardInfo = {spellCard.CardInfo}");

        SpellInfo spellInfo = spellCard.SpellInfo;
        string name = spellCard.GetName;
        int power = spellCard.Power;
        Vector2Int areaOfEffect = spellCard.AreaOfEffect;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("SpellCardData:");
        sb.AppendLine($"Name: {name}");
        sb.AppendLine($"Info: {spellInfo}");
        sb.AppendLine($"Power: {power}");
        sb.AppendLine($"Area of Effect: {areaOfEffect}");

        Debug.Log(sb.ToString());

        SpellCardData newSpellCardData = new SpellCardData()
        {
            CardInfo = spellInfo,
            SpellInfo = spellInfo,
            Name = name,
            Power = power,
            AreaOfEffect = areaOfEffect,
        };

        return newSpellCardData;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        SpellCardData other = (SpellCardData)obj;

        // Compare the properties of both SpellCardData objects to check if they are equal
        return Name == other.Name && CardInfo == other.CardInfo;
    }

    public override int GetHashCode()
    {
        int hash = 17; // Prime number to initialize the hash code

        hash = hash * 23 + (Name != null ? Name.GetHashCode() : 0); // 23 is another prime number

        return hash;
    }
}