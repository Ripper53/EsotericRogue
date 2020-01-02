using System;
using System.Collections.Generic;

namespace EsotericRogue {
    public class Menu : SelectableUI {
        public IEnumerable<Sprite> Sprites;
        private readonly List<Option> options;
        public IReadOnlyList<Option> Options => options;
        public int SelectedOptionIndex { get; private set; }

        public Menu() {
            options = new List<Option>();

            OnSelected += OnSelected_Event;
            OnDeselected += OnDeselected_Event;
        }

        private void OnSelected_Event(SelectableUI source) => WriteSelected();
        private void OnDeselected_Event(SelectableUI source) => WriteUnselected(SelectedOptionIndex);

        public Option GetSelectedOption() {
            if (options != null && options.Count > 0)
                return options[SelectedOptionIndex];
            return null;
        }

        public class Option {
            public IEnumerable<Sprite> Sprites;
            public delegate void OptionAction(Menu menu, Option option);
            public OptionAction Action;
        }

        public void ClearOptions() {
            SelectedOptionIndex = 0;
            options.Clear();
        }
        public void RemoveRangeOptions(int index, int count) {
            SelectedOptionIndex = 0;
            options.RemoveRange(index, count);
        }

        public void AddOption(Option option) {
            options.Add(option);
            if (Active) {
                Clear();
                Display();
            }
        }

        public void SetOption(Option option, int index) {
            options[index] = option;
        }

        public void RemoveOption(Option option) {
            options.Remove(option);
            if (SelectedOptionIndex == options.Count)
                SelectedOptionIndex = 0;
            if (Active) {
                Clear();
                Display();
            }
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
            if (options.Count > 0) {
                int offsetY = GetOffsetY();
                Renderer.Display(DeselectedSprite, new Vector2(Position.x, Position.y + index + offsetY));
                return offsetY;
            }
            return 0;
        }

        private void WriteSelected(int previousIndex) {
            if (options.Count > 1) {
                int offsetY = WriteUnselected(previousIndex);
                if (Selected) {
                    Renderer.Display(SelectedSprite, new Vector2(Position.x, Position.y + SelectedOptionIndex + offsetY));
                }
            }
        }

        protected int GetY(int optionIndex) {
            return Position.y + optionIndex + GetOffsetY();
        }
        private void WriteSelected() {
            if (options.Count > 0)
                Renderer.Display(SelectedSprite, new Vector2(Position.x, GetY(SelectedOptionIndex)));
        }

        public void NextOption() {
            int previousIndex = SelectedOptionIndex;
            SelectedOptionIndex++;
            if (SelectedOptionIndex >= options.Count)
                SelectedOptionIndex = 0;
            WriteSelected(previousIndex);
        }

        public void PreviousOption() {
            int previousIndex = SelectedOptionIndex;
            SelectedOptionIndex--;
            if (SelectedOptionIndex < 0)
                SelectedOptionIndex = options.Count - 1;
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
        private int[] clearOptions;
        protected void ClearOption(int optionIndex) {
            Renderer.Display(Sprite.CreateEmptyUI(clearOptions[optionIndex]), new Vector2(Position.x, GetY(optionIndex)));
        }
        protected void DisplayOptions() {
            int count = options.Count;
            clearOptions = new int[count];
            for (int i = 0; i < count; i++) {
                if (Selected && i == SelectedOptionIndex)
                    Renderer.Add(SelectedSprite);
                else
                    Renderer.Add(DeselectedSprite);
                int c = 0;
                foreach (Sprite sprite in options[i].Sprites) {
                    c += sprite.Display.Length;
                    Renderer.Add(sprite);
                }
                clearOptions[i] = c;
                Renderer.Add(Environment.NewLine);
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
