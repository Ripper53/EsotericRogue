using System;
using System.Collections.Generic;
using System.Text;

namespace EsotericRogue {
    public abstract class InventoryMenu : Menu {
        public readonly GameManager GameManager;
        public readonly Character Character;
        private readonly Sprite nameSprite, goldSprite, spaceSprite;
        private readonly ItemsMenu<Item> itemsMenu;
        private readonly ItemsMenu<Potion> potionsMenu;
        private readonly ItemStorage<Weapon> weaponStorage;
        private readonly ItemStorage<Boot> bootStorage;
        private readonly ItemStorage<Chestplate> chestplateStorage;
        private readonly ItemStorage<Sleeve> sleeveStorage;
        private readonly ItemStorage<Pants> pantsStorage;

        private const int maxWidth = 20;

        protected abstract Option.OptionAction GetOptionAction<T>(ItemsMenu<T> itemsMenu, T item) where T : Item;

        #region Classes
        protected abstract class ItemsMenu : Menu {
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
        protected class ItemsMenu<T> : ItemsMenu where T : Item {
            private readonly InventoryMenu inventoryMenu;
            public readonly Dictionary<string, List<ItemOption>> ItemOptions;
            public int Count(T item) => ItemOptions[item.Name].Count;

            public ItemsMenu(InventoryMenu inventoryMenu) {
                ItemOptions = new Dictionary<string, List<ItemOption>>();
                this.inventoryMenu = inventoryMenu;
            }

