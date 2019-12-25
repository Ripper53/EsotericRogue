using System.Collections.Generic;

namespace EsotericRogue {
    public abstract class ViewBrain {
        private readonly HashSet<Vector2> oldPositions, positions;
        
        public ViewBrain() {
            oldPositions = new HashSet<Vector2>();
            positions = new HashSet<Vector2>();
        }

        public bool Contains(Vector2 position) => positions.Contains(position);

        public void Update(Scene scene, Vector2 origin) {
            oldPositions.Clear();
            oldPositions.EnsureCapacity(positions.Count);
            foreach (Vector2 position in positions)
                oldPositions.Add(position);

            positions.Clear();
            UpdatePositions(positions, scene, origin);

            foreach (Vector2 position in oldPositions) {
                if (!positions.Contains(position)) {
                    // The old position cannot be seen any longer.
                    // Back into the fog!
                    scene.DisplayFog(position);
                }
            }
            foreach (Vector2 position in positions) {
                if (!oldPositions.Contains(position)) {
                    // New position that has to be seen.
                    scene.DisplayTile(position);
                }
            }
        }
        public void UpdateWithoutReset(Scene scene, Vector2 origin) {
            oldPositions.Clear();
            positions.Clear();
            UpdatePositions(positions, scene, origin);
        }
        protected abstract void UpdatePositions(HashSet<Vector2> positions, Scene scene, Vector2 origin);
    }
}
