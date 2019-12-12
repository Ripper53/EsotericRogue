namespace EsotericRogue {
    public abstract class SceneGenerator : Generator {
        public readonly Scene Scene;
        public Unit PlayerUnit;

        public SceneGenerator(Scene scene) {
            Scene = scene;
        }

        protected void SetPlayerUnit(Vector2 position) {
            Scene.SetUnit(PlayerUnit, position);
        }

        protected void GeneratePath(Vector2 start, Vector2 end) {
            Scene.SetTile(Scene.Tile.Ground, start);
            while (start != end) {
                if (start.x < end.x)
                    start.x++;
                else if (start.x > end.x)
                    start.x--;
                else if (start.y < end.y)
                    start.y++;
                else if (start.y > end.y)
                    start.y--;
                Scene.SetTile(Scene.Tile.Ground, start);
            }
        }

        protected void GenerateBox(Vector2 start, Vector2 size) {
            for (int y = 0, startX = start.x; y < size.y; y++, start.y++, start.x = startX) {
                for (int x = 0; x < size.x; x++, start.x++) {
                    if (Scene.InBounds(start))
                        Scene.SetTile(Scene.Tile.Ground, start);
                }
            }
        }
    }
}
