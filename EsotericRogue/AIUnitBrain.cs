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
    }
}
