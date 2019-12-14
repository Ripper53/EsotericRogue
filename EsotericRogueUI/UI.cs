using System;
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
            set {
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

        public static Sprite CreateSprite(string display) {
            return new Sprite(display, ConsoleColor.White, ConsoleColor.Black);
        }

        public static string GetStringMax(string text, int maxLength) {
            StringBuilder stringBuilder = new StringBuilder(text, text.Length + Environment.NewLine.Length);
            if (stringBuilder.Length > maxLength) {
                string n = stringBuilder.ToString(0, maxLength - 1);
                stringBuilder.Clear();
                stringBuilder.Append(n);
                stringBuilder.Append(ContinuedString);
            }
            return stringBuilder.ToString();
        }
        public static string GetStringFill(string text, int length) {
            StringBuilder stringBuilder = new StringBuilder(text, length);
            for (int i = 0, count = length - text.Length; i < count; i++)
                stringBuilder.Append(' ');
            return stringBuilder.ToString();
        }

        public void Display() {
            DisplayUI();
            Renderer.Display(Position);
        }
        protected abstract void DisplayUI();
    }
}
