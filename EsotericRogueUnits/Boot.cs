using System;
using System.Collections.Generic;

namespace EsotericRogue {
    public abstract class Boot : EquippableItem {
        public int Speed { get; set; }

        protected override bool UseAction(Character character) {
            character.Boot.Equipped = this;
            return true;
        }

        public bool Move(int addDistance) {
            int absDistance = Math.Abs(addDistance);
            if (absDistance > Character.RemainingDistance)
                return false;
            Character.RemainingDistance -= absDistance;
            Character.Distance += addDistance;
            return true;
        }

        public override IEnumerable<Sprite> GetDescription() {
            return new Sprite[] {
                Sprite.CreateUI("Speed: " + Speed)
            };
        }
    }
}
