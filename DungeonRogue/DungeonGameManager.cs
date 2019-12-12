using System;
using EsotericRogue;

namespace DungeonRogue {
    public class DungeonGameManager : GameManager {
        public DungeonGameManager() : base(
            new Unit(new Character(10, new PlayerCharacterBrain(), new BareWeapon()), new PlayerUnitBrain()) {
                Sprite = new Sprite("@", ConsoleColor.Magenta)
            }
        ) {
        
        }

        protected override void Start() {
            bool startGame = false;
            Menu menu = new Menu() {
                Sprites = new Sprite[] {
                    new Sprite("Dungeon Rogue\n")
                },
                Options = new Menu.Option[] {
                    new Menu.Option() {
                        Sprites = new Sprite[] {
                            new Sprite("Start", ConsoleColor.White, ConsoleColor.Black)
                        },
                        Action = (menu, op) => startGame = true
                    }
                },
                Position = new Vector2(0, 0)
            };

            menu.Display();
            do {
                PlayerInfo.Input.SelectedUI = menu;
            } while (!PlayerInfo.Input.UIControls() || !startGame);
            PlayerInfo.Input.SelectedUI = null;

            Renderer.Clear();
            Renderer.Display(new Sprite("Enter Name:" + Environment.NewLine), new Vector2(0, 0));
            string name = Renderer.ReadLine();
            if (name == "")
                name = "You";
            PlayerInfo.Input.PlayerUnitBrain.Unit.Character.Name = name;
        }
    }
}
