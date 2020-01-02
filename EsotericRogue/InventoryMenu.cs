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

        private const int maxWidth = 20;

        #region Classes
        private abstract class ItemsMenu : Menu {
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
            private readonly InventoryMenu inventoryMenu;
            private readonly Character character;
            private readonly Dictionary<string, List<ItemOption>> itemOptions;
            public int Count(T item) => itemOptions[item.Name].Count;

            public ItemsMenu(Character character, InventoryMenu inventoryMenu) {
                itemOptions = new Dictionary<string, List<ItemOption>>();
                this.character = character;
                this.inventoryMenu = inventoryMenu;
            }

            private class ItemOption {
                public readonly T Item;
                public readonly Option Option;
                public readonly Sprite Sprite;
                public ItemOption(T item, Option option, Sprite sprite) {
                    Item = item;
                    Option = option;
                    Sprite = sprite;
                }
            }

            private void DisplayOption(int optionIndex) {
                ClearOption(optionIndex);
                Vector2 pos = new Vector2(Position.x + 1, GetY(optionIndex));
                foreach (Sprite sprite in Options[optionIndex].Sprites) {
                    Renderer.Display(sprite, pos);
                    pos = Renderer.CursorPosition;
                }
            }
            public Sprite AddOption(T item) {
                string key = item.Name;
                Sprite sprite;
                if (itemOptions.ContainsKey(key)) {
                    // Item already exists in inventory.
                    List<ItemOption> itemOptions = this.itemOptions[key];
                    Option option = null;
                    // Get already created option.
                    foreach (ItemOption itemOption in itemOptions) {
                        if (itemOption.Item.Name == key)
                            option = itemOption.Option;
                    }
                    int optionIndex = 0;
                    for (int i = 1, count = Options.Count; i < count; i++) {
                        if (Options[i] == option)
                            optionIndex = i;
                    }
                    sprite = itemOptions[0].Sprite;
                    itemOptions.Add(new ItemOption(item, option, sprite));
                    sprite.Display = GetItemName(item, itemOptions.Count);
                    option.Sprites = itemOptions[0].Option.Sprites;
                    if (Active)
                        DisplayOption(optionIndex);
                } else {
                    // This is a new item in the inventory.
                    Option option = new Option() {
                        Action = (menu, op) => itemOptions[key][0].Item.Use(character)
                    };
                    sprite = Sprite.CreateUI(GetItemName(item));
                    itemOptions.Add(key, new List<ItemOption>(1) { new ItemOption(item, option, sprite) });
                    AddOption(option);
                    option.Sprites = new Sprite[] { sprite };
                    if (Active)
                        DisplayOption(Options.Count - 1);
                }
                return sprite;
            }
            public void RemoveOption(T item) {
                string key = item.Name;
                List<ItemOption> itemOptions = this.itemOptions[key];
                if (itemOptions.Count > 1) {
                    itemOptions.RemoveAt(itemOptions.FindIndex(v => v.Item == item));
                    itemOptions[0].Sprite.Display = GetItemName(item, itemOptions.Count);
                } else {
                    this.itemOptions.Remove(key);
                    RemoveOption(itemOptions[0].Option);
                }
                if (Active) {
                    for (int i = 0, count = Options.Count; i < count; i++) {
                        if (Options[i] == itemOptions[0].Option) {
                            DisplayOption(i);
                            return;
                        }
                    }
                }
            }

            public void ClearItemOptions() => itemOptions.Clear();
        }

        private class ItemStorage<T> where T : EquippableItem {
            private readonly Dictionary<T, ItemUI> dict;
            public readonly ItemsMenu<T> Menu;

            private readonly Equipment<T> equipment;

            public ItemStorage(Character character, InventoryMenu inventoryMenu, string name, Equipment<T> equipment) {
                dict = new Dictionary<T, ItemUI>();
                Menu = new ItemsMenu<T>(character, inventoryMenu) {
                    Sprites = new Sprite[] {
                        new Sprite(name + Environment.NewLine)
                    }
                };
                this.equipment = equipment;
            }

            private class ItemUI {
                public readonly Sprite Sprite;
                public readonly Equipment<T>.ItemEquippedAction EquippedAction;
                public ItemUI(Sprite sprite, Equipment<T>.ItemEquippedAction equippedAction) {
                    Sprite = sprite;
                    EquippedAction = equippedAction;
                }
            }

            public void Clear() {
                dict.Clear();
            }

            public void AddOption(T equippableItem) {
                const string equipPrefix = "E";
                Sprite sprite = Menu.AddOption(equippableItem);
                void SetEquippedSprite() {
                    sprite.Display = GetPrefixedItemName(equippableItem, equipPrefix, Menu.Count(equippableItem));
                    Menu.ClearLength = sprite.Display.Length;
                }
                if (equippableItem.Name == equipment.Equipped.Name) {
                    // If item is already equipped, show it is so in the menu.
                    SetEquippedSprite();
                    Menu.ClearPosition = Menu.Options.Count /*- 1*/;
                }
                void ItemEquippedAction(Character source, T item, T oldItem) {
                    if (item != equippableItem) return;
                    ItemUI stored = dict[oldItem];
                    Menu.ClearSelectedItem();
                    stored.Sprite.Display = GetItemName(oldItem, Menu.Count(oldItem));
                    Renderer.Display(stored.Sprite, Menu.ClearItemPosition);

                    Menu.ClearPosition = Menu.SelectedOptionIndex + 1;
                    SetEquippedSprite();
                    Renderer.Display(sprite, Menu.ClearItemPosition);
                }
                equipment.ItemEquipped += ItemEquippedAction;
                dict.Add(equippableItem, new ItemUI(
                    sprite,
                    ItemEquippedAction
                ));
            }

            public void Remove(T key) {
                ItemUI item = dict[key];
                dict.Remove(key);
                Menu.RemoveOption(key);
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
        private void CloseMenuEvent(Menu menu, Option op) {
            if (previousMenu == menu)
                previousMenu = null;
            menu.Clear();
            GameManager.RemoveUI(menu);
            GameManager.PlayerInfo.Input.SelectedUIIndex = GameManager.GetSelectableUIIndex(this);
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
            itemsMenu = new ItemsMenu<Item>(Character, this) {
                Sprites = new Sprite[] {
                    new Sprite(itemsTitle + Environment.NewLine)
                }
            };
            potionsMenu = new ItemsMenu<Potion>(Character, this) {
                Sprites = new Sprite[] {
                    new Sprite(potionsTitle + Environment.NewLine)
                }
            };
            weaponStorage = new ItemStorage<Weapon>(Character, this, weaponsTitle, character.Weapon);
            bootStorage = new ItemStorage<Boot>(Character, this, bootsTitle, character.Boot);
            chestplateStorage = new ItemStorage<Chestplate>(Character, this, chestplatesTitle, character.Chestplate);
            sleeveStorage = new ItemStorage<Sleeve>(Character, this, sleevesTitle, character.Sleeve);
            pantsStorage = new ItemStorage<Pants>(Character, this, pantsTitle, character.Pants);

            Option closeOption = new Option() {
                Sprites = new Sprite[] {
                    new Sprite("Close", ConsoleColor.Red, ConsoleColor.Black)
                },
                Action = CloseMenuEvent
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
                itemsMenu.ClearItemOptions();
                MenuClear(potionsMenu);
                potionsMenu.ClearItemOptions();
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

                Character.Inventory.GoldChanged += Inventory_GoldChanged;
            };
            Deactivated += source => {
                Character.Inventory.AddedItem -= AddOption;
                Character.Inventory.RemovedItem -= Inventory_RemovedItem;

                Character.Inventory.GoldChanged -= Inventory_GoldChanged;
            };
        }

        #region Get Names
        private static string GetItemName(Item item, int count) {
            if (count > 1) {
                string countStr = count.ToString();
                return $"{GetContinuedString(item.Name, maxWidth - countStr.Length - 2)} x{countStr}";
            } else
                return GetItemName(item);
        }
        private static string GetItemName(Item item) {
            return GetContinuedString(item.Name, maxWidth);
        }
        private static string GetPrefixedItemName(Item item, string prefix, int count) {
            string str = $"[{prefix}]{item.Name}";
            if (count > 1) {
                string countStr = count.ToString();
                return $"{GetContinuedString(str, maxWidth - countStr.Length - 2)} x{countStr}";
            } else
                return GetContinuedString(str, maxWidth);
        }
        #endregion

        #region Events
        private void AddOption(Inventory inventory, Item item) {
            if (item is Potion potion) {
                potionsMenu.AddOption(potion);
            } else if (item is Weapon weapon) {
                weaponStorage.AddOption(weapon);
            } else if (item is Boot boot) {
                bootStorage.AddOption(boot);
            } else if (item is Chestplate chestplate) {
                chestplateStorage.AddOption(chestplate);
            } else if (item is Sleeve sleeve) {
                sleeveStorage.AddOption(sleeve);
            } else if (item is Pants pants) {
                pantsStorage.AddOption(pants);
            } else {
                itemsMenu.AddOption(item);
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

        private void Inventory_GoldChanged(Inventory source, int gold, int oldGold) {
            Vector2 pos = Position + new Vector2(6, 1);
            Renderer.Display(Sprite.CreateEmptyUI(oldGold.ToString().Length), pos);
            UpdateGoldSprite(gold);
            Renderer.Display(goldSprite, pos);
        }
        #endregion

        private void UpdateGoldSprite(int gold) {
            goldSprite.Display = GetContinuedString(gold.ToString(), maxWidth) + Environment.NewLine;
        }

        protected override void DisplayUI() {
            string name = GetContinuedString(Character.Name, maxWidth);
            StringBuilder stringBuilder = new StringBuilder(name, name.Length + 2);
            stringBuilder.Append('\'');
            if (!Character.Name.EndsWith('s')) {
                stringBuilder.Append('s');
            }
            nameSprite.Display = stringBuilder.ToString();
            UpdateGoldSprite(Character.Inventory.Gold);

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
