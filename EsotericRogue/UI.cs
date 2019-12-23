using System;
using System.Collections.Generic;
using System.Text;

namespace EsotericRogue {
    public abstract class UI {
        public static string ContinuedString = "…";
        public delegate void ActivatedAction(UI ui);
        public ActivatedAction Activated;
        public delegate void DeactivatedAction(UI ui);
        public DeactivatedAction Deactivated;
        private bool active = false;
        public bool Active {
            get => active;
            internal set {
                bool procEvent = active != value;
                active = value;
                if (procEvent) {
                    if (active)
                        Activated?.Invoke(this);
                    else
                        Deactivated?.Invoke(this);
                }
            }
        }
        public Vector2 Position;

        public static string GetContinuedString(string text, int maxLength) {
            StringBuilder stringBuilder = new StringBuilder(text, text.Length + Environment.NewLine.Length);
            if (stringBuilder.Length > maxLength) {
                string n = stringBuilder.ToString(0, maxLength - 1);
                stringBuilder.Clear();
                stringBuilder.Append(n);
                stringBuilder.Append(ContinuedString);
            }
            return stringBuilder.ToString();
        }
        public static string GetFilledString(string text, int length) {
            StringBuilder stringBuilder = new StringBuilder(text, text.Length + length);
            for (int i = 0; i < length; i++)
                stringBuilder.Append(' ');
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Like string.PadRight but excludes Environment.NewLine
        /// </summary>
        public static string GetStringPadRight(string text, int maxLength) {
            StringBuilder stringBuilder = new StringBuilder(text, maxLength);
            int count = maxLength - text.Replace(Environment.NewLine, string.Empty).Length;
            for (int i = 0; i < count; i++) {
                stringBuilder.Append(' ');
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Length of string excluding Environment.NewLine
        /// </summary>
        public static int GetStringLength(string text) {
            return text.Replace(Environment.NewLine, string.Empty).Length;
        }

        protected Vector2 ClearPosition { get; private set; }
        private readonly List<Sprite> clearSprites = new List<Sprite>();

        public void Display() {
            DisplayUI();
            // Clear display prep
            ClearPosition = Position;
            clearSprites.Clear();
            clearSprites.Capacity = Renderer.Sprites.Count;
            StringBuilder stringBuilder = new StringBuilder();
            foreach (Sprite sprite in Renderer.Sprites) {
                stringBuilder.Clear();
                stringBuilder.EnsureCapacity(sprite.Display.Length);
                string[] lines = sprite.Display.Split(Environment.NewLine);
                int count = lines.Length - 1;
                for (int i = 0; i < count; i++) {
                    for (int j = 0, c = lines[i].Length; j < c; j++)
                        stringBuilder.Append(' ');
                    stringBuilder.Append(Environment.NewLine);
                }
                // There should be no new line at the end of the last line.
                for (int i = 0, c = lines[count].Length; i < c; i++)
                    stringBuilder.Append(' ');
                clearSprites.Add(Sprite.CreateUI(stringBuilder.ToString()));
            }
            // Display UI
            Renderer.Display(Position);
        }
        protected abstract void DisplayUI();

        public virtual void Clear() {
            foreach (Sprite sprite in clearSprites)
                Renderer.Add(sprite);
            Renderer.Display(ClearPosition);
        }
    }
}
