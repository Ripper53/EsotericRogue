using System.Collections.Generic;
using EsotericRogue;

namespace DungeonRogue {
    public abstract class RandomPositionGetter {
        public enum TargetType {
            Ground, Any
        };
        public TargetType Type = TargetType.Ground;

        public abstract Vector2 GetTarget();
    }
    public class RandomPositionGetter<T> : RandomPositionGetter where T : IScene, IPosition {
        public readonly T Data;

        public RandomPositionGetter(T data) {
            Data = data;
        }

        private Vector2? target;

        private Vector2 GetRandomPosition() {
            switch (Type) {
                case TargetType.Ground:
                    IReadOnlyList<Vector2> groundedPositions = Data.Scene.GroundPositions;
                    return groundedPositions[RandomUtility.GetInt(groundedPositions.Count)];
                default:
                    Vector2 size = Data.Scene.Size;
                    return new Vector2(RandomUtility.GetInt(size.x), RandomUtility.GetInt(size.y));
            }
        }

        public override Vector2 GetTarget() {
            if (!target.HasValue || Data.Position == target || (Type == TargetType.Ground && Data.Scene.GetTile(target.Value) != Scene.Tile.Ground))
                target = GetRandomPosition();
            return target.Value;
        }
    }
}
