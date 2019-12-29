using EsotericRogue;

namespace DungeonRogue.Chestplates {
    public class SteelChestplate : Chestplate {
        public override string Name => "Steel";

        public SteelChestplate() {
            PhysicalDefense = 5;
        }
    }
}
