using EsotericRogue;

namespace DungeonRogue.Sleeves {
    public class RubberSleeve : Sleeve {
        public override string Name => "Rubber";

        public RubberSleeve() {
            ElectricalDefense = 10;
        }
    }
}
