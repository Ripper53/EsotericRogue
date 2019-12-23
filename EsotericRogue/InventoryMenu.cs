using System;
using System.Collections.Generic;
using System.Text;

namespace EsotericRogue {
    public class InventoryMenu : Menu {
        public readonly GameManager GameManager;
        public readonly Character Character;
        private readonly Sprite nameSprite, goldSprite;
        private readonly ItemsMenu<Item> itemsMenu;
        private readonly ItemsMenu<Potion> potionsMenu;
        private readonly ItemStorage<Weapon> weaponStorage;
        private readonly ItemStorage<Boot> bootStorage;
        private readonly ItemStorage<Chestplate> chestplateStorage;
        private readonly ItemStorage<Sleeve> sleeveStorage;
        private readonly ItemStorage<Pants> pantsStorage;

        #region Classes
        private class ItemsMenu : Menu {
            public new int ClearPosition;
            public int ClearLength;
            public Vector2 ClearItemPosition => Position + new Vector2(1, ClearPosition);

            public sealed override void Clear() {
                base.Clear();
                ClearSelectedItem();
            }
            public void ClearSelectedItem() {
                Renderer.Display(Sprite.CreateEmptyUI(ClearLength), ClearItemPosition);
            }
        }
        private class ItemsMenu<T> : ItemsMenu where T : Item {
            private readonly Dictionary<T, Option> itemOptions = new Dictionary<T, Option>();

            public void AddOption(T item, Option option) {
                itemOptions.Add(item, option);
                AddOption(option);
            }
            public void RemoveOption(T item) {
                RemoveOption(itemOptions[item]);
                itemOptions.Remove(item);
            }

            public new void ClearOptions() => itemOptions.Clear();
        }

        private class ItemStorage<T> where T : EquippableItem {
            private readonly Dictionary<T, Item> dict;
            public readonly ItemsMenu Menu;

            private readonly Equipment<T> equipment;

            public ItemStorage(string name, Equipment<T> equipment) {
                dict = new Dictionary<T, Item>();
                Menu = new ItemsMenu() {
                    Sprites = new Sprite[] {
                        new Sprite(name + Environment.NewLine)
                    }
                };
                this.equipment = equipment;
            }

            private class Item {
                public readonly Option Option;
                public readonly Sprite Sprite;
                public readonly Equipment<T>.ItemEquippedAction EquippedAction;
                public Item(Option option, Sprite sprite, Equipment<T>.ItemEquippedAction equippedAction) {
                    Option = option;
                    Sprite = sprite;
                    EquippedAction = equippedAction;
                }
            }

            public void Clear() => dict.Clear();

            public void AddOption(T equippableItem, Option option, Sprite sprite) {
                const string equipPrefix = "E";
                if (equippableItem == equipment.Equipped) {
                    // If item is already equipped, show it is in the menu.
                    sprite.Display = GetPrefixedItemName(equippableItem, equipPrefix);
                    Menu.ClearLength = sprite.Display.Length;
                    Menu.ClearPosition = Menu.Options.Count + 1;
                }
                void ItemEquippedAction(Character source, T item, T oldItem) {
                    if (item != equippableItem) return;
                    Item stored = dict[oldItem];
                    Menu.ClearSelectedItem();
                    stored.Sprite.Display = GetItemName(item);
                    Renderer.Display(stored.Sprite, Menu.ClearItemPosition);

                    Menu.ClearPosition = Menu.SelectedOptionIndex + 1;
                    sprite.Display = GetPrefixedItemName(item, equipPrefix);
                    Menu.ClearLength = sprite.Display.Length;
                    Renderer.Display(sprite, Menu.ClearItemPosition);
                }
                equipment.ItemEquipped += ItemEquippedAction;
                dict.Add(equippableItem, new Item(
                    option,
                    sprite,
                    ItemEquippedAction
                ));
                Menu.AddOption(option);
            }

            public void Remove(T key) {
                Item item = dict[key];
                dict.Remove(key);
                Menu.RemoveOption(item.Option);
                equipment.ItemEquipped -= item.EquippedAction;
            }
        }
        #endregion

        private Menu previousMenu;
        private Vector2 MenuPosition => new Vector2(Position.x, Position.y + 10);
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

            const string
                itemsTitle =       "Items       ",
                potionsTitle =     "Potions     ",
                weaponsTitle =     "Weapons     ",
                bootsTitle =       "Boots       ",
                chestplatesTitle = "Chestplates ",
                sleevesTitle =     "Sleeves     ",
                pantsTitle =       "Pants       ";
            Sprites = new Sprite[] {
                nameSprite,
                new Sprite(" Inventory" + Environment.NewLine),
                Sprite.CreateUI("Gold: "),
                goldSprite
            };
            itemsMenu = new ItemsMenu<Item>() {
                Sprites = new Sprite[] {
                    new Sprite(itemsTitle + Environment.NewLine)
                }
            };
            potionsMenu = new ItemsMenu<Potion>() {
                Sprites = new Sprite[] {
                    new Sprite(potionsTitle + Environment.NewLine)
                }
            };
            weaponStorage = new ItemStorage<Weapon>(weaponsTitle, character.Weapon);
            bootStorage = new ItemStorage<Boot>(bootsTitle, character.Boot);
            chestplateStorage = new ItemStorage<Chestplate>(chestplatesTitle, character.Chestplate);
            sleeveStorage = new ItemStorage<Sleeve>(sleevesTitle, character.Sleeve);
            pantsStorage = new ItemStorage<Pants>(pantsTitle, character.Pants);

