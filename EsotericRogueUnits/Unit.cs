namespace EsotericRogue {
    public class Unit {
        public Sprite Sprite;
        public Vector2 PreviousPosition { get; private set; }
        private Vector2 position;
        public Vector2 Position {
            get => position;
            internal set {
                PreviousPosition = position;
                position = value;
            }
        }
        public readonly Character Character;
        public readonly UnitBrain Brain;

        public Unit(Character character) => Character = character;

        public Unit(Character character, UnitBrain unitBrain) : this(character) {
            Brain = unitBrain;
            Brain.Unit = this;
        }

        public Vector2 UpPosition => new Vector2(Position.x, Position.y - 1);
        public Vector2 RightPosition => new Vector2(Position.x + 1, Position.y);
        public Vector2 DownPosition => new Vector2(Position.x, Position.y + 1);
        public Vector2 LeftPosition => new Vector2(Position.x - 1, Position.y);
    }
}
