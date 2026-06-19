using System;
using System.Collections.Generic;

namespace CCGToolkit.Cards
{
    public enum CardValidationSeverity { Info, Warning, Error }

    public readonly struct CardValidationIssue
    {
        public CardValidationIssue(CardValidationSeverity severity, string code, string message)
        {
            Severity = severity;
            Code = code ?? string.Empty;
            Message = message ?? string.Empty;
        }

        public CardValidationSeverity Severity { get; }
        public string Code { get; }
        public string Message { get; }
    }

    public sealed class CardValidationReport
    {
        private readonly List<CardValidationIssue> issues = new List<CardValidationIssue>();
        public IReadOnlyList<CardValidationIssue> Issues => issues;
        public bool IsValid => !issues.Exists(issue => issue.Severity == CardValidationSeverity.Error);
        public void Add(CardValidationSeverity severity, string code, string message) => issues.Add(new CardValidationIssue(severity, code, message));
    }
}
