using System.Collections.Generic;

namespace EsotericRogue {
    public abstract class Ammo : Item {
        public override bool Consumable => false;

        protected override bool UseAction(Character character) {
            return false;
        }

        public override IEnumerable<Sprite> GetDescription() {
            return new Sprite[] {
                Sprite.CreateUI("Ammo")
            };
        }
    }
}
