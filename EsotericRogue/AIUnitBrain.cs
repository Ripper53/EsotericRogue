using System;
using System.Collections.Generic;

namespace EsotericRogue {
    public abstract class AIUnitBrain : UnitBrain {
        protected static readonly Random rng = new Random();

        public abstract void PreCalculate(GameManager gameManager);

        public interface IView {
            HashSet<Vector2> View { get; set; }
        }
        public interface IPathfinder {
            LinkedList<Vector2> Path { get; set; }
        }

        #region Static
        private static readonly AStarPathfinder aStarPathfinder = new AStarPathfinder();
        public static void FindAStarPath<T>(T ai, Vector2 end) where T : AIUnitBrain, IPathfinder {
            ai.Path = aStarPathfinder.FindPath(ai.Scene, ai.Unit.Position, end);
        }
        #endregion
    }
}
