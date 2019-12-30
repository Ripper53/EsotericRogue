using System;

namespace EsotericRogue {
    public class PlayerInput {
        public enum InputFocus {
            Unit, UI
        };
        public InputFocus Focus { get; private set; }
        public bool UIOnly = false;
        private SelectableUI selectableUI;
        public SelectableUI SelectedUI {
            get => selectableUI;
            private set {
                if (value == null && UIOnly)
                    return;
                if (selectableUI != null)
                    selectableUI.Selected = false;
                selectableUI = value;
                if (selectableUI != null) {
                    Focus = InputFocus.UI;
                    selectableUI.Selected = true;
                } else if (UIOnly) {
                    Focus = InputFocus.UI;
                    selectableUI = GameManager.GetFirstSelectableUI();
                    if (selectableUI == null) {
                        UIOnly = false;
                        Focus = InputFocus.Unit;
                    } else {
                        selectableUI.Selected = true;
                    }
                } else {
                    Focus = InputFocus.Unit;
                }
            }
        }
        private int selectedUIIndex;
        public int SelectedUIIndex {
            get => selectedUIIndex;
            set {
                selectedUIIndex = value;
                if (selectedUIIndex < 0) {
                    selectedUIIndex = GameManager.SelectableUICount - 1;
                    // If the Count is already 0, then subtracting 1 will make it -1
                    if (selectedUIIndex < 0)
                        selectedUIIndex = 0;
                } else if (selectedUIIndex >= GameManager.SelectableUICount) {
                    selectedUIIndex = 0;
                }

                SelectedUI = selectedUIIndex < GameManager.SelectableUICount ? GameManager.selectableUIs[selectedUIIndex] : null;
            }
        }
        public void DeselectUI() {
            SelectedUI = null;
        }
        public readonly GameManager GameManager;
        public readonly PlayerUnitBrain PlayerUnitBrain;
        public ConsoleKeyInfo LatestKeyInfo { get; private set; }
        public bool ValidInput { get; private set; }

        public PlayerInput(GameManager gameManager, PlayerUnitBrain brain) {
            GameManager = gameManager;
            Focus = InputFocus.Unit;
            PlayerUnitBrain = brain;
            PlayerUnitBrain.PlayerInput = this;
        }

        public ConsoleKeyInfo GetInput() {
            LatestKeyInfo = Renderer.ReadKey();
            return LatestKeyInfo;
        }

        public void UnitControls() {
            ValidInput = false;
            PlayerUnitBrain.Controls();
            if (!PlayerUnitBrain.ValidInput) {
                switch (LatestKeyInfo.Key) {
                    case ConsoleKey.Tab:
                        SelectedUI = GameManager.GetFirstSelectableUI();
                        ValidInput = true;
                        return;
                }
            }
        }

        public bool UIControls() {
            while (true) {
                ConsoleKeyInfo key = GetInput();
                switch (key.Key) {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        SelectedUI.Up();
                        return true;
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.D:
                        SelectedUI.Right();
                        return true;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        SelectedUI.Down();
                        return true;
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.A:
                        SelectedUI.Left();
                        return true;
                    case ConsoleKey.Enter:
                        SelectedUI.Enter();
                        return true;
                    case ConsoleKey.Tab:
                        if (key.Modifiers.HasFlag(ConsoleModifiers.Shift))
                            SelectedUIIndex--;
                        else
                            SelectedUIIndex++;
                        return true;
                    case ConsoleKey.Q:
                        DeselectUI();
                        return true;
                }
            }
        }
    }
}
