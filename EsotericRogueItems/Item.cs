namespace EsotericRogue {
    public abstract class Item {
        /// <summary>
        /// Unique item name.
        /// </summary>
        public abstract string Name { get; }
        /// <summary>
        /// True if item is removed from inventory when used, otherwise false.
        /// </summary>
        public bool Consumable = false;
        public Inventory Inventory { get; internal set; }

        public void Use(Character character) {
            if (Consumable) {
                Inventory.RemoveItem(this);
                Inventory = null;
            }
            UseAction(character);
        }
        protected abstract void UseAction(Character character);
    }
}
