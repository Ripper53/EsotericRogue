using System;

namespace EsotericRogue {
    public class PlayerInput {
        public enum InputFocus {
            Unit, UI
        };
        public InputFocus Focus { get; private set; }
        private SelectableUI selectableUI;
        public SelectableUI SelectedUI {
            get => selectableUI;
            set {
                if (selectableUI != null)
                    selectableUI.Selected = false;
                selectableUI = value;
                if (selectableUI != null) {
                    Focus = InputFocus.UI;
                    selectableUI.Selected = true;
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
                if (selectedUIIndex < 0)
                    selectedUIIndex = GameManager.selectableUIs.Count - 1;
                else if (selectedUIIndex >= GameManager.selectableUIs.Count)
                    selectedUIIndex = 0;

                SelectedUI = GameManager.selectableUIs[selectedUIIndex];
            }
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
            LatestKeyInfo = Console.ReadKey(true);
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
                switch (GetInput().Key) {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        SelectedUI.Up();
                        return true;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        SelectedUI.Down();
                        return true;
                    case ConsoleKey.Enter:
                        SelectedUI.Enter();
                        return true;
                    case ConsoleKey.Tab:
                        int previousSelectedUIIndex = SelectedUIIndex;
                        SelectedUIIndex++;
                        if (previousSelectedUIIndex == SelectedUIIndex)
                            SelectedUI = null;
                        return true;
                }
            }
        }
    }
}
