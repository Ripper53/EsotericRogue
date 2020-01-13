using System.Collections.Generic;

namespace EsotericRogue {
    public abstract class Potion : Item {
        public override bool Consumable => true;

        public override IEnumerable<Sprite> GetDescription() {
            return new Sprite[] {
                Sprite.CreateUI("Potion")
            };
        }
    }
}
