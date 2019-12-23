namespace EsotericRogue {
    public abstract class ProtectiveItem : EquippableItem {
        public override bool Consumable => false;

        public int PhysicalDefense { get; set; }
        public int MagicalDefense { get; set; }
        public int ElectricalDefense { get; set; }
    }
}
