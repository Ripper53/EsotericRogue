using System;
using System.Collections.Generic;
using System.Text;

namespace EsotericRogue {
    public static class Renderer {
        private static readonly List<Sprite> sprites = new List<Sprite>();
        private static Sprite GetLatest() {
            if (sprites.Count > 0)
                return sprites[sprites.Count - 1];
            return null;
        }

        public static void Clear() => Console.Clear();

        private static void SetUp(Sprite sprite) {
            Console.ForegroundColor = sprite.Foreground;
            Console.BackgroundColor = sprite.Background;
        }

        private static void Write(Sprite sprite) {
            SetUp(sprite);
            Console.Write(sprite.Display);
        }

        public static void Display(Sprite sprite, Vector2 position) {
            Console.SetCursorPosition(position.x, position.y);
            Write(sprite);
        }

        public static void Add(Sprite sprite) {
            Sprite latest = GetLatest();
            if (latest != null && sprite.Foreground == latest.Foreground && sprite.Background == latest.Background) {
                latest.Display += sprite.Display;
            } else {
                sprites.Add(new Sprite(sprite.Display, sprite.Foreground, sprite.Background));
            }
        }

        public static void Add(string display) {
            Sprite latest = GetLatest();
            if (latest != null)
                latest.Display += display;
            else
                sprites.Add(new Sprite(display));
        }

        public static void Display() {
            foreach (Sprite sprite in sprites) {
                Write(sprite);
            }
            sprites.Clear();
        }

        public static void Display(Vector2 position) {
            for (int i = 0, countI = sprites.Count, linePadY = 0; i < countI; i++) {
                Sprite sprite = sprites[i];
                SetUp(sprite);
                string[] lines = sprite.Display.Split(Environment.NewLine);
                Console.SetCursorPosition(position.x, position.y + linePadY);
                Console.Write(lines[0]);
                for (int j = 1, countJ = lines.Length; j < countJ; j++, linePadY++) {
                    Console.SetCursorPosition(position.x, position.y + linePadY);
                    Console.Write(lines[j]);
                }
            }
            sprites.Clear();
        }

        public static string ReadLine() {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.CursorVisible = true;
            string inputString = Console.ReadLine();
            Console.CursorVisible = false;
            return inputString;
        }
    }
}
