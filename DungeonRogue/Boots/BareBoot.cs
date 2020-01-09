using EsotericRogue;

namespace DungeonRogue.Boots {
    public class BareBoot : Boot {
        public override string Name => "Bare";
        
        public BareBoot() {
            Speed = 2;
        }
    }
}
