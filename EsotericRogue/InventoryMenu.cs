using System;
using System.Collections.Generic;
using System.Text;

namespace EsotericRogue {
    public class InventoryMenu : Menu {
        public readonly Character Character;
        public readonly Sprite NameSprite;
        public readonly Sprite GoldSprite;
        private readonly Dictionary<Item, Option> ops;

        public InventoryMenu(Character character) {
            Character = character;
            NameSprite = new Sprite(character.Name);
            GoldSprite = CreateSprite(character.Inventory.Gold.ToString());

            Sprites = new Sprite[] {
                NameSprite,
                new Sprite(" Inventory" + Environment.NewLine),
                CreateSprite("Gold: "),
                GoldSprite
            };
            ops = new Dictionary<Item, Option>();
            Options = new List<Option>();

            Activated += source => {
                Character.Inventory.AddedItem += Inventory_AddedItem;
                Character.Inventory.RemovedItem += Inventory_RemovedItem;
            };
            Deactivated += source => {
                Character.Inventory.AddedItem -= Inventory_AddedItem;
                Character.Inventory.RemovedItem -= Inventory_RemovedItem;
            };
        }

        #region Events
        private void AddOption(Inventory inventory, Item item) {
            Option op = new Option() {
                Sprites = new Sprite[] {
                    CreateSprite(item.Name)
                },
                Action = (menu, op) => item.Use(inventory.Character)
            };
            ops.Add(item, op);
        }
        private void RemoveOption(Item item) {
            Options.Remove(ops[item]);
            ops.Remove(item);
        }

        private void Inventory_AddedItem(Inventory inventory, Item addedItem) {
            AddOption(inventory, addedItem);
        }
        private void Inventory_RemovedItem(Inventory inventory, Item removedItem) {
            RemoveOption(removedItem);
        }
        #endregion

        protected override void DisplayUI() {
            string name = GetStringMax(Character.Name, 12);
            StringBuilder stringBuilder = new StringBuilder(name, name.Length + 2);
            if (Character.Name.EndsWith('s')) {
                stringBuilder.Append('\'');
            } else {
                stringBuilder.Append("'s");
            }
            NameSprite.Display = stringBuilder.ToString();
            stringBuilder.Clear();
            stringBuilder.Append(Character.Inventory.Gold);
            stringBuilder.Append(Environment.NewLine);
            GoldSprite.Display = stringBuilder.ToString();
            base.DisplayUI();
        }

    }
}
