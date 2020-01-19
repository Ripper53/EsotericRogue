using EsotericRogue;
using System;
using System.Collections.Generic;

namespace EsotericRogue {
    public static class AStarPathfinderUtility {
        private static readonly AStarPathfinder aStarPathfinder = new AStarPathfinder();

        public static LinkedList<Vector2> FindAStarPath<T>(T ai, Vector2 end) where T : IScene, IPosition {
            return aStarPathfinder.FindPath(ai.Scene, ai.Position, end);
        }
    }
}
