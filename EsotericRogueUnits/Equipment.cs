namespace EsotericRogue {
    public class Equipment<T> where T : EquippableItem {
        public readonly Character Character;
        public readonly T BareItem;
        public delegate void ItemEquippedAction(Equipment<T> source, T item, T oldItem);
        public event ItemEquippedAction ItemEquipped;
        private T equipped;
        public T Equipped {
            get => equipped;
            set {
                T oldWeapon = equipped;
                oldWeapon.Character = null;
                if (value == null || value == equipped)
                    value = BareItem;
                equipped = value;
                equipped.Character = Character;
                ItemEquipped?.Invoke(this, equipped, oldWeapon);
            }
        }

        /// <param name="character">Owner of equipment.</param>
        /// <param name="bareItem">Default value, sets to this when Equipped = null. Cannot be null.</param>
        public Equipment(Character character, T bareItem) {
            Character = character;
            BareItem = bareItem;
            equipped = bareItem;
            Equipped = bareItem;
            //character.Inventory.AddItem(bareItem);
        }
    }
}
