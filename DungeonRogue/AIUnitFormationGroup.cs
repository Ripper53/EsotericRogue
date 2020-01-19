using System.Collections.Generic;
using EsotericRogue;

namespace DungeonRogue {
    public class AIUnitFormationGroup<T> : AIUnitGroup<T> where T : AIUnitBrain, IUnitGroup<T> {
        private readonly List<Vector2> formations = new List<Vector2>();
        public Vector2 Position;

        public AIUnitFormationGroup(Vector2 position) {
            Position = position;
        }

        public override void Add(T unitBrain) {
            base.Add(unitBrain);
            formations.Add(new Vector2(0, 0));
        }

        public void Add(T unitBrain, Vector2 position) {
            base.Add(unitBrain);
            formations.Add(position);
        }

        public Vector2 GetPosition(int index) => formations[index];
        public void SetPosition(int index, Vector2 position) => formations[index] = position;
    }
}
