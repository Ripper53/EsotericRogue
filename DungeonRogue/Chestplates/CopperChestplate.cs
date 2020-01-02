using EsotericRogue;

namespace DungeonRogue.Chestplates {
    public class CopperChestplate : Chestplate {
        public override string Name => "Copper Chestplate";

        public CopperChestplate() {
            PhysicalDefense = 5;
        }
    }
}
