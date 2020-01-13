﻿using System;
using System.Collections.Generic;

namespace EsotericRogue {
    public abstract class AIUnitBrain : UnitBrain {
        protected static readonly RNGMemory rng = new RNGMemory();

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
        }
        public interface IPathfinder {
            LinkedList<Vector2> Path { get; set; }
        }

        #region Static
        private static readonly AStarPathfinder aStarPathfinder = new AStarPathfinder();
        protected static void FindAStarPath<T>(T ai, Vector2 end) where T : AIUnitBrain, IPathfinder {
            ai.Path = aStarPathfinder.FindPath(ai.Scene, ai.Unit.Position, end);
        }
        #endregion
    }
}
