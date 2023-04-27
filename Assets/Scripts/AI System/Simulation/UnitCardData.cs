using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static ActionSystem_Simulated;

public class UnitCardData : CardData
{
    public int Power { get; set; }
    public int Health { get; set; }
    public int Cost { get; set; }
    public int MaxHealth { get; set; }
    public List<ActionInfo> Actions { get; set; }
    public int NextAction { get; set; }
    public bool IsProvoked { get; set; }
    public UnitCardData ProvokingCard { get; set; }
    public TileData CurrentTile { get; set; }

    public UnitCardData() : base() { }

    public UnitCardData(UnitInfo info)
    {
        CardInfo = info;

        Name = info.Name;
        Power = info.Power;
        Health = info.Health;
        Cost = info.Cost;
        MaxHealth = info.Health;
        Actions = info.Actions;
        NextAction = 0;
        IsProvoked = false;
        ProvokingCard = null;
        CurrentTile = null;
    }

    public static UnitCardData FromUnitCard(UnitCard unitCard)
    {
        if (unitCard == null)
        {
            Debug.LogError("UnitCardData.FromUnitCard: unitCard is null.");
            return null;
        }

        // Assign properties to temporary variables
        string name = unitCard.GetName;
        int power = unitCard.Power;
        int health = unitCard.Health;
        int cost = unitCard.Cost;
        int maxHealth = unitCard.MaxHealth;
        List<ActionInfo> actions = unitCard.Actions;
        int nextAction = actions.IndexOf(unitCard.NextAction);
        bool isProvoked = unitCard.IsProvoked;
        TileData currentTile = unitCard.CurrentTile != null ? TileData.FromTile(unitCard.CurrentTile) : null;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("UnitCardData:");
        sb.AppendLine($"Name: {name}");
        sb.AppendLine($"Power: {power}");
        sb.AppendLine($"Health: {health}");
        sb.AppendLine($"Cost: {cost}");
        sb.AppendLine($"MaxHealth: {maxHealth}");
        sb.AppendLine($"Actions: {(actions != null ? string.Join(", ", actions) : "null")}");
        sb.AppendLine($"NextAction: {nextAction}");
        sb.AppendLine($"IsProvoked: {isProvoked}");
        sb.AppendLine($"CurrentTile: {currentTile}");

        Debug.Log(sb.ToString());

        // Construct and return a new UnitCardData object using the temporary variables
        return new UnitCardData()
        {
            Name = name,
            Power = power,
            Health = health,
            Cost = cost,
            MaxHealth = maxHealth,
            Actions = actions,
            NextAction = nextAction,
            IsProvoked = isProvoked,
            CurrentTile = currentTile
        };
    }

    public void TakeDamage(int amount, bool isDeathTouch, GameState gameState)
    {
        Health -= amount;

        if (Health <= 0 || isDeathTouch)
        {
            // Remove the card from the activeCards list
            gameState.RemoveActiveCard(this);

            // Set the relevant TileData to not have an active card
            CurrentTile.ActiveCard = null;
        }
    }

    public void Heal(int amount)
    {
        Health += amount;
        Health = Mathf.Clamp(Health, 0, MaxHealth);
    }

    public void TriggerAction(GameState gameState)
    {
        Debug.Log($"GameState{gameState.ID}: {this}.TriggerAction (start): Card: {Name} CurrentTile: {CurrentTile.GridPosition}");

        ActionInfo action = Actions[NextAction];
        List<UnitCardData> targetCards = TargetCards(this, action, gameState); // This is the only place TargetCards is called.

        if (targetCards.Count <= 0)
        {
            return;
        }

        PerformAction(this, action, targetCards, gameState);

        NextAction++;
        if (NextAction >= Actions.Count)
        {
            NextAction = 0;
        }
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        UnitCardData other = (UnitCardData)obj;

        // Compare the properties of both UnitCardData objects to check if they are equal
        return Name == other.Name && CardInfo == other.CardInfo;
    }

    public override int GetHashCode()
    {
        int hash = 17; // Prime number to initialize the hash code

        hash = hash * 23 + (Name != null ? Name.GetHashCode() : 0); // 23 is another prime number

        return hash;
    }
}
