namespace EsotericRogue {
    public struct Vector2 {
        public int x, y;

        public Vector2(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public override string ToString() => $"({x}, {y})";

        public static bool operator ==(Vector2 vec1, Vector2 vec2) => vec1.x == vec2.x && vec1.y == vec2.y;
        public static bool operator !=(Vector2 vec1, Vector2 vec2) => vec1.x != vec2.x || vec1.y != vec2.y;

        public static Vector2 operator +(Vector2 vec1, Vector2 vec2) => new Vector2(vec1.x + vec2.x, vec1.y + vec2.y);
        public static Vector2 operator -(Vector2 vec1, Vector2 vec2) => new Vector2(vec1.x - vec2.x, vec1.y - vec2.y);
    }
}
