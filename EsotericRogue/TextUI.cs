using System;
using System.Text;
using System.Collections.Generic;

namespace EsotericRogue {
    public class TextUI : UI {
        public IEnumerable<Sprite> Sprites;
        public int Width = 10;
        public int MaxLength = 100;

        private string WordWrap(string text, ref int widthCount) {
            string[] lines = text.Split(Environment.NewLine);
            StringBuilder stringBuilder = new StringBuilder(text.Length);
            for (int i = 0, count = lines.Length; i < count; i++) {
                string line = lines[i];
                string[] words = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                foreach (string word in words) {
                    stringBuilder.Append(word);
                    stringBuilder.Append(' ');
                    widthCount += word.Length;
                    if (widthCount > Width) {
                        widthCount = 0;
                        stringBuilder.Append(Environment.NewLine);
                    }
                }
                if (count > 1) {
                    widthCount = 0;
                    stringBuilder.Append(Environment.NewLine);
                }
                lines[i] = stringBuilder.ToString();
                stringBuilder.Clear();
            }
            foreach (string line in lines)
                stringBuilder.Append(line);
            return stringBuilder.ToString();
        }

        protected override void DisplayUI() {
            if (Sprites != null) {
                int characterCount = 0, widthCount = 0;
                foreach (Sprite sprite in Sprites) {
                    Sprite wrappedSprite = new Sprite(WordWrap(sprite.Display, ref widthCount), sprite.Foreground, sprite.Background);
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
