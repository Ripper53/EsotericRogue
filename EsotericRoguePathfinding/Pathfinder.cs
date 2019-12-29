using System.Collections.Generic;

namespace EsotericRogue {
    public abstract class Pathfinder {

        public abstract LinkedList<Vector2> FindPath(Scene scene, Vector2 start, Vector2 end);
    }
}
