using System;

namespace EsotericRogue {
    public class PlayerUnitBrain : UnitBrain {
        public PlayerInput PlayerInput { get; internal set; }
        public bool ValidInput { get; private set; }

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

        public override bool Move(Vector2 position) {
            if (Scene.InBounds(position)) {
                Unit enemyUnit = Scene.GetUnit(position);
                if (enemyUnit != null)
                    PlayerInput.GameManager.Battle(enemyUnit.Brain);
                return base.Move(position);
            }
            return false;
        }
    }
}
