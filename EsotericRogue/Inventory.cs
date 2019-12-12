using System.Collections.Generic;

namespace EsotericRogue {
    public class Inventory {
        public int Gold;
        private readonly Dictionary<string, ItemCount> items;

        private class ItemCount {
            public readonly Item Item;
            public int Count;

            public ItemCount(Item item) {
                Item = item;
                Count = 1;
            }
        }

        public Inventory() {
            Gold = 0;
            items = new Dictionary<string, ItemCount>();
        }

        public void AddItem(Item item) {
            if (items.ContainsKey(item.Name)) {
                items[item.Name].Count++;
            } else {
                items.Add(item.Name, new ItemCount(item));
            }
        }

        public bool RemoveItem(Item item) {
            return RemoveItem(item.Name);
        }

        public bool RemoveItem(string itemName) {
            if (items.ContainsKey(itemName)) {
                items[itemName].Count--;
                if (items[itemName].Count == 0)
                    items.Remove(itemName);
                return true;
            }
            return false;
        }
    }
}
