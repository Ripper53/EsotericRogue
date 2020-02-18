using System.Collections.Generic;

namespace EsotericRogue {
    public abstract class Pathfinder {

        public abstract void FindPath(LinkedList<Vector2> path, Scene scene, Vector2 start, Vector2 end);
    }
}
