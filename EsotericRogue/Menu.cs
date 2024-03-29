﻿using System;
using System.Collections.Generic;

namespace EsotericRogue {
    public class Menu : SelectableUI {
        public IEnumerable<Sprite> Sprites;
        private readonly List<Option> options;
        public IReadOnlyList<Option> Options => options;
        public delegate void SelectedOptionIndexChangedAction(Menu source, int selectedOptionIndex, int oldSelectedOptionIndex);
        public event SelectedOptionIndexChangedAction SelectedOptionIndexChanged;
        private int selectedOptionIndex;
        public int SelectedOptionIndex {
            get => selectedOptionIndex;
            set {
                int oldSelectedOptionIndex = selectedOptionIndex;
                selectedOptionIndex = value;
                SelectedOptionIndexChanged?.Invoke(this, selectedOptionIndex, oldSelectedOptionIndex);
            }
        }

        public Menu() {
            options = new List<Option>();

            OnSelected += source => WriteSelected();
            OnDeselected += source => WriteUnselected(SelectedOptionIndex);
        }

        public Option GetSelectedOption() {
            if (options != null && options.Count > 0)
                return options[SelectedOptionIndex];
            return null;
        }

        public class Option {
            public IEnumerable<Sprite> Sprites;
            public delegate void OptionAction(Menu menu, Option option);
            public OptionAction Action;
            public int Index { get; internal set; }
        }

        public void ClearOptions() {
            SelectedOptionIndex = 0;
            options.Clear();
        }
        public void RemoveRangeOptions(int index, int count) {
            options.RemoveRange(index, count);
            DisplayRemoved();
        }
        private void DisplayRemoved() {
            if (SelectedOptionIndex >= options.Count)
                SelectedOptionIndex = 0;
            if (Active) {
                Clear();
                Display();
            }
        }

        public void AddOption(Option option) {
            option.Index = options.Count;
            options.Add(option);
            if (Active) {
                Clear();
                Display();
            }
        }

        public void SetOption(Option option, int index) {
            option.Index = index;
            options[index] = option;
        }

        public void RemoveOption(Option option) {
            options.Remove(option);
            DisplayRemoved();
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

        private int GetY(int optionIndex) {
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
        protected Vector2 GetOptionPosition(int optionIndex) => new Vector2(Position.x + 1, GetY(optionIndex));

        protected int[] clearOptions;
        protected void ClearOption(int optionIndex) {
            Renderer.Display(Sprite.CreateEmptyUI(clearOptions[optionIndex]), GetOptionPosition(optionIndex));
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
