using EsotericRogue;

namespace DungeonRogue.Chestplates {
    public class SteelChestplate : Chestplate {
        public override string Name => "Steel Chestplate";

        public SteelChestplate() {
            PhysicalDefense = 10;
        }
    }
}
