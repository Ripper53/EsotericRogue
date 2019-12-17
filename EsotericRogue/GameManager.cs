using System.Collections.Generic;

namespace EsotericRogue {
    public abstract class GameManager {
        public readonly PlayerInfo PlayerInfo;
        public readonly Scene Scene;
        public SceneGenerator SceneGenerator;
        private readonly List<UI> uis;
        internal readonly List<SelectableUI> selectableUIs;
        public int SelectableUICount => selectableUIs.Count;
        private readonly BattleMenu battleMenu;

        public GameManager(Unit playerUnit) {
            uis = new List<UI>();
            selectableUIs = new List<SelectableUI>();
            Scene = new Scene();
            PlayerInfo = new PlayerInfo(this, playerUnit);
            battleMenu = new BattleMenu(this, PlayerInfo);
        }

        public bool AddUI(UI ui) {
            if (ui.Active)
                return false;
            ui.Active = true;
            uis.Add(ui);
            if (ui is SelectableUI selectableUI)
                selectableUIs.Add(selectableUI);
            return true;
        }

        public bool RemoveUI(UI ui) {
            if (!ui.Active)
                return false;
            ui.Active = false;
            uis.Remove(ui);
            if (ui is SelectableUI selectableUI) {
                selectableUIs.Remove(selectableUI);
                if (PlayerInfo.Input.SelectedUI == selectableUI)
                    PlayerInfo.Input.SelectedUIIndex--;
            }
            return true;
        }

        public SelectableUI GetFirstSelectableUI() {
            if (SelectableUICount > 0)
                return selectableUIs[0];
            return null;
        }

        public int GetSelectableUIIndex(SelectableUI selectableUI) {
            return selectableUIs.FindIndex(v => selectableUI == v);
        }

        /// <summary>
        /// Return true to start game, otherwise false to quit.
        /// </summary>
        protected abstract bool Start();

        private void Generate() {
            SceneGenerator.Generate();
            // Render
            Display();
        }

        public void Display() {
            Scene.Display();
            DisplayUI();
        }
        public void DisplayUI() {
            foreach (UI ui in uis) {
                ui.Display();
            }
        }

        private readonly Queue<Character> toBattle = new Queue<Character>();
        public void Battle(UnitBrain brain) {
            Scene.DestroyUnit(brain.Unit);
            toBattle.Enqueue(brain.Unit.Character);
        }

        public void Run() {
            Renderer.Clear();
            if (!Start()) return;

            AddUI(PlayerInfo.InfoUI);
            AddUI(PlayerInfo.InventoryMenu);

            Generate();
            while (true) {

                // Player
                bool playerInputValid;
                do {
                    PlayerInput playerInput = PlayerInfo.Input;
                    switch (playerInput.Focus) {
                        case PlayerInput.InputFocus.Unit:
                            playerInput.UnitControls();
                            PlayerUnitBrain brain = playerInput.PlayerUnitBrain;
                            playerInputValid = brain.ValidInput || playerInput.ValidInput;
                            // If Player made a valid move.
                            if (brain.ValidInput) {
                                if (Scene.GetTile(brain.Unit.Position) == Scene.Tile.Exit) {
                                    // If Player reached exit, generate new dungeon.
                                    Generate();
                                } else {
                                    // If Player moved to free spot.
                                    foreach (Unit unit in new List<Unit>(Scene.Units)) {
                                        // If Unit has a brain and it is not the Player's brain, execute controls.
                                        if (unit.Brain != null && unit.Brain != brain)
                                            unit.Brain.Controls();
                                    }
                                }
                                // If any enemies were met, battle them.
                                while (toBattle.Count > 0) {
                                    if (!battleMenu.Battle(toBattle.Dequeue())) {
                                        // Death! Game Over!
                                        return;
                                    }
                                }
                            }
                            break;
                        default:
                            playerInputValid = playerInput.UIControls();
                            break;
                    }
                } while (!playerInputValid);

            }
        }

    }
}
