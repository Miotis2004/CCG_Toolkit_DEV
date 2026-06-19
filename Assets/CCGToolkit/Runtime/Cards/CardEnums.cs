namespace CCGToolkit.Cards
{
    public enum CardType { Minion, Spell, Weapon, Hero, Token, Generated }
    public enum CardRarity { Free, Common, Rare, Epic, Legendary }
    public enum CardFaction { Neutral, Radiant, Umbral, Verdant, Arcane, Mechanical }
    public enum CardSet { Core, Starter, Prototype, Tutorial }
    public enum CardKeyword { Taunt, Charge, Rush, Lifesteal, Shield, SpellDamage, Generated, Token }
    public enum CardZone { Deck, Hand, Board, Graveyard, Banished, Secret, Generated }
    public enum TargetType { None, Any, Friendly, Enemy, FriendlyMinion, EnemyMinion, AllMinions, Hero }
}
