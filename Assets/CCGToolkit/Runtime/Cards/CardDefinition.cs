using System.Collections.Generic;
using UnityEngine;

namespace CCGToolkit.Cards
{
    [CreateAssetMenu(menuName = "CCG Toolkit/Cards/Card Definition", fileName = "New Card Definition")]
    public sealed class CardDefinition : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private CardIdentifier cardId;
        [SerializeField] private string title;
        [SerializeField, TextArea(2, 6)] private string description;

        [Header("Classification")]
        [SerializeField] private int cost;
        [SerializeField] private CardType cardType;
        [SerializeField] private CardRarity rarity;
        [SerializeField] private CardFaction faction;
        [SerializeField] private CardSet set;
        [SerializeField] private TargetType targetType;
        [SerializeField] private List<CardKeyword> keywords = new List<CardKeyword>();
        [SerializeField] private List<string> tags = new List<string>();

        [Header("Presentation")]
        [SerializeField] private CardPresentationAsset art;
        [SerializeField] private CardPresentationAsset frame;
        [SerializeField] private string artistCredit;
        [SerializeField] private string flavorText;

        [Header("Optional Stats")]
        [SerializeField] private CardStatBlock stats = new CardStatBlock();

        public CardIdentifier CardId { get => cardId; set => cardId = value; }
        public string Title { get => title; set => title = value; }
        public string Description { get => description; set => description = value; }
        public int Cost { get => cost; set => cost = value; }
        public CardType CardType { get => cardType; set => cardType = value; }
        public CardRarity Rarity { get => rarity; set => rarity = value; }
        public CardFaction Faction { get => faction; set => faction = value; }
        public CardSet Set { get => set; set => this.set = value; }
        public TargetType TargetType { get => targetType; set => targetType = value; }
        public IReadOnlyList<CardKeyword> Keywords => keywords;
        public IReadOnlyList<string> Tags => tags;
        public CardPresentationAsset Art { get => art; set => art = value; }
        public CardPresentationAsset Frame { get => frame; set => frame = value; }
        public string ArtistCredit { get => artistCredit; set => artistCredit = value; }
        public string FlavorText { get => flavorText; set => flavorText = value; }
        public CardStatBlock Stats => stats;


        public void AddKeyword(CardKeyword keyword)
        {
            if (!keywords.Contains(keyword)) keywords.Add(keyword);
        }

        public void RemoveKeyword(CardKeyword keyword)
        {
            keywords.Remove(keyword);
        }

        public void ReplaceKeywords(IEnumerable<CardKeyword> newKeywords)
        {
            keywords.Clear();
            if (newKeywords == null) return;
            foreach (var keyword in newKeywords) AddKeyword(keyword);
        }

        public void ReplaceTags(IEnumerable<string> newTags)
        {
            tags.Clear();
            if (newTags == null) return;
            foreach (string tag in newTags)
            {
                if (!string.IsNullOrWhiteSpace(tag)) tags.Add(tag.Trim());
            }
        }

        public CardValidationReport Validate(bool requireArt = true)
        {
            var report = new CardValidationReport();
            if (cardId.IsEmpty) report.Add(CardValidationSeverity.Error, "CARD_ID_MISSING", "Card ID is required.");
            if (string.IsNullOrWhiteSpace(title)) report.Add(CardValidationSeverity.Error, "TITLE_MISSING", "Card title is required.");
            if (cost < 0) report.Add(CardValidationSeverity.Error, "COST_NEGATIVE", "Card cost cannot be negative.");
            if (requireArt && art == null) report.Add(CardValidationSeverity.Warning, "ART_MISSING", "Card art has not been assigned.");

            if ((cardType == CardType.Minion || cardType == CardType.Token) && (!stats.Enabled || stats.Health <= 0))
                report.Add(CardValidationSeverity.Error, "MINION_HEALTH_REQUIRED", "Minion and token cards require enabled stats with positive health.");

            if (cardType == CardType.Spell && stats.Enabled && (stats.Attack != 0 || stats.Health != 0 || stats.Durability != 0))
                report.Add(CardValidationSeverity.Warning, "SPELL_STATS_UNUSUAL", "Spell cards usually should not define attack, health, or durability.");

            if (cardType == CardType.Weapon && (!stats.Enabled || stats.Attack <= 0 || stats.Durability <= 0))
                report.Add(CardValidationSeverity.Error, "WEAPON_STATS_REQUIRED", "Weapon cards require positive attack and durability.");

            return report;
        }

        private void OnValidate()
        {
            if (cardId.IsEmpty) cardId = CardIdentifier.NewGuid();
            if (cost < 0) cost = 0;
        }
    }
}
