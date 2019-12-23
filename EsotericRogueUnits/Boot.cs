using System;

namespace EsotericRogue {
    public abstract class Boot : EquippableItem {
        public abstract int Speed { get; }
        public override bool Consumable => false;

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
    }
}
