using System;
using System.Collections.Generic;
using System.Text;

namespace EsotericRogue {
    public class InventoryMenu : Menu {
        public readonly GameManager GameManager;
        public readonly Character Character;
        private readonly Sprite nameSprite, goldSprite;
        private readonly Dictionary<Item, Option> itemOps;
        private readonly Dictionary<Weapon, Option> weaponOps;
        private readonly Dictionary<Potion, Option> potionOps;
        private readonly Menu itemsMenu, weaponsMenu, potionsMenu;

        private Menu previousMenu;
        private Vector2 MenuPosition => new Vector2(Position.x, Position.y + 5);
        private void DisplayMenuEvent(Menu menu) {
            if (previousMenu != null) {
                previousMenu.Clear();
                GameManager.RemoveUI(previousMenu);
            }
            previousMenu = menu;
            GameManager.AddUI(menu);
            menu.Position = MenuPosition;
            menu.Display();

            GameManager.PlayerInfo.Input.SelectedUIIndex = GameManager.GetSelectableUIIndex(menu);
        }
        private void CloseMenu(Menu menu, Option op) {
            if (previousMenu == menu)
                previousMenu = null;
            menu.Clear();
            GameManager.RemoveUI(menu);
        }

        public InventoryMenu(GameManager gameManager, Character character) {
            GameManager = gameManager;
            Character = character;
            nameSprite = new Sprite(string.Empty);
            goldSprite = Sprite.CreateUI(string.Empty);

            Sprites = new Sprite[] {
                nameSprite,
                new Sprite(" Inventory" + Environment.NewLine),
                Sprite.CreateUI("Gold: "),
                goldSprite
            };
            itemOps = new Dictionary<Item, Option>();
            weaponOps = new Dictionary<Weapon, Option>();
            potionOps = new Dictionary<Potion, Option>();
            itemsMenu = new Menu() {
                Sprites = new Sprite[] {
                    new Sprite("Items  " + Environment.NewLine)
                }
            };
            weaponsMenu = new Menu() {
                Sprites = new Sprite[] {
                    new Sprite("Weapons" + Environment.NewLine)
                }
            };
            potionsMenu = new Menu() {
                Sprites = new Sprite[] {
                    new Sprite("Potions" + Environment.NewLine)
                }
            };

            Option closeOption = new Option() {
                Sprites = new Sprite[] {
                    new Sprite("Close", ConsoleColor.Red, ConsoleColor.Black)
                },
                Action = CloseMenu
            };
            itemsMenu.AddOption(closeOption);
            weaponsMenu.AddOption(closeOption);
            potionsMenu.AddOption(closeOption);

            AddOption(new Option() {
                Sprites = new Sprite[] {
                    Sprite.CreateUI("Items  ")
                },
                Action = (menu, op) => DisplayMenuEvent(itemsMenu)
            });
            AddOption(new Option() {
                Sprites = new Sprite[] {
                    Sprite.CreateUI("Weapons")
                },
                Action = (menu, op) => DisplayMenuEvent(weaponsMenu)
            });
            AddOption(new Option() {
                Sprites = new Sprite[] {
                    Sprite.CreateUI("Potions")
                },
                Action = (menu, op) => DisplayMenuEvent(potionsMenu)
            });

            Activated += source => {
                // Clear inventory menu
                if (itemsMenu.Options.Count > 1)
                    itemsMenu.RemoveRangeOptions(1, itemsMenu.Options.Count - 1);
                itemOps.Clear();
                if (weaponsMenu.Options.Count > 1)
                    weaponsMenu.RemoveRangeOptions(1, weaponsMenu.Options.Count - 1);
                weaponOps.Clear();
                if (potionsMenu.Options.Count > 1)
                    potionsMenu.RemoveRangeOptions(1, potionsMenu.Options.Count - 1);
                potionOps.Clear();
                // Add inventory to menu
                foreach (Item item in Character.Inventory)
                    AddOption(Character.Inventory, item);

                Character.Inventory.AddedItem += AddOption;
                Character.Inventory.RemovedItem += Inventory_RemovedItem;
            };
            Deactivated += source => {
                Character.Inventory.AddedItem -= AddOption;
                Character.Inventory.RemovedItem -= Inventory_RemovedItem;
            };
        }

        #region Events
        private void AddOption(Inventory inventory, Item item) {
            Option op = new Option() {
                Sprites = new Sprite[] {
                    Sprite.CreateUI(GetContinuedString(item.Name, 12))
                },
                Action = (menu, op) => item.Use(inventory.Character)
            };
            if (item is Weapon weapon) {
                weaponOps.Add(weapon, op);
                weaponsMenu.AddOption(op);
            } else if (item is Potion potion) {
                potionOps.Add(potion, op);
                potionsMenu.AddOption(op);
            } else {
                itemOps.Add(item, op);
                itemsMenu.AddOption(op);
            }
        }
        private void RemoveOption(Item item) {
            if (item is Weapon weapon) {
                weaponsMenu.RemoveOption(weaponOps[weapon]);
                weaponOps.Remove(weapon);
            } else if (item is Potion potion) {
                potionsMenu.RemoveOption(potionOps[potion]);
                potionOps.Remove(potion);
            } else {
                itemsMenu.RemoveOption(itemOps[item]);
                itemOps.Remove(item);
            }
        }

        private void Inventory_RemovedItem(Inventory inventory, Item removedItem) {
            RemoveOption(removedItem);
        }
        #endregion

        protected override void DisplayUI() {
            string name = GetContinuedString(Character.Name, 12);
            StringBuilder stringBuilder = new StringBuilder(name, name.Length + 2);
            stringBuilder.Append('\'');
            if (!Character.Name.EndsWith('s')) {
                stringBuilder.Append('s');
            }
            nameSprite.Display = stringBuilder.ToString();
            stringBuilder.Clear();
            stringBuilder.Append(Character.Inventory.Gold);
            stringBuilder.Append(Environment.NewLine);
            goldSprite.Display = stringBuilder.ToString();

            itemsMenu.Position = MenuPosition;
            weaponsMenu.Position = MenuPosition;
            potionsMenu.Position = MenuPosition;

            base.DisplayUI();
        }

        public override void Clear() {
            base.Clear();
            if (previousMenu != null)
                previousMenu.Clear();
        }

    }
}
