using EsotericRogue;

namespace DungeonRogue.Chestplates {
    public class WoodenChestplate : Chestplate {
        public override string Name => "Wooden";

        public WoodenChestplate() {
            PhysicalDefense = 1;
        }
    }
}
