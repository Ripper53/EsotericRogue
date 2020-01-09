namespace EsotericRogue {
    public abstract class ProtectiveItem : EquippableItem, Enchantment.IPhysicalDefense, Enchantment.IMagicalDefense, Enchantment.IElectricalDefense {
        public int PhysicalDefense { get; set; }
        public int MagicalDefense { get; set; }
        public int ElectricalDefense { get; set; }
    }
}
