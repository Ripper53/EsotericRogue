using System;

namespace EsotericRogue {
    public class FriendlyUnit : Unit {
        public readonly Menu Menu;

        private GameManager gameManager;
        public FriendlyUnit(Character character) : base(character) {
            Menu = new Menu();
            AddEndOption();
        }
        public FriendlyUnit(Character character, UnitBrain unitBrain) : base(character, unitBrain) {
            Menu = new Menu();
            AddEndOption();
        }
        private void AddEndOption() {
            Menu.AddOption(new Menu.Option() {
                Sprites = new Sprite[] { new Sprite("End", ConsoleColor.Red, ConsoleColor.Black) },
                Action = (menu, op) => {
                    gameManager.RemoveUI(menu);
                    gameManager.PlayerInfo.Input.UIOnly = false;
                    gameManager.PlayerInfo.Input.DeselectUI();
                    Renderer.Clear();
                    gameManager.Display();
                }
            });
        }

        public delegate void InteractedAction(FriendlyUnit friendlyUnit, PlayerUnitBrain playerUnitBrain);
        public event InteractedAction Interacted;
        public void Interact(PlayerUnitBrain playerUnitBrain) {
            gameManager = playerUnitBrain.PlayerInput.GameManager;
            gameManager.AddUI(Menu);
            Menu.Display();
            gameManager.PlayerInfo.Input.UIOnly = true;
            playerUnitBrain.PlayerInput.SelectedUIIndex = gameManager.GetSelectableUIIndex(Menu);
            Interacted?.Invoke(this, playerUnitBrain);
        }
    }
}
