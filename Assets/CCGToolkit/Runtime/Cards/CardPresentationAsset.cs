using UnityEngine;

namespace CCGToolkit.Cards
{
    [CreateAssetMenu(menuName = "CCG Toolkit/Cards/Card Presentation Asset", fileName = "New Card Presentation Asset")]
    public sealed class CardPresentationAsset : ScriptableObject
    {
        [SerializeField] private Color primaryColor = Color.white;
        [SerializeField] private Color secondaryColor = Color.black;
        [SerializeField, TextArea(2, 4)] private string designerNotes;

        public Color PrimaryColor => primaryColor;
        public Color SecondaryColor => secondaryColor;
        public string DesignerNotes => designerNotes;
    }
}
