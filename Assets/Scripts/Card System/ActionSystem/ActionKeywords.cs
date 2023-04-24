public enum ActionKeywords
{
    Damage = 1,
    Heal = 2,
    Cleave = 3,         // Hits nearby targets in column
    Burst = 4,          // Hits all targets in the row
    Nova = 5,           // Hits all valid targets
    Drain = 6,          // Heals user
    DrawCard = 7,
    Provoke = 9,        // Forces enemy to target user
    DeathTouch = 11,    // Instantly kills target
    Overkill = 12,      // Overkill damage is dealt to unit behind target
}
