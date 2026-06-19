using System.Collections.Generic;
using CCGToolkit.Cards;
using UnityEditor;

namespace CCGToolkit.Editor.CardDatabase
{
    public static class CardBulkEditUtility
    {
        public static void Apply(IEnumerable<CardDefinition> cards, CardBulkEditOptions options)
        {
            foreach (var card in cards)
            {
                if (card == null) continue;
                Undo.RecordObject(card, "Bulk Edit Cards");
                if (options.Set.HasValue) card.Set = options.Set.Value;
                if (options.Rarity.HasValue) card.Rarity = options.Rarity.Value;
                if (options.Faction.HasValue) card.Faction = options.Faction.Value;
                if (options.Keyword.HasValue) card.AddKeyword(options.Keyword.Value);
                if (options.CostDelta != 0) card.Cost = System.Math.Max(0, card.Cost + options.CostDelta);
                if (options.AttackDelta != 0) card.Stats.Attack = System.Math.Max(0, card.Stats.Attack + options.AttackDelta);
                if (options.HealthDelta != 0) card.Stats.Health = System.Math.Max(0, card.Stats.Health + options.HealthDelta);
                EditorUtility.SetDirty(card);
            }
            AssetDatabase.SaveAssets();
        }
    }

    public struct CardBulkEditOptions
    {
        public CardSet? Set;
        public CardRarity? Rarity;
        public CardFaction? Faction;
        public CardKeyword? Keyword;
        public int CostDelta;
        public int AttackDelta;
        public int HealthDelta;
    }
}