            Option closeOption = new Option() {
                Sprites = new Sprite[] {
                    new Sprite("Close", ConsoleColor.Red, ConsoleColor.Black)
                },
                Action = CloseMenu
            };
            itemsMenu.AddOption(closeOption);
            potionsMenu.AddOption(closeOption);
            weaponStorage.Menu.AddOption(closeOption);
            bootStorage.Menu.AddOption(closeOption);
            chestplateStorage.Menu.AddOption(closeOption);
            sleeveStorage.Menu.AddOption(closeOption);
            pantsStorage.Menu.AddOption(closeOption);

            AddOption(new Option() {
                Sprites = new Sprite[] {
                    Sprite.CreateUI(itemsTitle)
                },
                Action = (menu, op) => DisplayMenuEvent(itemsMenu)
            });
            AddOption(new Option() {
                Sprites = new Sprite[] {
                    Sprite.CreateUI(potionsTitle)
                },
                Action = (menu, op) => DisplayMenuEvent(potionsMenu)
            });
            AddOption(new Option() {
                Sprites = new Sprite[] {
                    Sprite.CreateUI(weaponsTitle)
                },
                Action = (menu, op) => DisplayMenuEvent(weaponStorage.Menu)
            });
            AddOption(new Option() {
                Sprites = new Sprite[] {
                    Sprite.CreateUI(bootsTitle)
                },
                Action = (menu, op) => DisplayMenuEvent(bootStorage.Menu)
            });
            AddOption(new Option() {
                Sprites = new Sprite[] {
                    Sprite.CreateUI(chestplatesTitle)
                },
                Action = (menu, op) => DisplayMenuEvent(chestplateStorage.Menu)
            });
            AddOption(new Option() {
                Sprites = new Sprite[] {
                    Sprite.CreateUI(sleevesTitle)
                },
                Action = (menu, op) => DisplayMenuEvent(sleeveStorage.Menu)
            });
            AddOption(new Option() {
                Sprites = new Sprite[] {
                    Sprite.CreateUI(pantsTitle)
                },
                Action = (menu, op) => DisplayMenuEvent(pantsStorage.Menu)
            });

            Activated += source => {
                static void MenuClear(Menu menu) {
                    const int index = 1;
                    if (menu.Options.Count > index)
                        menu.RemoveRangeOptions(index, menu.Options.Count - index);
                }
                // Clear inventory menu
                MenuClear(itemsMenu);
                itemsMenu.ClearOptions();
                MenuClear(potionsMenu);
                potionsMenu.ClearOptions();
                MenuClear(weaponStorage.Menu);
                weaponStorage.Clear();
                MenuClear(bootStorage.Menu);
                bootStorage.Clear();
                MenuClear(chestplateStorage.Menu);
                chestplateStorage.Clear();
                MenuClear(sleeveStorage.Menu);
                sleeveStorage.Clear();
                MenuClear(pantsStorage.Menu);
                pantsStorage.Clear();

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
        private static string GetItemName(Item item) {
            return GetContinuedString(item.Name, itemNameLength);
        }
        private static string GetPrefixedItemName(Item item, string prefix) {
            return GetContinuedString($"[{prefix}]{item.Name}", itemNameLength);
        }

        private void AddOption(Inventory inventory, Item item) {
            Sprite sprite = Sprite.CreateUI(GetItemName(item));
            Option op = new Option() {
                Sprites = new Sprite[] {
                    sprite
                },
                Action = (menu, op) => item.Use(inventory.Character)
            };
            if (item is Potion potion) {
                potionsMenu.AddOption(potion, op);
            } else if (item is Weapon weapon) {
                weaponStorage.AddOption(weapon, op, sprite);
            } else if (item is Boot boot) {
                bootStorage.AddOption(boot, op, sprite);
            } else if (item is Chestplate chestplate) {
                chestplateStorage.AddOption(chestplate, op, sprite);
            } else if (item is Sleeve sleeve) {
                sleeveStorage.AddOption(sleeve, op, sprite);
            } else if (item is Pants pants) {
                pantsStorage.AddOption(pants, op, sprite);
            } else {
                itemsMenu.AddOption(item, op);
            }
        }
        private void RemoveOption(Item item) {
            if (item is Potion potion) {
                potionsMenu.RemoveOption(potion);
            } else if (item is Weapon weapon) {
                weaponStorage.Remove(weapon);
            } else if (item is Boot boot) {
                bootStorage.Remove(boot);
            } else if (item is Chestplate chestplate) {
                chestplateStorage.Remove(chestplate);
            } else if (item is Sleeve sleeve) {
                sleeveStorage.Remove(sleeve);
            } else if (item is Pants pants) {
                pantsStorage.Remove(pants);
            } else {
                itemsMenu.RemoveOption(item);
            }
        }

        private void Inventory_RemovedItem(Inventory inventory, Item removedItem) {
            RemoveOption(removedItem);
        }
        #endregion

        private const int itemNameLength = 12;
        protected override void DisplayUI() {
            string name = GetContinuedString(Character.Name, itemNameLength);
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
            weaponStorage.Menu.Position = MenuPosition;
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
