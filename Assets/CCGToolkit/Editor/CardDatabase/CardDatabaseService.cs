using System;
using System.Collections.Generic;
using System.Linq;
using CCGToolkit.Cards;
using UnityEditor;

namespace CCGToolkit.Editor.CardDatabase
{
    public sealed class CardDatabaseService
    {
        private readonly List<CardDatabaseEntry> entries = new List<CardDatabaseEntry>();
        public IReadOnlyList<CardDatabaseEntry> Entries => entries;

        public void RebuildIndex(string[] searchFolders = null)
        {
            entries.Clear();
            string[] guids = AssetDatabase.FindAssets("t:CardDefinition", searchFolders ?? new[] { "Assets" });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var card = AssetDatabase.LoadAssetAtPath<CardDefinition>(path);
                if (card != null) entries.Add(new CardDatabaseEntry(card, path, guid));
            }
            entries.Sort((left, right) => string.Compare(left.Card.Title, right.Card.Title, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<CardDatabaseEntry> Query(CardSearchQuery query)
        {
            IEnumerable<CardDatabaseEntry> result = entries;
            if (!string.IsNullOrWhiteSpace(query.SearchText))
            {
                string text = query.SearchText.Trim();
                result = result.Where(entry => Contains(entry.Card.Title, text) || Contains(entry.Card.Description, text) || Contains(entry.Card.CardId.Value, text) || Contains(entry.AssetPath, text));
            }
            if (query.Type.HasValue) result = result.Where(entry => entry.Card.CardType == query.Type.Value);
            if (query.Rarity.HasValue) result = result.Where(entry => entry.Card.Rarity == query.Rarity.Value);
            if (query.Faction.HasValue) result = result.Where(entry => entry.Card.Faction == query.Faction.Value);
            if (query.Set.HasValue) result = result.Where(entry => entry.Card.Set == query.Set.Value);
            if (query.Keyword.HasValue) result = result.Where(entry => entry.Card.Keywords.Contains(query.Keyword.Value));
            return result;
        }

        public CardValidationReport ValidateAll() => CardDefinitionValidator.ValidateCollection(entries.Select(entry => entry.Card));

        public IReadOnlyList<IGrouping<CardIdentifier, CardDatabaseEntry>> FindDuplicateIds() => entries.Where(entry => !entry.Card.CardId.IsEmpty).GroupBy(entry => entry.Card.CardId).Where(group => group.Count() > 1).ToList();

        public void GenerateMissingOrDuplicateIds()
        {
            var seen = new HashSet<CardIdentifier>();
            foreach (var entry in entries)
            {
                if (entry.Card.CardId.IsEmpty || !seen.Add(entry.Card.CardId))
                {
                    Undo.RecordObject(entry.Card, "Generate Card ID");
                    entry.Card.CardId = CardIdentifier.NewGuid();
                    EditorUtility.SetDirty(entry.Card);
                    seen.Add(entry.Card.CardId);
                }
            }
            AssetDatabase.SaveAssets();
        }

        private static bool Contains(string source, string value) => !string.IsNullOrEmpty(source) && source.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    public struct CardSearchQuery
    {
        public string SearchText;
        public CardType? Type;
        public CardRarity? Rarity;
        public CardFaction? Faction;
        public CardSet? Set;
        public CardKeyword? Keyword;
    }
}
