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
            // Set up
            SceneGenerator = new DungeonSceneGenerator(Scene) {
                PlayerUnit = PlayerInfo.Unit
            };
            const int sizeX = 110, sizeY = 35;
            Scene.SetSize(new Vector2(sizeX, sizeY));
            PlayerInfo.InfoUI.Position = new Vector2(sizeX + 2, 0);
            PlayerInfo.InventoryMenu.Position = new Vector2(sizeX + 2, 7);

            bool startGame = false;
            Menu menu = new Menu() {
                Sprites = new Sprite[] {
                    new Sprite("Dungeon Rogue" + Environment.NewLine)
                },
                Position = new Vector2(0, 0)
            };
            menu.AddOption(new Menu.Option() {
                Sprites = new Sprite[] {
                    Sprite.CreateUI("Start")
                },
                Action = (menu, op) => startGame = true
            });
            menu.AddOption(new Menu.Option() {
                Sprites = new Sprite[] {
                    Sprite.CreateUI("Quit")
                },
                Action = (menu, op) => QuitGame = true
            });

            AddUI(menu);
            DisplayUI();
            do {
                PlayerInfo.Input.SelectedUIIndex = 0;
                if (QuitGame)
                    return false;
            } while (!PlayerInfo.Input.UIControls() || !startGame);
            PlayerInfo.Input.DeselectUI();
            RemoveUI(menu);

            startGame = false;
            Menu mapSizeMenu = new Menu() {
                Sprites = new Sprite[] {
                    new Sprite("Map Size" + Environment.NewLine)
                }
            };
            void setMapSize(Vector2 size) {
                Scene.SetSize(size);
                PlayerInfo.InfoUI.Position = new Vector2(size.x + 2, 0);
                PlayerInfo.InventoryMenu.Position = new Vector2(size.x + 2, 7);
                startGame = true;
            }
            Menu.Option.OptionAction setSizeAction(Vector2 size) {
                return (menu, op) => setMapSize(size);
            }
            void createMapSizeOption(Vector2 size) {
                mapSizeMenu.AddOption(new Menu.Option() {
                    Sprites = new Sprite[] {
                        Sprite.CreateUI(size.ToString())
                    },
                    Action = setSizeAction(size)
                });
            }
            createMapSizeOption(new Vector2(110, 35));
            createMapSizeOption(new Vector2(60, 20));
            createMapSizeOption(new Vector2(35, 35));
            mapSizeMenu.AddOption(new Menu.Option() {
                Sprites = new Sprite[] {
                    Sprite.CreateUI("Custom")
                },
                Action = (menu, op) => {
                    string x, y;
                    int xSize, ySize;
                    do {
                        Renderer.Clear();
                        Renderer.Add(Sprite.CreateUI("X: "));
                        Renderer.Display();
                        x = Renderer.ReadLine();
                        Renderer.Add(Sprite.CreateUI("Y: "));
                        Renderer.Display();
                        y = Renderer.ReadLine();
                    } while (!int.TryParse(x, out xSize) || !int.TryParse(y, out ySize));
                    const int minSize = 2, maxSize = 200;
                    if (xSize < minSize)
                        xSize = minSize;
                    else if (xSize > maxSize)
                        xSize = maxSize;
                    if (ySize < minSize)
                        ySize = minSize;
                    else if (ySize > maxSize)
                        ySize = maxSize;
                    setMapSize(new Vector2(xSize, ySize));
                }
            });

            AddUI(mapSizeMenu);
            Renderer.Clear();
            DisplayUI();
            do {
                PlayerInfo.Input.SelectedUIIndex = 0;
            } while (!PlayerInfo.Input.UIControls() || !startGame);
            RemoveUI(mapSizeMenu);

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
