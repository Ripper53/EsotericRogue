using System;
using System.Text;

namespace EsotericRogue {
    public class Sprite {
        public string Display;
        public ConsoleColor Foreground, Background;

        public Sprite(string display, ConsoleColor foreground = ConsoleColor.Black, ConsoleColor background = ConsoleColor.White) {
            Display = display;
            Foreground = foreground;
            Background = background;
        }

        public static Sprite CreateUI(string text) {
            return new Sprite(text, ConsoleColor.White, ConsoleColor.Black);
        }

        public static Sprite CreateEmptyUI(int count) {
            StringBuilder stringBuilder = new StringBuilder(count);
            for (int i = 0; i < count; i++)
                stringBuilder.Append(' ');
            return CreateUI(stringBuilder.ToString());
        }
    }
}
