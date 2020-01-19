using System;
using System.Collections.Generic;

namespace EsotericRogue {
    public class PlayerUnitBrain : UnitBrain {
        public PlayerInput PlayerInput { get; internal set; }
        public bool ValidInput { get; private set; }

        private ViewBrain viewBrain = new CastViewBrain();
        public void SetViewBrain(ViewBrain viewBrain) => this.viewBrain = viewBrain;

        public bool CanView(Vector2 position) => viewBrain.Contains(position);
        public void UpdateView() => viewBrain.Update(Scene, Unit.Position);

        public void Battle(UnitBrain unitBrain) {
            PlayerInput.GameManager.Battle(unitBrain);
        }

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
        }

        public override bool CanMove(Vector2 position) {
            switch (Scene.GetTile(position)) {
                case Scene.Tile.Ground:
                case Scene.Tile.Exit:
                    return true;
                default:
                    return false;
            }
        }

        public void Display() {
            // What do we want to display?
            // The fog of war!
            viewBrain.UpdateWithoutReset(Scene, Unit.Position);

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
                    if (viewBrain.Contains(pos)) {
                        // If we can, display the according tile or unit.
                        IReadOnlyCollection<Unit> units = Scene.GetUnits(pos);
                        if (units != null) {
                            if (pos == Unit.Position) {
                                Renderer.Add(Unit.Sprite);
                            } else {
                                foreach (Unit unit in units) {
                                    Renderer.Add(unit.Sprite);
                                    break;
                                }
                            }
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

        protected override bool UnitCollision(Unit unit) {
            if (unit is FriendlyUnit friendlyUnit)
                PlayerInput.GameManager.Interact(friendlyUnit);
            else
                PlayerInput.GameManager.Battle(unit.Brain);
            return true;
        }
    }
}
