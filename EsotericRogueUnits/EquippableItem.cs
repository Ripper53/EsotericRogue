namespace EsotericRogue {
    public abstract class EquippableItem : Item {
        /// <summary>
        /// Equipped character.
        /// </summary>
        public Character Character { get; internal set; }
    }
}