            public class ItemOption {
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
                Vector2 pos = GetOptionPosition(optionIndex);
                clearOptions[optionIndex] = 0;
                foreach (Sprite sprite in Options[optionIndex].Sprites) {
                    Renderer.Display(sprite, pos);
                    clearOptions[optionIndex] += sprite.Display.Length;
                    pos = Renderer.CursorPosition;
                }
            }
            public Sprite AddOption(T item) {
                string key = item.Name;
                Sprite sprite;
                if (ItemOptions.ContainsKey(key)) {
                    // Item already exists in inventory.
                    List<ItemOption> itemOptions = ItemOptions[key];
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
                    //option.Sprites = itemOptions[0].Option.Sprites;
                    if (Active)
                        DisplayOption(optionIndex);
                } else {
                    // This is a new item in the inventory.
                    Option option = new Option() {
                        Action = inventoryMenu.GetOptionAction(this, item)
                    };
                    sprite = Sprite.CreateUI(GetItemName(item));
                    ItemOptions.Add(key, new List<ItemOption>(1) { new ItemOption(item, option, sprite) });
                    AddOption(option);
                    option.Sprites = new Sprite[] { sprite };
                    if (Active)
                        DisplayOption(Options.Count - 1);
                }
                return sprite;
            }
            public void RemoveOption(T item) {
                string key = item.Name;
                List<ItemOption> itemOptions = ItemOptions[key];
                if (itemOptions.Count > 1) {
                    itemOptions.RemoveAt(itemOptions.FindIndex(v => v.Item == item));
                    itemOptions[0].Sprite.Display = GetItemName(item, itemOptions.Count);
                } else {
                    ItemOptions.Remove(key);
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

            public void ClearItemOptions() => ItemOptions.Clear();
        }

        private class ItemStorage<T> where T : EquippableItem {
            private readonly Dictionary<T, ItemUI> dict;
            public readonly ItemsMenu<T> Menu;

            private readonly Equipment<T> equipment;

            public ItemStorage(InventoryMenu inventoryMenu, string name, Equipment<T> equipment) {
                dict = new Dictionary<T, ItemUI>();
                Menu = new ItemsMenu<T>(inventoryMenu) {
                    Sprites = new Sprite[] {
                        new Sprite(name + Environment.NewLine)
                    }
                };
                this.equipment = equipment;
                inventoryMenu.Activated += menu => this.equipment.ItemEquipped += ItemEquippedAction;
                inventoryMenu.Deactivated += menu => this.equipment.ItemEquipped -= ItemEquippedAction;
            }

            private class ItemUI {
                public readonly Sprite Sprite;
                public ItemUI(Sprite sprite) {
                    Sprite = sprite;
                }
            }

            private void SetEquippedSprite(Sprite sprite, T equippableItem) {
                sprite.Display = GetPrefixedItemName(equippableItem, "E", Menu.Count(equippableItem));
                Menu.ClearLength = sprite.Display.Length;
            }
            private void ItemEquippedAction(Equipment<T> source, T item, T oldItem) {
                if (dict.ContainsKey(oldItem)) {
                    ItemUI storedOldItem = dict[oldItem];
                    Menu.ClearSelectedItem();
                    storedOldItem.Sprite.Display = GetItemName(oldItem, Menu.Count(oldItem));
                    Renderer.Display(storedOldItem.Sprite, Menu.ClearItemPosition);
                }
                if (dict.ContainsKey(item)) {
                    ItemUI storedItem = dict[item];
                    Menu.ClearPosition = Menu.SelectedOptionIndex + 1;
                    SetEquippedSprite(storedItem.Sprite, item);
                    Renderer.Display(storedItem.Sprite, Menu.ClearItemPosition);
                }
            }

            public void Clear() {
                dict.Clear();
                Menu.ClearItemOptions();
            }

            public void AddOption(T equippableItem) {
                Sprite sprite = Menu.AddOption(equippableItem);
                if (equippableItem.Name == equipment.Equipped.Name) {
                    // If item is already equipped, show it is so in the menu.
                    SetEquippedSprite(sprite, equippableItem);
                    Menu.ClearPosition = Menu.Options.Count /*- 1*/;
                }
                dict.Add(equippableItem, new ItemUI(
                    sprite
                ));
            }

            public void Remove(T key) {
                dict.Remove(key);
                Menu.RemoveOption(key);
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
            spaceSprite = Sprite.CreateUI(string.Empty);

            Deactivated += menu => {
                if (previousMenu != null)
                    GameManager.RemoveUI(previousMenu);
            };

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
                goldSprite,
                Sprite.CreateUI(Environment.NewLine + "Space: "),
                spaceSprite
            };
            itemsMenu = new ItemsMenu<Item>(this) {
                Sprites = new Sprite[] {
                    new Sprite(itemsTitle + Environment.NewLine)
                }
            };
            potionsMenu = new ItemsMenu<Potion>(this) {
                Sprites = new Sprite[] {
                    new Sprite(potionsTitle + Environment.NewLine)
                }
            };
            weaponStorage = new ItemStorage<Weapon>(this, weaponsTitle, character.Weapon);
            bootStorage = new ItemStorage<Boot>(this, bootsTitle, character.Boot);
            chestplateStorage = new ItemStorage<Chestplate>(this, chestplatesTitle, character.Chestplate);
            sleeveStorage = new ItemStorage<Sleeve>(this, sleevesTitle, character.Sleeve);
            pantsStorage = new ItemStorage<Pants>(this, pantsTitle, character.Pants);

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
                Inventory inventory = Character.Inventory;
                // Add inventory to menu
                foreach (Item item in inventory)
                    AddOption(inventory, item);

                inventory.AddedItem += AddOption;
                inventory.RemovedItem += Inventory_RemovedItem;

                inventory.GoldChanged += Inventory_GoldChanged;
            };
            Deactivated += source => {
                static void MenuClear(Menu menu) {
                    const int index = 1;
                    if (menu.Options.Count > index)
                        menu.RemoveRangeOptions(index, menu.Options.Count - index);
                }
                // Clear inventory menu
                MenuClear(itemsMenu);
                MenuClear(potionsMenu);
                MenuClear(weaponStorage.Menu);
                MenuClear(bootStorage.Menu);
                MenuClear(chestplateStorage.Menu);
                MenuClear(sleeveStorage.Menu);
                MenuClear(pantsStorage.Menu);
                itemsMenu.ClearItemOptions();
                potionsMenu.ClearItemOptions();
                weaponStorage.Clear();
                bootStorage.Clear();
                chestplateStorage.Clear();
                sleeveStorage.Clear();
                pantsStorage.Clear();

                Inventory inventory = Character.Inventory;
                inventory.AddedItem -= AddOption;
                inventory.RemovedItem -= Inventory_RemovedItem;

                inventory.GoldChanged -= Inventory_GoldChanged;
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
        private void Inventory_RemovedItem(Inventory inventory, Item removedItem) => RemoveOption(removedItem);

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
