using System;
using System.Collections.Generic;

namespace EsotericRogue {
    public static class Renderer {
        private static readonly List<Sprite> sprites = new List<Sprite>();
        private static Sprite GetLatest() {
            if (sprites.Count > 0)
                return sprites[^1];
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
            int startX = position.x;
            Console.SetCursorPosition(position.x, position.y);
            foreach (Sprite sprite in sprites) {
                SetUp(sprite);
                string[] lines = sprite.Display.Split(Environment.NewLine);
                if (lines.Length > 1) {
                    // If there is more than 1 line, then we need to make sure the last line does not create a new line!
                    int count = lines.Length - 1;
                    for (int i = 0; i < count; i++) {
                        Console.CursorLeft = position.x;
                        Console.WriteLine(lines[i]);
                    }
                    Console.CursorLeft = position.x;
                    position.x = startX;
                    Console.Write(lines[count]);
                } else {
                    foreach (string line in lines) {
                        Console.CursorLeft = position.x;
                        position.x += line.Length;
                        Console.Write(line);
                    }
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
