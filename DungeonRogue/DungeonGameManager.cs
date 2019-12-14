using System;
using EsotericRogue;

namespace DungeonRogue {
    public class DungeonGameManager : GameManager {
        public bool QuitGame { get; private set; }

        public DungeonGameManager() : base(
            new Unit(new Character(10, new PlayerCharacterBrain(), new BareWeapon()), new PlayerUnitBrain()) {
                Sprite = new Sprite("@", ConsoleColor.Magenta)
            }
        ) {
        
        }

        protected override bool Start() {
            bool startGame = false;
            Menu menu = new Menu() {
                Sprites = new Sprite[] {
                    new Sprite("Dungeon Rogue" + Environment.NewLine)
                },
                Options = new Menu.Option[] {
                    new Menu.Option() {
                        Sprites = new Sprite[] {
                            UI.CreateSprite("Start")
                        },
                        Action = (menu, op) => startGame = true
                    },
                    new Menu.Option() {
                        Sprites = new Sprite[] {
                            UI.CreateSprite("Quit")
                        },
                        Action = (menu, op) => QuitGame = true
                    }
                },
                Position = new Vector2(0, 0)
            };

            menu.Display();
            do {
                PlayerInfo.Input.SelectedUI = menu;
                if (QuitGame)
                    return false;
            } while (!PlayerInfo.Input.UIControls() || !startGame);
            PlayerInfo.Input.SelectedUI = null;

            Renderer.Clear();
            Renderer.Display(new Sprite("Enter Name:" + Environment.NewLine), new Vector2(0, 0));
            string name = Renderer.ReadLine();
            if (name == "")
                name = "You";
            PlayerInfo.Character.Name = name;
            return true;
        }
    }
}
