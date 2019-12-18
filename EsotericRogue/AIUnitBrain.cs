using System;
using System.Collections.Generic;

namespace EsotericRogue {
    public class AIUnitBrain : UnitBrain {
        protected static readonly Random rng = new Random();
        public delegate void ControlsAction(AIUnitBrain source);
        public IEnumerable<ControlsAction> ControlsActions;

        public override void Controls() {
            foreach (ControlsAction action in ControlsActions)
                action(this);
        }

        protected virtual bool CanMoveCheck(Vector2 position) {
            return base.CanMove(position);
        }

        public sealed override bool CanMove(Vector2 position) {
            if (CanMoveCheck(position)) {
                Unit unit = Scene.GetUnit(position);
                if (unit == null)
                    return true;
                else if (unit.Brain is PlayerUnitBrain p)
                    p.PlayerInput.GameManager.Battle(this);
                return false;
            }
            return false;
        }
    }
}
