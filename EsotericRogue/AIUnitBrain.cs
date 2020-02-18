using System.Collections.Generic;

namespace EsotericRogue {
    public abstract class AIUnitBrain : UnitBrain {

        protected override bool UnitCollision(Unit unit) {
            if (unit.Brain is PlayerUnitBrain p) {
                p.Battle(this);
                return false;
            }
            return true;
        }

        public abstract void PreCalculate(GameManager gameManager);

        public interface IView {
            HashSet<Vector2> View { get; set; }

            public static void CastView<T>(T ai, int obstacleCount) where T : AIUnitBrain, IView {
                if (ai.View == null)
                    ai.View = new HashSet<Vector2>();
                else
                    ai.View.Clear();
                CastViewBrain.CastView(ai.View, ai.Scene, ai.Unit.Position, obstacleCount);
            }
        }
        public interface IPathfinder {
            LinkedList<Vector2> Path { get; set; }

            public static void FindAStarPath<T>(T ai, Vector2 end) where T : AIUnitBrain, IPathfinder {
                if (ai.Path == null)
                    ai.Path = new LinkedList<Vector2>();
                else
                    ai.Path.Clear();
                AStarPathfinderUtility.FindAStarPath(ai, ai.Path, end);
            }
        }

        #region Static
        protected static void MoveToPath<T>(T ai) where T : AIUnitBrain, IPathfinder {
            if (ai.Path != null && ai.Path.First != null && ai.Path.First.Next != null) {
                ai.Move(ai.Path.First.Next.Value);
            }
        }
        #endregion
    }
}
