using System;

namespace EsotericRogue {
    public class Sprite {
        public string Display;
        public ConsoleColor Foreground, Background;

        public Sprite(string display, ConsoleColor foreground = ConsoleColor.Black, ConsoleColor background = ConsoleColor.White) {
            Display = display;
            Foreground = foreground;
            Background = background;
        }
    }
}
