using EsotericRogue;

namespace DungeonRogue.Units {
    public abstract class FriendlyAIUnitBrain : AIUnitBrain {

        protected override bool UnitCollision(Unit unit) {
            if (unit.Brain is PlayerUnitBrain p)
                ((FriendlyUnit)Unit).Interact(p);
            return true;
        }
    }
}
