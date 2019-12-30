namespace EsotericRogue {
    public class Equipment<T> where T : EquippableItem {
        private readonly Character character;
        public readonly T BareItem;
        public delegate void ItemEquippedAction(Character character, T item, T oldItem);
        public event ItemEquippedAction ItemEquipped;
        private T equipped;
        public T Equipped {
            get => equipped;
            set {
                T oldWeapon = equipped;
                oldWeapon.Character = null;
                if (value == null)
                    value = BareItem;
                equipped = value;
                equipped.Character = character;
                ItemEquipped?.Invoke(character, equipped, oldWeapon);
            }
        }

        /// <param name="character">Owner of equipment.</param>
        /// <param name="bareItem">Default value, sets to this when Equipped = null. Cannot be null.</param>
        public Equipment(Character character, T bareItem) {
            this.character = character;
            this.BareItem = bareItem;
            equipped = bareItem;
            Equipped = bareItem;
            character.Inventory.AddItem(bareItem);
        }
    }
}
