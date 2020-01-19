using System.Collections.Generic;

namespace EsotericRogue {
    public enum Rarity {
        Unique,
        Common,
        Uncommon,
        Rare,
        Extraordinary,
        Epic,
        Legendary,
        Galactic
    };
    public abstract class Item {
        /// <summary>
        /// Unique item name.
        /// </summary>
        public abstract string Name { get; }
        public int Space { get; private set; } = 1;
        /// <summary>
        /// True if item is removed from inventory when used, otherwise false.
        /// </summary>
        public abstract bool Consumable { get; }
        /// <summary>
        /// Added inventory.
        /// </summary>
        public Inventory Inventory { get; internal set; }
        public Rarity Rarity = Rarity.Common;

        public bool Use(Character character) {
            if (UseAction(character)) {
                if (Consumable) {
                    Inventory.RemoveItem(this);
                    Inventory = null;
                }
                return true;
            }
            return false;
        }
        protected abstract bool UseAction(Character character);
        public abstract IEnumerable<Sprite> GetDescription();
    }
}
