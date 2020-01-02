using System;
using EsotericRogue;
using DungeonRogue.Weapons;
using DungeonRogue.Boots;
using DungeonRogue.Chestplates;
using DungeonRogue.Sleeves;
using DungeonRogue.Pants;
using DungeonRogue.Ammunition;

namespace DungeonRogue {
    public class DungeonGameManager : GameManager {
        public bool QuitGame { get; private set; }

        public DungeonGameManager() : base(
            new Unit(new Character(10, new PlayerCharacterBrain(), new BareWeapon(), new BareBoot(), new BareChestplate(), new BareSleeve(), new BarePants()), new PlayerUnitBrain()) {
                Sprite = new Sprite("@", ConsoleColor.Magenta)
            }
        ) {
            PlayerInfo.Character.Stamina.IncreaseMax(5);
            PlayerInfo.Character.Stamina.Regen = 1;
            PlayerInfo.Character.Inventory.AddItem(new WoodenBowWeapon());
            for (int i = 0; i < 5; i++)
                PlayerInfo.Character.Inventory.AddItem(new Arrow());

            PlayerInfo.Character.Inventory.RemoveItems<Arrow>(1);
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
                Vector2 bufferSize = new Vector2(size.x + 20, size.y);
                if (bufferSize.x < 120)
                    bufferSize.x = 120;
                if (bufferSize.y < 20)
                    bufferSize.y = 20;
                Renderer.BufferWidth = bufferSize.x;
                Renderer.BufferHeight = bufferSize.y;
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
