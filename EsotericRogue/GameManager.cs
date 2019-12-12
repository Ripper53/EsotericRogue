using System.Collections.Generic;

namespace EsotericRogue {
    public abstract class GameManager {
        public readonly PlayerInfo PlayerInfo;
        public readonly Scene Scene;
        public SceneGenerator SceneGenerator;
        private readonly List<UI> uis;
        internal readonly List<SelectableUI> selectableUIs;
        private readonly BattleMenu battleMenu;

        public GameManager(Unit playerUnit) {
            Scene = new Scene();
            PlayerInfo = new PlayerInfo(this, playerUnit);
            uis = new List<UI>();
            selectableUIs = new List<SelectableUI>();
            battleMenu = new BattleMenu(this, playerUnit.Character);
        }

        public void AddUI(UI ui) {
            uis.Add(ui);
            if (ui is SelectableUI selectableUI)
                selectableUIs.Add(selectableUI);
        }

        public void RemoveUI(UI ui) {
            uis.Remove(ui);
            if (ui is SelectableUI selectableUI) {
                selectableUIs.Remove(selectableUI);
                if (PlayerInfo.Input.SelectedUI == selectableUI)
                    PlayerInfo.Input.SelectedUI = GetFirstSelectableUI();
            }
        }

        public SelectableUI GetFirstSelectableUI() {
            if (selectableUIs.Count > 0)
                return selectableUIs[0];
            return null;
        }

        protected abstract void Start();

        private void Generate() {
            SceneGenerator.Generate();
            // Render
            Display();
        }

        public void Display() {
            Scene.Display();
            PlayerInfo.InfoUI.Display();

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
            Start();

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
                                    battleMenu.Battle(toBattle.Dequeue());
                                    if (brain.Unit.Character.Health <= 0) {
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
