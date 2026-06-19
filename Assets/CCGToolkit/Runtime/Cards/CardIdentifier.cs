using System;
using UnityEngine;

namespace CCGToolkit.Cards
{
    [Serializable]
    public struct CardIdentifier : IEquatable<CardIdentifier>
    {
        [SerializeField] private string value;

        public CardIdentifier(string value)
        {
            this.value = Normalize(value);
        }

        public string Value => value;
        public bool IsEmpty => string.IsNullOrWhiteSpace(value);

        public static CardIdentifier NewGuid() => new CardIdentifier(Guid.NewGuid().ToString("N"));

        public static string Normalize(string raw) => string.IsNullOrWhiteSpace(raw) ? string.Empty : raw.Trim();

        public bool Equals(CardIdentifier other) => string.Equals(value, other.value, StringComparison.Ordinal);
        public override bool Equals(object obj) => obj is CardIdentifier other && Equals(other);
        public override int GetHashCode() => value == null ? 0 : StringComparer.Ordinal.GetHashCode(value);
        public override string ToString() => value ?? string.Empty;
    }
}
