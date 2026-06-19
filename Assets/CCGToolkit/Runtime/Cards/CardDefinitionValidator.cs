using System.Collections.Generic;

namespace CCGToolkit.Cards
{
    public static class CardDefinitionValidator
    {
        public static CardValidationReport ValidateCollection(IEnumerable<CardDefinition> cards)
        {
            var report = new CardValidationReport();
            var names = new HashSet<string>();
            var ids = new HashSet<CardIdentifier>();

            foreach (var card in cards)
            {
                if (card == null)
                {
                    report.Add(CardValidationSeverity.Error, "CARD_NULL", "Card collection contains a null entry.");
                    continue;
                }

                foreach (var issue in card.Validate().Issues)
                    report.Add(issue.Severity, issue.Code, $"{card.name}: {issue.Message}");

                if (!card.CardId.IsEmpty && !ids.Add(card.CardId))
                    report.Add(CardValidationSeverity.Error, "CARD_ID_DUPLICATE", $"Duplicate card ID '{card.CardId}'.");

                if (!string.IsNullOrWhiteSpace(card.Title) && !names.Add(card.Title.Trim()))
                    report.Add(CardValidationSeverity.Warning, "TITLE_DUPLICATE", $"Duplicate card title '{card.Title}'.");
            }

            return report;
        }
    }
}
