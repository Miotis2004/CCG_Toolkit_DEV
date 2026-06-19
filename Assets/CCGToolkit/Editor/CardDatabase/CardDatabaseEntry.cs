using CCGToolkit.Cards;

namespace CCGToolkit.Editor.CardDatabase
{
    public sealed class CardDatabaseEntry
    {
        public CardDatabaseEntry(CardDefinition card, string assetPath, string assetGuid)
        {
            Card = card;
            AssetPath = assetPath ?? string.Empty;
            AssetGuid = assetGuid ?? string.Empty;
        }

        public CardDefinition Card { get; }
        public string AssetPath { get; }
        public string AssetGuid { get; }
    }
}
