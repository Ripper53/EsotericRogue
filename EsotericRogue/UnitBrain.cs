namespace EsotericRogue {
    public abstract class UnitBrain {
        public Unit Unit { get; internal set; }
        public Scene Scene { get; internal set; }

        public abstract void Controls();

        public virtual bool Move(Vector2 position) {
            return Scene.MoveUnit(Unit, position);
        }
    }
}
