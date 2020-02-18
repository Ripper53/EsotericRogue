using EsotericRogue;
using System.Collections.Generic;

namespace DungeonRogue.Units {
    public abstract class FlyMinDistanceAIUnitBrain : FlyAIUnitBrain, AIUnitBrain.IView, AIUnitBrain.IPathfinder {
        public HashSet<Vector2> View { get; set; }
        public LinkedList<Vector2> Path { get; set; }

        public int GroundMinDistance = 5, FlyMinDistance = 5, FlyViewDistance = 10;
        private int groundDistance = 0, flyDistance = 0;

        private readonly RandomPositionGetter<FlyMinDistanceAIUnitBrain> randomPositionGetter;

        public FlyMinDistanceAIUnitBrain() {
            randomPositionGetter = new RandomPositionGetter<FlyMinDistanceAIUnitBrain>(this);
        }

        public override void PreCalculate(GameManager gameManager) {
            Vector2 target, playerPos = gameManager.PlayerInfo.Unit.Position;
            if (IsFlying) {

                if (Vector2.Distance(Position, playerPos) <= FlyViewDistance) {
                    target = playerPos;
                } else {
                    randomPositionGetter.Type = RandomPositionGetter.TargetType.Any;
                    target = randomPositionGetter.GetTarget();
                }

                if (Position.x < target.x) {
                    target = new Vector2(Position.x + 1, Position.y);
                } else if (Position.x > target.x) {
                    target = new Vector2(Position.x - 1, Position.y);
                } else if (Position.y < target.y) {
                    target = new Vector2(Position.x, Position.y + 1);
                } else if (Position.y > target.y) {
                    target = new Vector2(Position.x, Position.y - 1);
                }

                if (Path == null)
                    Path = new LinkedList<Vector2>();
                else
                    Path.Clear();
                Path.AddFirst(target);
                Path.AddLast(target);
            } else {
                IView.CastView(this, 0);

                if (View.Contains(playerPos)) {
                    target = playerPos;
                } else {
                    randomPositionGetter.Type = RandomPositionGetter.TargetType.Ground;
                    target = randomPositionGetter.GetTarget();
                }

                IPathfinder.FindAStarPath(this, target);
            }

        }

        public override void Controls() {
            MoveToPath(this);
            if (IsFlying) {
                flyDistance++;
                if (flyDistance > FlyMinDistance && Scene.GetTile(Position) == Scene.Tile.Ground) {
                    flyDistance = 0;
                    IsFlying = false;
                }
            } else {
                groundDistance++;
                if (groundDistance > GroundMinDistance) {
                    groundDistance = 0;
                    IsFlying = true;
                }
            }
        }
    }
}
