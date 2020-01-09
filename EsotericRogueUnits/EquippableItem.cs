namespace EsotericRogue {
    public abstract class EquippableItem : Item {
        public override bool Consumable => false;
        /// <summary>
        /// Equipped character.
        /// </summary>
        public Character Character { get; internal set; }

        public delegate void EquippedAction();
        public event EquippedAction Equipped, Unequipped;
        internal void OnEquipped() => Equipped?.Invoke();
        internal void OnUnequipped() => Unequipped?.Invoke();
    }
}
