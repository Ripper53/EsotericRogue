using System.Collections.Generic;
using EsotericRogue;

namespace DungeonRogue {
    public class RandomPositionGetter<T> where T : IScene, IPosition {
        public readonly T Data;

        public RandomPositionGetter(T data) {
            Data = data;
        }

        private Vector2? target;

        private Vector2 GetRandomPosition() {
            IReadOnlyList<Vector2> groundedPositions = Data.Scene.GroundPositions;
            return groundedPositions[RandomUtility.GetInt(groundedPositions.Count)];
        }

        public Vector2 GetTarget() {
            if (!target.HasValue || Data.Position == target)
                target = GetRandomPosition();
            return target.Value;
        }
    }
}
