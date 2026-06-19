#if UNITY_EDITOR
using CCGToolkit.Editor.CardDatabase;
using CCGToolkit.Cards;
using NUnit.Framework;

namespace CCGToolkit.Tests.EditMode
{
    public sealed class CardDatabaseServiceTests
    {
        [Test]
        public void DatabaseIndexesAndFiltersSampleCards()
        {
            var database = new CardDatabaseService();
            database.RebuildIndex(new[] { "Assets/CCGToolkit/Samples/SampleCards" });

            Assert.That(database.Entries.Count, Is.GreaterThanOrEqualTo(20));
            Assert.That(database.Query(new CardSearchQuery { Type = CardType.Minion }), Is.Not.Empty);
            Assert.That(database.ValidateAll().IsValid, Is.True);
        }

        [Test]
        public void ExportIncludesCoreCardFields()
        {
            var database = new CardDatabaseService();
            database.RebuildIndex(new[] { "Assets/CCGToolkit/Samples/SampleCards" });

            string csv = CardImportExportUtility.ToCsv(database.Entries.Count > 0 ? new[] { database.Entries[0].Card } : new CardDefinition[0]);

            Assert.That(csv, Does.Contain("id,title,description,cost,type,rarity,faction,set,target"));
        }
    }
}
#endif
