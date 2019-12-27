using System;
using System.Text;
using System.Collections.Generic;

namespace EsotericRogue {
    public class TextUI : UI {
        public IEnumerable<Sprite> Sprites;
        public int Width = 10;
        public int MaxLength = 100;

        private int widthCount;
        private string WordWrap(string text) {
            string[] lines = text.Split(Environment.NewLine);
            int resultCount = 0;
            for (int i = 0; i < lines.Length; i++) {
                string line = lines[i];
                string[] words = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                StringBuilder stringBuilder = new StringBuilder(text.Length + words.Length);
                foreach (string word in words) {
                    stringBuilder.Append(word);
                    stringBuilder.Append(' ');
                    widthCount += word.Length;
                    if (widthCount > Width) {
                        widthCount = 0;
                        stringBuilder.Append(Environment.NewLine);
                    }
                }
                lines[i] = stringBuilder.ToString();
                resultCount += lines[i].Length;
            }
            StringBuilder result = new StringBuilder(resultCount);
            foreach (string line in lines)
                result.Append(line);
            return result.ToString();
        }

        protected override void DisplayUI() {
            if (Sprites != null) {
                widthCount = 0;
                int characterCount = 0;
                foreach (Sprite sprite in Sprites) {
                    Sprite wrappedSprite = new Sprite(WordWrap(sprite.Display), sprite.Foreground, sprite.Background);
                    int thisCharacterCount = wrappedSprite.Display.Length;
                    characterCount += thisCharacterCount;
                    if (characterCount > MaxLength) {
                        wrappedSprite.Display = GetContinuedString(wrappedSprite.Display, thisCharacterCount - (characterCount - MaxLength));
                        Renderer.Add(wrappedSprite);
                        return;
                    }
                    Renderer.Add(wrappedSprite);
                }
            }
        }
    }
}
