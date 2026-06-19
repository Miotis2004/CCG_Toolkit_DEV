using System;
using UnityEngine;

namespace CCGToolkit.Cards
{
    [Serializable]
    public sealed class CardStatBlock
    {
        [SerializeField] private bool enabled;
        [SerializeField] private int attack;
        [SerializeField] private int health;
        [SerializeField] private int durability;
        [SerializeField] private int armor;
        [SerializeField] private int resourceModifier;

        public bool Enabled { get => enabled; set => enabled = value; }
        public int Attack { get => attack; set => attack = value; }
        public int Health { get => health; set => health = value; }
        public int Durability { get => durability; set => durability = value; }
        public int Armor { get => armor; set => armor = value; }
        public int ResourceModifier { get => resourceModifier; set => resourceModifier = value; }
    }
}
