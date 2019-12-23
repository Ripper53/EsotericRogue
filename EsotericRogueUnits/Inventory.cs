using System.Collections;
using System.Collections.Generic;

namespace EsotericRogue {
    public class Inventory : IEnumerable<Item> {
        public readonly Character Character;
        public int Gold;
        private readonly List<Item> items;

        public Inventory(Character character) {
            Character = character;
            Gold = 0;
            items = new List<Item>();
        }

        public Item this[int index] => items[index];

        public delegate void AddedItemAction(Inventory inventory, Item addedItem);
        public event AddedItemAction AddedItem;

        public bool AddItem(Item item) {
            if (items.Contains(item)) return false;
            item.Inventory = this;
            items.Add(item);
            AddedItem?.Invoke(this, item);
            return true;
        }

        public delegate void RemovedItemAction(Inventory inventory, Item removedItem);
        public event RemovedItemAction RemovedItem;

        public bool RemoveItem(Item item) {
            if (items.Remove(item)) {
                RemovedItem?.Invoke(this, item);
                return true;
            }
            return false;
        }

        public IEnumerator<Item> GetEnumerator() {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
