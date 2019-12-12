namespace EsotericRogue {
    public abstract class UI {
        public static string ContinuedString = "…";
        public Vector2 Position;

        public static Sprite CreateSprite(string display) {
            return new Sprite(display, System.ConsoleColor.White, System.ConsoleColor.Black);
        }

        public void Display() {
            DisplayUI();
            Renderer.Display(Position);
        }
        protected abstract void DisplayUI();
    }
}
