using System;
using System.Collections.Generic;
using System.Text;

namespace EsotericRogue {
    public abstract class InventoryMenu : Menu {
        public readonly GameManager GameManager;
        public readonly Character Character;
        private readonly Sprite nameSprite, inventorySprite, goldSprite, capacitySprite;
        private readonly ItemsMenu<Item> itemsMenu;
        private readonly ItemsMenu<Potion> potionsMenu;
        private readonly ItemStorage<Weapon> weaponStorage;
        private readonly ItemStorage<Boot> bootStorage;
        private readonly ItemStorage<Helmet> helmetStorage;
        private readonly ItemStorage<Chestplate> chestplateStorage;
        private readonly ItemStorage<Sleeve> sleeveStorage;
        private readonly ItemStorage<Pants> pantsStorage;
        private readonly TextUI DescriptionTextUI;

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
            public abstract void ClearItemOptions();
        }
        protected class ItemsMenu<T> : ItemsMenu where T : Item {
            private readonly InventoryMenu inventoryMenu;
            public readonly Dictionary<string, List<ItemOption>> ItemOptions;
            public int Count(T item) => ItemOptions[item.Name].Count;

            public ItemsMenu(InventoryMenu inventoryMenu) {
                ItemOptions = new Dictionary<string, List<ItemOption>>();
                this.inventoryMenu = inventoryMenu;
                SelectedOptionIndexChanged += (source, index, oldIndex) => this.inventoryMenu.InventoryMenu_SelectedOptionIndexChanged(this, index);
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
                    // Get already created option.
                    Option option = itemOptions[0].Option;
                    sprite = itemOptions[0].Sprite;
                    itemOptions.Add(new ItemOption(item, option, sprite));
                    sprite.Display = GetItemName(item, itemOptions.Count);
                    if (Active)
                        DisplayOption(option.Index);
                } else {
                    // This is a new item in the inventory.
                    Option option = new InventoryMenu.Option() {
                        Item = item,
                        Action = inventoryMenu.GetOptionAction(this, item)
                    };
                    sprite = Sprite.CreateUI(GetItemName(item));
                    ItemOptions.Add(key, new List<ItemOption>(1) { new ItemOption(item, option, sprite) });
                    option.Sprites = new Sprite[] { sprite };
                    AddOption(option);
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

            public override void ClearItemOptions() => ItemOptions.Clear();
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
        private Vector2 MenuPosition => new Vector2(Position.x, Position.y + 11);
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
        private void CloseMenuEvent(Menu menu, Menu.Option option) {
            if (previousMenu == menu)
                previousMenu = null;
            menu.Clear();
            GameManager.RemoveUI(menu);
            GameManager.PlayerInfo.Input.SelectedUIIndex = GameManager.GetSelectableUIIndex(this);
        }

        private delegate void MenuClearAction();
        public InventoryMenu(GameManager gameManager, Character character) {
            GameManager = gameManager;
            Character = character;
            nameSprite = new Sprite(string.Empty);
            inventorySprite = new Sprite(string.Empty);
            goldSprite = Sprite.CreateUI(string.Empty);
            capacitySprite = Sprite.CreateUI(string.Empty);
            DescriptionTextUI = new TextUI();

            const string
                itemsTitle =       "Items       ",
                potionsTitle =     "Potions     ",
                weaponsTitle =     "Weapons     ",
                bootsTitle =       "Boots       ",
                helmetTitle =      "Helmets     ",
                chestplatesTitle = "Chestplates ",
                sleevesTitle =     "Sleeves     ",
                pantsTitle =       "Pants       ";
            Sprites = new Sprite[] {
                nameSprite,
                inventorySprite,
                Sprite.CreateUI("Gold: "),
                goldSprite,
                Sprite.CreateUI(Environment.NewLine + "Capacity: "),
                capacitySprite
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

            Menu.Option closeOption = new Menu.Option() {
                Sprites = new Sprite[] {
                    new Sprite("Close", ConsoleColor.Red, ConsoleColor.Black)
                },
                Action = CloseMenuEvent
            };
            MenuClearAction menuClear = null;
            void AddToMenuClear(ItemsMenu menu) {
                menuClear += () => {
                    const int index = 1;
                    if (menu.Options.Count > index)
                        menu.RemoveRangeOptions(index, menu.Options.Count - index);
                    menu.ClearItemOptions();
                };
            }
            void AddOptionItemsMenu<T>(ItemsMenu<T> itemsMenu, string title) where T : Item {
                itemsMenu.AddOption(closeOption);
                AddToMenuClear(itemsMenu);
                AddOption(new Menu.Option() {
                    Sprites = new Sprite[] {
                        Sprite.CreateUI(title)
                    },
                    Action = (menu, op) => DisplayMenuEvent(itemsMenu)
                });
            }
            AddOptionItemsMenu(itemsMenu, itemsTitle);
            AddOptionItemsMenu(potionsMenu, potionsTitle);

            ItemStorage<T> CreateItemStorage<T>(string title, Equipment<T> equipment) where T : EquippableItem {
                ItemStorage<T> itemStorage = new ItemStorage<T>(this, title, equipment);
                AddOptionItemsMenu(itemStorage.Menu, title);
                AddToMenuClear(itemStorage.Menu);
                return itemStorage;
            }

            weaponStorage = CreateItemStorage(weaponsTitle, Character.Weapon);
            bootStorage = CreateItemStorage(bootsTitle, Character.Boot);
            helmetStorage = CreateItemStorage(helmetTitle, Character.Helmet);
            chestplateStorage = CreateItemStorage(chestplatesTitle, Character.Chestplate);
            sleeveStorage = CreateItemStorage(sleevesTitle, Character.Sleeve);
            pantsStorage = CreateItemStorage(pantsTitle, Character.Pants);

            Activated += source => {
                Inventory inventory = Character.Inventory;
                // Add inventory to menu
                foreach (Item item in inventory)
                    AddOption(inventory, item);

                inventory.AddedItem += AddOption;
                inventory.RemovedItem += Inventory_RemovedItem;

                inventory.GoldChanged += Inventory_GoldChanged;
                inventory.TakenSpaceChanged += Inventory_TakenSpaceChanged;
                inventory.CapacityChanged += Inventory_TakenSpaceChanged;
            };
            Deactivated += source => {
                if (previousMenu != null)
                    GameManager.RemoveUI(previousMenu);

                // Clear inventory menu
                menuClear();

                Inventory inventory = Character.Inventory;
                inventory.AddedItem -= AddOption;
                inventory.RemovedItem -= Inventory_RemovedItem;

                inventory.GoldChanged -= Inventory_GoldChanged;
                inventory.TakenSpaceChanged -= Inventory_TakenSpaceChanged;
                inventory.CapacityChanged -= Inventory_TakenSpaceChanged;
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
        public new class Option : Menu.Option {
            public Item Item;

            public IEnumerable<Sprite> GetDescription() {
                return Item.GetDescription();
            }
        }

        private void InventoryMenu_SelectedOptionIndexChanged(ItemsMenu itemsMenu, int selectedOptionIndex) {
            DescriptionTextUI.Clear();
            IReadOnlyList<Menu.Option> options = itemsMenu.Options;
            if (selectedOptionIndex >= options.Count || selectedOptionIndex < 0) return;
            Menu.Option menuOption = options[selectedOptionIndex];
            if (!(menuOption is Option option)) return;
            DescriptionTextUI.Sprites = option.GetDescription();
            DescriptionTextUI.Display();
        }

        private void AddOption(Inventory inventory, Item item) {
            if (item is Potion potion) {
                potionsMenu.AddOption(potion);
            } else if (item is Weapon weapon) {
                weaponStorage.AddOption(weapon);
            } else if (item is Boot boot) {
                bootStorage.AddOption(boot);
            } else if (item is Helmet helmet) {
                helmetStorage.AddOption(helmet);
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
            } else if (item is Helmet helmet) {
                helmetStorage.Remove(helmet);
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
            Renderer.Display(Sprite.CreateEmptyUI(goldSprite.Display.Length), pos);
            UpdateGoldSprite(gold);
            Renderer.Display(goldSprite, pos);
        }
        private void Inventory_TakenSpaceChanged(Inventory source, int takenSpace, int oldTakenSpace) {
            Vector2 pos = Position + new Vector2(10, 2);
            Renderer.Display(Sprite.CreateEmptyUI(capacitySprite.Display.Length), pos);
            UpdateCapacitySprite(source.Capacity, takenSpace);
            Renderer.Display(capacitySprite, pos);
        }
        #endregion

        private void UpdateGoldSprite(int gold) {
            goldSprite.Display = GetContinuedString(gold.ToString(), maxWidth);
        }
        private void UpdateCapacitySprite(int capcity, int takenSpace) {
            capacitySprite.Display = GetContinuedString($"{takenSpace}/{capcity}", maxWidth) + Environment.NewLine;
        }

        protected override void DisplayUI() {
            string name = GetContinuedString(Character.Name, maxWidth);
            StringBuilder stringBuilder = new StringBuilder(name, name.Length + 2);
            stringBuilder.Append('\'');
            if (!Character.Name.EndsWith('s')) {
                stringBuilder.Append('s');
            }
            nameSprite.Display = stringBuilder.ToString();
            inventorySprite.Display = " Inventory".PadRight(maxWidth - 10 - nameSprite.Display.Length) + Environment.NewLine;
            UpdateGoldSprite(Character.Inventory.Gold);
            UpdateCapacitySprite(Character.Inventory.Capacity, Character.Inventory.TakenSpace);

            itemsMenu.Position = MenuPosition;
            weaponStorage.Menu.Position = MenuPosition;
            potionsMenu.Position = MenuPosition;

            DescriptionTextUI.Position = Position + new Vector2(maxWidth, 0);
            //DescriptionTextUI.Display();

            base.DisplayUI();
        }

        public override void Clear() {
            base.Clear();
            if (previousMenu != null)
                previousMenu.Clear();
        }

    }
}
