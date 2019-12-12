using System;
using System.Collections.Generic;

namespace EsotericRogue {
    public class Menu : SelectableUI {
        public IEnumerable<Sprite> Sprites;
        public IList<Option> Options;
        public int SelectedOptionIndex { get; private set; }

        protected override void OnSelected() {
            WriteSelected();
        }

        protected override void OnDeselected() {
            WriteUnselected(SelectedOptionIndex);
        }

        public Option GetSelectedOption() {
            if (Options != null && Options.Count > 0)
                return Options[SelectedOptionIndex];
            return null;
        }

        public class Option {
            public IEnumerable<Sprite> Sprites;
            public delegate void OptionAction(Menu menu, Option option);
            public OptionAction Action;
        }

        private int GetOffsetY() {
            int offsetY = 1;
            if (Sprites != null) {
                foreach (Sprite sprite in Sprites) {
                    offsetY += sprite.Display.Split(Environment.NewLine).Length;
                }
            }
            return offsetY;
        }

        private int WriteUnselected(int index) {
            int offsetY = GetOffsetY();
            Renderer.Display(DeselectedSprite, new Vector2(Position.x, Position.y + index + offsetY));
            return offsetY;
        }

        private void WriteSelected(int previousIndex) {
            int offsetY = WriteUnselected(previousIndex);
            if (Selected) {
                Renderer.Display(SelectedSprite, new Vector2(Position.x, Position.y + SelectedOptionIndex + offsetY));
            }
        }

        private void WriteSelected() {
            Renderer.Display(SelectedSprite, new Vector2(Position.x, Position.y + SelectedOptionIndex + GetOffsetY()));
        }

        public void NextOption() {
            int previousIndex = SelectedOptionIndex;
            SelectedOptionIndex++;
            if (SelectedOptionIndex >= Options.Count)
                SelectedOptionIndex = 0;
            WriteSelected(previousIndex);
        }

        public void PreviousOption() {
            int previousIndex = SelectedOptionIndex;
            SelectedOptionIndex--;
            if (SelectedOptionIndex < 0)
                SelectedOptionIndex = Options.Count - 1;
            WriteSelected(previousIndex);
        }

        public static Sprite
            SelectedSprite = new Sprite(">", ConsoleColor.White, ConsoleColor.Black),
            DeselectedSprite = new Sprite(" ", ConsoleColor.White, ConsoleColor.Black);
        protected override void DisplayUI() {
            if (Sprites != null) {
                foreach (Sprite sprite in Sprites)
                    Renderer.Add(sprite);
                Renderer.Add(Environment.NewLine);
            }
            if (Options != null) {
                for (int i = 0; i < Options.Count; i++) {
                    Option option = Options[i];
                    Renderer.Add(Environment.NewLine);
                    if (Selected && i == SelectedOptionIndex)
                        Renderer.Add(SelectedSprite);
                    else
                        Renderer.Add(DeselectedSprite);
                    foreach (Sprite sprite in option.Sprites)
                        Renderer.Add(sprite);
                }
            }
        }

        public override void Up() {
            PreviousOption();
        }

        public override void Right() {
            NextOption();
        }

        public override void Down() {
            NextOption();
        }

        public override void Left() {
            PreviousOption();
        }

        public override void Enter() {
            Option option = GetSelectedOption();
            if (option != null)
                option.Action(this, option);
        }
    }
}
