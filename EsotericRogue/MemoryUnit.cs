using System.Collections.Generic;

namespace EsotericRogue {
    public class MemoryUnit : Unit {
        internal readonly List<Vector2> allPositions = new List<Vector2>();
        public IReadOnlyList<Vector2> AllPositions => allPositions;

        public MemoryUnit(Character character) : base(character) { }
        public MemoryUnit(Character character, UnitBrain unitBrain) : base(character, unitBrain) { }
    }
}
