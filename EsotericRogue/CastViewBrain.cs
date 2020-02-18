using System.Collections.Generic;

namespace EsotericRogue {
    public class CastViewBrain : ViewBrain {

        #region Static
        public static void CastView(HashSet<Vector2> positions, Scene scene, Vector2 origin, int obstacleCount) {
            // Remember positive is down!

            bool CanSee(Vector2 position) => scene.GetTile(position) != Scene.Tile.Wall;

            int CanSeeCast(Vector2 origin, Vector2 add, int maxCount) {
                bool cannotSee = false;
                for (int i = 0, obstacleI = 0; i < maxCount && scene.InBounds(origin); i++) {
                    // Can see after because we should be able to see the first wall hit!
                    if (cannotSee || !CanSee(origin)) {
                        cannotSee = true;
                        obstacleI++;
                        if (obstacleI > obstacleCount)
                            return i;
                    }
                    positions.Add(origin);
                    origin += add;
                }
                return maxCount;
            }

            void UpdateViewCast(Vector2 incrementValue, Vector2 add) {
                int maxCount = int.MaxValue;
                for (Vector2 i = new Vector2(0, 0), pos = origin; scene.InBounds(pos); i += incrementValue, pos = origin + i) {
                    if (CanSee(pos))
                        maxCount = CanSeeCast(pos, add, maxCount);
                    else
                        return;
                }
            }

            // (go right, cast down)
            UpdateViewCast(new Vector2(1, 0), new Vector2(0, 1));
            // (go right, cast up)
            UpdateViewCast(new Vector2(1, 0), new Vector2(0, -1));
            // (go left, cast down)
            UpdateViewCast(new Vector2(-1, 0), new Vector2(0, 1));
            // (go left, cast up)
            UpdateViewCast(new Vector2(-1, 0), new Vector2(0, -1));

            // (go down, cast right)
            UpdateViewCast(new Vector2(0, 1), new Vector2(1, 0));
            // (go down, cast left)
            UpdateViewCast(new Vector2(0, 1), new Vector2(-1, 0));
            // (go up, cast right)
            UpdateViewCast(new Vector2(0, -1), new Vector2(1, 0));
            // (go up, cast left)
            UpdateViewCast(new Vector2(0, -1), new Vector2(-1, 0));
        }
        #endregion

        protected override void UpdatePositions(HashSet<Vector2> positions, Scene scene, Vector2 origin) {
            CastView(positions, scene, origin, 1);

            // DELETE CODE BELOW, ONLY USED WHEN DEBUGGING!
            for (int y = 0; y < scene.Size.y; y++) {
                for (int x = 0; x < scene.Size.x; x++)
                    positions.Add(new Vector2(x, y));
            }
        }

    }
}
