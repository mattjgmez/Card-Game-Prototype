public enum ActionKeywords
{
    Damage = 1,
    Heal = 2,
    Cleave = 3,         // Hits nearby targets in column
    Burst = 4,          // Hits all targets in the row
    Nova = 5,           // Hits all valid targets
    Drain = 6,          // Heals user
    DrawCard = 7,
    Momentum = 8,       // Refunds energy if lethal
    Provoke = 9,        // Forces enemy to target user
    Combo = 10,         // Bonus when targeting the same as the last action
    DeathTouch = 11,    // Instantly kills target
    Overkill = 12,      // Overkill damage is dealt to unit behind target
}
