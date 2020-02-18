using EsotericRogue;
using System;
using System.Collections.Generic;

namespace EsotericRogue {
    public static class AStarPathfinderUtility {
        private static readonly AStarPathfinder aStarPathfinder = new AStarPathfinder();

        public static void FindAStarPath<T>(T arg, LinkedList<Vector2> path, Vector2 end) where T : IScene, IPosition {
            aStarPathfinder.FindPath(path, arg.Scene, arg.Position, end);
        }
    }
}
