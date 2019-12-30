using System.Collections.Generic;

namespace EsotericRogue {
    public abstract class UnitBrain {
        public Unit Unit { get; internal set; }
        public Scene Scene { get; internal set; }

        public abstract void Controls();

        public virtual bool CanMove(Vector2 position) {
            return Scene.GetTile(position) == Scene.Tile.Ground;
        }

        public bool Move(Vector2 position) {
            if (Scene.InBounds(position) && CanMove(position)) {
                ExecuteMoveUnitToPosition(position);
                return true;
            }
            return false;
        }

        private void ExecuteMoveUnitToPosition(Vector2 position) {
            IReadOnlyCollection<Unit> units = Scene.GetUnits(position);
            if (units != null) {
                foreach (Unit unit in units) {
                    if (!UnitCollision(unit))
                        return;
                }
            }
            Scene.MoveUnit(Unit, position);
        }

        /// <summary>
        /// Return true to continue collision checks with other units.
        /// <para>Returning false will not move the unit.</para>
        /// </summary>
        /// <param name="unit">Colliding unit.</param>
        protected abstract bool UnitCollision(Unit unit);
    }
}
