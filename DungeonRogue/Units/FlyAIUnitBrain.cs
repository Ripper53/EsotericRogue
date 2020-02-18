using EsotericRogue;

namespace DungeonRogue.Units {
    public abstract class FlyAIUnitBrain : AIUnitBrain {
        private bool flying = false;
        public bool IsFlying {
            get => flying;
            set {
                flying = value;
                if (flying) {
                    Unit.Sprite = FlyingSprite;
                } else {
                    Unit.Sprite = GroundedSprite;
                }
            }
        }
        public Sprite GroundedSprite { get; protected set; }
        public Sprite FlyingSprite { get; protected set; }

        public override bool CanMove(Vector2 position) {
            if (IsFlying)
                return true;
            else
                return base.CanMove(position);
        }
    }
}
