namespace EsotericRogue {
    public abstract class Item {
        /// <summary>
        /// Unique item name.
        /// </summary>
        public abstract string Name { get; }

        public abstract void Use(Character character);
    }
}
