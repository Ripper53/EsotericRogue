using System;
using System.Collections.Generic;

namespace EsotericRogue {
    public class PlayerUnitBrain : UnitBrain {
        public PlayerInput PlayerInput { get; internal set; }
        public bool ValidInput { get; private set; }

        #region View
        public readonly HashSet<Vector2> ViewPositions = new HashSet<Vector2>();

        private bool CanSee(Vector2 position) => Scene.GetTile(position) != Scene.Tile.Wall;

        private void CanSeeCast(Vector2 origin, Vector2 add) {
            while (Scene.InBounds(origin) && CanSee(origin)) {
                ViewPositions.Add(origin);
                origin += add;
            }
        }

        private void UpdateViewCast(Vector2 incrementValue, Vector2 add) {
            Vector2 pos = Unit.Position;
            for (Vector2 i = new Vector2(0, 0); Scene.InBounds(pos); i += incrementValue, pos = Unit.Position + i) {
                if (CanSee(pos))
                    CanSeeCast(pos, add);
                else
                    break;
            }
        }
        private readonly HashSet<Vector2> oldViewPositions = new HashSet<Vector2>();
        private void UpdateViewWithoutOldClear() {
            // Remember positive is down!

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
        private void UpdateView() {
            oldViewPositions.Clear();
            oldViewPositions.EnsureCapacity(ViewPositions.Count);
            foreach (Vector2 position in ViewPositions)
                oldViewPositions.Add(position);
            ViewPositions.Clear();
            ViewPositions.Add(Unit.Position);

            UpdateViewWithoutOldClear();

            foreach (Vector2 position in oldViewPositions) {
                if (!ViewPositions.Contains(position)) {
                    // The old position cannot be seen any longer.
                    // Back into the fog!
                    Scene.DisplayFog(position);
                }
            }
            foreach (Vector2 position in ViewPositions) {
                if (!oldViewPositions.Contains(position)) {
                    // New position that has to be seen.
                    Scene.DisplayTile(position);
                }
            }
        }
        #endregion

        public override void Controls() {
            switch (PlayerInput.GetInput().Key) {
                case ConsoleKey.UpArrow:
                case ConsoleKey.W:
                    ValidInput = Move(Unit.UpPosition);
                    break;
                case ConsoleKey.RightArrow:
                case ConsoleKey.D:
                    ValidInput = Move(Unit.RightPosition);
                    break;
                case ConsoleKey.DownArrow:
                case ConsoleKey.S:
                    ValidInput = Move(Unit.DownPosition);
                    break;
                case ConsoleKey.LeftArrow:
                case ConsoleKey.A:
                    ValidInput = Move(Unit.LeftPosition);
                    break;
                default:
                    ValidInput = false;
                    break;
            }
            if (ValidInput) {
                // We have moved!
                // Check what is in view!
                UpdateView();
            }
        }

        public override bool CanMove(Vector2 position) {
            switch (Scene.GetTile(position)) {
                case Scene.Tile.Ground:
                case Scene.Tile.Exit:
                    Unit enemyUnit = Scene.GetUnit(position);
                    if (enemyUnit != null)
                        PlayerInput.GameManager.Battle(enemyUnit.Brain);
                    return true;
                default:
                    return false;
            }
        }

        public void Display() {
            // What do we want to display?
            // The fog of war!
            oldViewPositions.Clear();
            UpdateViewWithoutOldClear();

            // Offset by 2 because of left border and Size.x + 1 to reach the right side.
            Vector2 size = Scene.Size;
            for (int x = -2; x < size.x; x++)
                Renderer.Add(Scene.BorderSprite);
            Renderer.Add(Environment.NewLine);
            for (int y = 0; y < size.y; y++) {
                Renderer.Add(Scene.BorderSprite);
                for (int x = 0; x < size.x; x++) {
                    Vector2 pos = new Vector2(x, y);
                    // Can we see this position?
                    if (ViewPositions.Contains(pos)) {
                        // If we can, display the according tile or unit.
                        Unit unit = Scene.GetUnit(pos);
                        if (unit != null) {
                            Renderer.Add(unit.Sprite);
                        } else {
                            Renderer.Add(Scene.GetTileSprite(pos));
                        }
                    } else {
                        // If we can't, display the fog.
                        Renderer.Add(Scene.FogSprite);
                    }
                }
                Renderer.Add(Scene.BorderSprite);
                Renderer.Add(Environment.NewLine);
            }
            for (int x = -2; x < size.x; x++)
                Renderer.Add(Scene.BorderSprite);
            Renderer.Add(Environment.NewLine);
            Renderer.Display();
        }
    }
}
