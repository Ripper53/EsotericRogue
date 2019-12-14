using System;
using System.Collections.Generic;

namespace EsotericRogue {
    public class Menu : SelectableUI {
        public IEnumerable<Sprite> Sprites;
        public IList<Option> Options;
        public int SelectedOptionIndex { get; private set; }

        public Menu() {
            OnSelected += OnSelected_Event;
            OnDeselected += OnDeselected_Event;
        }
        private void OnSelected_Event(SelectableUI source) => WriteSelected();
        private void OnDeselected_Event(SelectableUI source) => WriteUnselected(SelectedOptionIndex);

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
            int offsetY = 0;
            if (Sprites != null) {
                foreach (Sprite sprite in Sprites) {
                    offsetY += sprite.Display.Split(Environment.NewLine).Length - 1;
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
            DisplaySprites();
            DisplayOptions();
        }
        protected void DisplaySprites() {
            if (Sprites != null) {
                foreach (Sprite sprite in Sprites)
                    Renderer.Add(sprite);
            }
        }
        protected void DisplayOptions() {
            if (Options != null) {
                for (int i = 0; i < Options.Count; i++) {
                    if (Selected && i == SelectedOptionIndex)
                        Renderer.Add(SelectedSprite);
                    else
                        Renderer.Add(DeselectedSprite);
                    foreach (Sprite sprite in Options[i].Sprites)
                        Renderer.Add(sprite);
                    Renderer.Add(Environment.NewLine);
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
