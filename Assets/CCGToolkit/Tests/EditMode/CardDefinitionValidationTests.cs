using CCGToolkit.Cards;
using NUnit.Framework;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CCGToolkit.Tests.EditMode
{
    public sealed class CardDefinitionValidationTests
    {
        [Test]
        public void MissingIdentityAndTitleAreErrors()
        {
            var card = ScriptableObject.CreateInstance<CardDefinition>();
            card.Cost = 1;

            var report = card.Validate(requireArt: false);

            Assert.That(report.IsValid, Is.False);
            Assert.That(report.Issues, Has.Exactly(1).Matches<CardValidationIssue>(issue => issue.Code == "CARD_ID_MISSING"));
            Assert.That(report.Issues, Has.Exactly(1).Matches<CardValidationIssue>(issue => issue.Code == "TITLE_MISSING"));
        }

        [Test]
        public void MinionRequiresPositiveHealth()
        {
            var card = ScriptableObject.CreateInstance<CardDefinition>();
            card.CardId = new CardIdentifier("test_minion");
            card.Title = "Test Minion";
            card.CardType = CardType.Minion;
            card.Cost = 2;
            card.Stats.Enabled = true;
            card.Stats.Attack = 2;
            card.Stats.Health = 0;

            var report = card.Validate(requireArt: false);

            Assert.That(report.IsValid, Is.False);
            Assert.That(report.Issues, Has.Exactly(1).Matches<CardValidationIssue>(issue => issue.Code == "MINION_HEALTH_REQUIRED"));
        }

#if UNITY_EDITOR
        [Test]
        public void SampleCardsLoadAndValidate()
        {
            string[] guids = AssetDatabase.FindAssets("t:CardDefinition", new[] { "Assets/CCGToolkit/Samples/SampleCards" });

            Assert.That(guids.Length, Is.GreaterThanOrEqualTo(20));
            foreach (string guid in guids)
            {
                var card = AssetDatabase.LoadAssetAtPath<CardDefinition>(AssetDatabase.GUIDToAssetPath(guid));
                Assert.That(card, Is.Not.Null);
                Assert.That(card.Validate(requireArt: false).IsValid, Is.True, card != null ? card.name : guid);
            }
        }
#endif
    }
}
