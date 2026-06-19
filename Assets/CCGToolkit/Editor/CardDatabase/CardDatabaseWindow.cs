using System.Linq;
using CCGToolkit.Cards;
using UnityEditor;
using UnityEngine;

namespace CCGToolkit.Editor.CardDatabase
{
    public sealed class CardDatabaseWindow : EditorWindow
    {
        private readonly CardDatabaseService database = new CardDatabaseService();
        private Vector2 listScroll, reportScroll;
        private CardSearchQuery query;
        private CardDefinition selected;
        private CardBulkEditOptions bulkOptions;
        private bool hasType, hasRarity, hasFaction, hasSet, hasKeyword;

        [MenuItem("CCG Toolkit/Card Database")]
        public static void Open() => GetWindow<CardDatabaseWindow>("Card Database");

        private void OnEnable() => database.RebuildIndex();

        private void OnGUI()
        {
            DrawToolbar();
            using (new EditorGUILayout.HorizontalScope())
            {
                DrawFilters();
                DrawList();
                DrawPreviewAndTools();
            }
        }

        private void DrawToolbar()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                if (GUILayout.Button("Refresh", EditorStyles.toolbarButton)) database.RebuildIndex();
                if (GUILayout.Button("Validate", EditorStyles.toolbarButton)) Debug.Log(FormatReport(database.ValidateAll()));
                if (GUILayout.Button("Generate Missing/Duplicate IDs", EditorStyles.toolbarButton)) database.GenerateMissingOrDuplicateIds();
                if (GUILayout.Button("Export CSV", EditorStyles.toolbarButton)) ExportCsv();
                if (GUILayout.Button("Export JSON", EditorStyles.toolbarButton)) ExportJson();
                if (GUILayout.Button("Import CSV", EditorStyles.toolbarButton)) ImportCsv();
                if (GUILayout.Button("Import JSON", EditorStyles.toolbarButton)) ImportJson();
                GUILayout.FlexibleSpace();
                GUILayout.Label($"{database.Entries.Count} cards indexed");
            }
        }

        private void DrawFilters()
        {
            using (new EditorGUILayout.VerticalScope(GUILayout.Width(230)))
            {
                EditorGUILayout.LabelField("Search & Filters", EditorStyles.boldLabel);
                query.SearchText = EditorGUILayout.TextField("Search", query.SearchText);
                DrawEnumFilter("Type", ref hasType, ref query.Type);
                DrawEnumFilter("Rarity", ref hasRarity, ref query.Rarity);
                DrawEnumFilter("Faction", ref hasFaction, ref query.Faction);
                DrawEnumFilter("Set", ref hasSet, ref query.Set);
                DrawEnumFilter("Keyword", ref hasKeyword, ref query.Keyword);
                EditorGUILayout.Space();
                DrawBulkTools();
            }
        }

        private static void DrawEnumFilter<T>(string label, ref bool enabled, ref T? value) where T : struct, System.Enum
        {
            enabled = EditorGUILayout.ToggleLeft($"Filter {label}", enabled);
            using (new EditorGUI.DisabledScope(!enabled)) value = (T)EditorGUILayout.EnumPopup(label, value ?? default);
            if (!enabled) value = null;
        }

        private void DrawList()
        {
            var rows = database.Query(query).ToList();
            using (new EditorGUILayout.VerticalScope(GUILayout.MinWidth(360)))
            {
                EditorGUILayout.LabelField($"Cards ({rows.Count})", EditorStyles.boldLabel);
                listScroll = EditorGUILayout.BeginScrollView(listScroll);
                foreach (var entry in rows)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button(entry.Card.Title, EditorStyles.linkLabel, GUILayout.Width(170))) Selection.activeObject = selected = entry.Card;
                        GUILayout.Label(entry.Card.Cost.ToString(), GUILayout.Width(20));
                        GUILayout.Label(entry.Card.CardType.ToString(), GUILayout.Width(70));
                        GUILayout.Label(entry.Card.Rarity.ToString(), GUILayout.Width(70));
                        if (GUILayout.Button("Open", GUILayout.Width(48))) AssetDatabase.OpenAsset(entry.Card);
                    }
                }
                EditorGUILayout.EndScrollView();
            }
        }

        private void DrawPreviewAndTools()
        {
            using (new EditorGUILayout.VerticalScope(GUILayout.Width(330)))
            {
                EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);
                selected = (CardDefinition)EditorGUILayout.ObjectField(selected, typeof(CardDefinition), false);
                if (selected != null)
                {
                    EditorGUILayout.LabelField(selected.Title, EditorStyles.largeLabel);
                    EditorGUILayout.LabelField($"{selected.Cost} mana • {selected.CardType} • {selected.Rarity}");
                    EditorGUILayout.HelpBox(selected.Description, MessageType.None);
                    EditorGUILayout.ObjectField("Art", selected.Art, typeof(CardPresentationAsset), false);
                    EditorGUILayout.ObjectField("Frame", selected.Frame, typeof(CardPresentationAsset), false);
                    if (selected.Stats.Enabled) EditorGUILayout.LabelField($"ATK {selected.Stats.Attack} / HP {selected.Stats.Health} / DUR {selected.Stats.Durability}");
                    EditorGUILayout.LabelField($"Faction: {selected.Faction}  Set: {selected.Set}");
                    EditorGUILayout.LabelField($"Keywords: {string.Join(", ", selected.Keywords)}");
                    var report = selected.Validate();
                    reportScroll = EditorGUILayout.BeginScrollView(reportScroll, GUILayout.Height(120));
                    foreach (var issue in report.Issues) EditorGUILayout.HelpBox($"{issue.Code}: {issue.Message}", ToMessageType(issue.Severity));
                    EditorGUILayout.EndScrollView();
                }
            }
        }

        private void DrawBulkTools()
        {
            EditorGUILayout.LabelField("Bulk Edit Filtered Cards", EditorStyles.boldLabel);
            bulkOptions.Set = ToggleEnum("Set", bulkOptions.Set);
            bulkOptions.Rarity = ToggleEnum("Rarity", bulkOptions.Rarity);
            bulkOptions.Faction = ToggleEnum("Faction", bulkOptions.Faction);
            bulkOptions.Keyword = ToggleEnum("Add Keyword", bulkOptions.Keyword);
            bulkOptions.CostDelta = EditorGUILayout.IntField("Cost Delta", bulkOptions.CostDelta);
            bulkOptions.AttackDelta = EditorGUILayout.IntField("Attack Delta", bulkOptions.AttackDelta);
            bulkOptions.HealthDelta = EditorGUILayout.IntField("Health Delta", bulkOptions.HealthDelta);
            if (GUILayout.Button("Apply To Filtered")) CardBulkEditUtility.Apply(database.Query(query).Select(e => e.Card), bulkOptions);
        }

        private static T? ToggleEnum<T>(string label, T? current) where T : struct, System.Enum
        {
            bool enabled = EditorGUILayout.ToggleLeft($"Change {label}", current.HasValue);
            if (!enabled) return null;
            return (T)EditorGUILayout.EnumPopup(label, current ?? default);
        }

        private void ExportCsv() { string path = EditorUtility.SaveFilePanel("Export Cards CSV", "", "cards.csv", "csv"); if (!string.IsNullOrEmpty(path)) CardImportExportUtility.ExportCsv(database.Query(query).Select(e => e.Card), path); }
        private void ExportJson() { string path = EditorUtility.SaveFilePanel("Export Cards JSON", "", "cards.json", "json"); if (!string.IsNullOrEmpty(path)) CardImportExportUtility.ExportJson(database.Query(query).Select(e => e.Card), path); }
        private void ImportCsv() { string path = EditorUtility.OpenFilePanel("Import Cards CSV", "", "csv"); if (!string.IsNullOrEmpty(path)) { CardImportExportUtility.ImportCsv(path, "Assets/CCGToolkit/Samples/SampleCards"); database.RebuildIndex(); } }
        private void ImportJson() { string path = EditorUtility.OpenFilePanel("Import Cards JSON", "", "json"); if (!string.IsNullOrEmpty(path)) { CardImportExportUtility.ImportJson(path, "Assets/CCGToolkit/Samples/SampleCards"); database.RebuildIndex(); } }
        private static MessageType ToMessageType(CardValidationSeverity s) => s == CardValidationSeverity.Error ? MessageType.Error : s == CardValidationSeverity.Warning ? MessageType.Warning : MessageType.Info;
        private static string FormatReport(CardValidationReport report) => string.Join("\n", report.Issues.Select(i => $"{i.Severity} {i.Code}: {i.Message}"));
    }
}
