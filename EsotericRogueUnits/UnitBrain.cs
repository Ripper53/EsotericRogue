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
            Unit unit = Scene.GetUnit(position);
            if (unit != null && unit.Brain is IBattleUnitBrain p)
                p.Battle(this);
            else
                Scene.MoveUnit(Unit, position);
        }
    }
}
