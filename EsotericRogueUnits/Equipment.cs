namespace EsotericRogue {
    public class Equipment<T> where T : EquippableItem {
        private readonly Character character;
        private readonly T bareItem;
        public delegate void ItemEquippedAction(Character character, T item, T oldItem);
        public event ItemEquippedAction ItemEquipped;
        private T equipped;
        public T Equipped {
            get => equipped;
            set {
                T oldWeapon = equipped;
                oldWeapon.Character = null;
                if (value == null)
                    value = bareItem;
                equipped = value;
                equipped.Character = character;
                ItemEquipped?.Invoke(character, equipped, oldWeapon);
            }
        }

        /// <param name="character">Owner of equipment.</param>
        /// <param name="bareItem">Default value, sets to this when Equipped = null. Cannot be null.</param>
        public Equipment(Character character, T bareItem) {
            this.character = character;
            this.bareItem = bareItem;
            equipped = bareItem;
            Equipped = bareItem;
            character.Inventory.AddItem(bareItem);
        }
    }
}
