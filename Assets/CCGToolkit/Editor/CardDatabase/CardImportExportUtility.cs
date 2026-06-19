using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CCGToolkit.Cards;
using UnityEditor;
using UnityEngine;

namespace CCGToolkit.Editor.CardDatabase
{
    public static class CardImportExportUtility
    {
        public static string ToJson(IEnumerable<CardDefinition> cards) => JsonUtility.ToJson(new CardExportCollection(cards.Select(CardExportRecord.FromCard).ToList()), true);

        public static string ToCsv(IEnumerable<CardDefinition> cards)
        {
            var builder = new StringBuilder();
            builder.AppendLine("id,title,description,cost,type,rarity,faction,set,target,attack,health,durability,armor,resource,keywords,tags");
            foreach (var card in cards) builder.AppendLine(CardExportRecord.FromCard(card).ToCsvLine());
            return builder.ToString();
        }

        public static void ExportJson(IEnumerable<CardDefinition> cards, string path) => File.WriteAllText(path, ToJson(cards));
        public static void ExportCsv(IEnumerable<CardDefinition> cards, string path) => File.WriteAllText(path, ToCsv(cards));

        public static IReadOnlyList<CardDefinition> ImportJson(string path, string destinationFolder)
        {
            var collection = JsonUtility.FromJson<CardExportCollection>(File.ReadAllText(path));
            return ImportRecords(collection == null || collection.cards == null ? Enumerable.Empty<CardExportRecord>() : collection.cards, destinationFolder);
        }

        public static IReadOnlyList<CardDefinition> ImportCsv(string path, string destinationFolder)
        {
            var lines = File.ReadAllLines(path).Skip(1).Where(line => !string.IsNullOrWhiteSpace(line));
            return ImportRecords(lines.Select(ParseCsvRecord), destinationFolder);
        }

        private static IReadOnlyList<CardDefinition> ImportRecords(IEnumerable<CardExportRecord> records, string destinationFolder)
        {
            Directory.CreateDirectory(destinationFolder);
            var imported = new List<CardDefinition>();
            foreach (var record in records)
            {
                var card = ScriptableObject.CreateInstance<CardDefinition>();
                Apply(record, card);
                string safeName = MakeSafeAssetName(string.IsNullOrWhiteSpace(record.title) ? "Imported Card" : record.title);
                string path = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(destinationFolder, $"{safeName}.asset").Replace("\\", "/"));
                AssetDatabase.CreateAsset(card, path);
                imported.Add(card);
            }
            AssetDatabase.SaveAssets();
            return imported;
        }

        private static void Apply(CardExportRecord record, CardDefinition card)
        {
            card.CardId = string.IsNullOrWhiteSpace(record.id) ? CardIdentifier.NewGuid() : new CardIdentifier(record.id);
            card.Title = record.title; card.Description = record.description; card.Cost = Math.Max(0, record.cost);
            if (Enum.TryParse(record.type, out CardType type)) card.CardType = type;
            if (Enum.TryParse(record.rarity, out CardRarity rarity)) card.Rarity = rarity;
            if (Enum.TryParse(record.faction, out CardFaction faction)) card.Faction = faction;
            if (Enum.TryParse(record.set, out CardSet set)) card.Set = set;
            if (Enum.TryParse(record.target, out TargetType target)) card.TargetType = target;
            card.Stats.Enabled = record.attack != 0 || record.health != 0 || record.durability != 0 || record.armor != 0 || record.resource != 0;
            card.Stats.Attack = record.attack; card.Stats.Health = record.health; card.Stats.Durability = record.durability; card.Stats.Armor = record.armor; card.Stats.ResourceModifier = record.resource;
            card.ReplaceKeywords(SplitEnums<CardKeyword>(record.keywords));
            card.ReplaceTags((record.tags ?? string.Empty).Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
        }

        private static CardExportRecord ParseCsvRecord(string line)
        {
            var cells = SplitCsv(line).ToArray();
            string Cell(int index) => index < cells.Length ? cells[index] : string.Empty;
            int IntCell(int index) => int.TryParse(Cell(index), out int value) ? value : 0;
            return new CardExportRecord { id = Cell(0), title = Cell(1), description = Cell(2), cost = IntCell(3), type = Cell(4), rarity = Cell(5), faction = Cell(6), set = Cell(7), target = Cell(8), attack = IntCell(9), health = IntCell(10), durability = IntCell(11), armor = IntCell(12), resource = IntCell(13), keywords = Cell(14), tags = Cell(15) };
        }

        private static IEnumerable<string> SplitCsv(string line)
        {
            var cell = new StringBuilder();
            bool quoted = false;
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (quoted && c == '"' && i + 1 < line.Length && line[i + 1] == '"') { cell.Append('"'); i++; }
                else if (c == '"') quoted = !quoted;
                else if (c == ',' && !quoted) { yield return cell.ToString(); cell.Length = 0; }
                else cell.Append(c);
            }
            yield return cell.ToString();
        }

        private static IEnumerable<T> SplitEnums<T>(string value) where T : struct
        {
            foreach (string item in (value ?? string.Empty).Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                if (Enum.TryParse(item, out T parsed)) yield return parsed;
        }

        private static string MakeSafeAssetName(string name)
        {
            foreach (char invalid in Path.GetInvalidFileNameChars()) name = name.Replace(invalid, '_');
            return name;
        }

        [Serializable] private sealed class CardExportCollection { public List<CardExportRecord> cards; public CardExportCollection() { } public CardExportCollection(List<CardExportRecord> cards) { this.cards = cards; } }

        [Serializable]
        private sealed class CardExportRecord
        {
            public string id, title, description, type, rarity, faction, set, target, keywords, tags;
            public int cost, attack, health, durability, armor, resource;
            public static CardExportRecord FromCard(CardDefinition card) => new CardExportRecord
            {
                id = card.CardId.Value, title = card.Title, description = card.Description, cost = card.Cost, type = card.CardType.ToString(), rarity = card.Rarity.ToString(), faction = card.Faction.ToString(), set = card.Set.ToString(), target = card.TargetType.ToString(),
                attack = card.Stats.Attack, health = card.Stats.Health, durability = card.Stats.Durability, armor = card.Stats.Armor, resource = card.Stats.ResourceModifier,
                keywords = string.Join("|", card.Keywords), tags = string.Join("|", card.Tags)
            };
            public string ToCsvLine() => string.Join(",", new[] { Q(id), Q(title), Q(description), cost.ToString(), type, rarity, faction, set, target, attack.ToString(), health.ToString(), durability.ToString(), armor.ToString(), resource.ToString(), Q(keywords), Q(tags) });
            private static string Q(string value) => $"\"{(value ?? string.Empty).Replace("\"", "\"\"")}\"";
        }
    }
}
