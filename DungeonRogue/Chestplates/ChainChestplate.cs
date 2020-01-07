using EsotericRogue;

namespace DungeonRogue.Chestplates {
    public class ChainChestplate : Chestplate {
        public override string Name => "Chain Chestplate";

        public ChainChestplate() {
            PhysicalDefense = 20;
        }
    }
}
