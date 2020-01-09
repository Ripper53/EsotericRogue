using System.Collections;
using System.Collections.Generic;

namespace EsotericRogue {
    public class Inventory : IReadOnlyList<Item> {
        public readonly Character Character;
        public delegate void GoldChangedAction(Inventory source, int gold, int oldGold);
        public event GoldChangedAction GoldChanged;
        private int gold;
        public int Gold {
            get => gold;
            set {
                int oldGold = gold;
                gold = value;
                GoldChanged?.Invoke(this, gold, oldGold);
            }
        }

        private readonly List<Item> items;
        public delegate void CapacityChangedAction(Inventory source, int capacity, int oldCapacity);
        public event CapacityChangedAction CapacityChanged;
        private int capacity;
        public int Capacity {
            get => capacity;
            set {
                int oldCapacity = capacity;
                capacity = value;
                CapacityChanged?.Invoke(this, capacity, oldCapacity);
            }
        }
        public delegate void TakenSpaceChangedAction(Inventory source, int takenSpace, int oldTakenSpace);
        public event TakenSpaceChangedAction TakenSpaceChanged;
        private int takenSpace;
        public int TakenSpace {
            get => takenSpace;
            set {
                int oldTakenSpace = takenSpace;
                takenSpace = value;
                TakenSpaceChanged?.Invoke(this, takenSpace, oldTakenSpace);
            }
        }
        public int Count => items.Count;
        //public void Clear() => items.Clear();

        public Inventory(Character character) {
            Character = character;
            gold = 0;
            items = new List<Item>();
            capacity = 30;
            takenSpace = 0;
        }

        public Item this[int index] => items[index];

        public bool Buy(Item item, int goldCost) {
            if (goldCost > Gold) return false;
            Gold -= goldCost;
            AddItem(item);
            return true;
        }

        public delegate void AddedItemAction(Inventory inventory, Item addedItem);
        public event AddedItemAction AddedItem;

        public bool AddItem(Item item) {
            int takenSpace = TakenSpace + item.Space;
            if (takenSpace > Capacity || items.Contains(item)) return false;
            TakenSpace = takenSpace;
            if (item.Inventory != null)
                item.Inventory.RemoveItem(item);
            item.Inventory = this;
            items.Add(item);
            AddedItem?.Invoke(this, item);
            return true;
        }

        public delegate void RemovedItemAction(Inventory inventory, Item removedItem);
        public event RemovedItemAction RemovedItem;
        private void RemoveItemEvent(Item item) {
            TakenSpace -= item.Space;
            item.Inventory = null;
            RemovedItem?.Invoke(this, item);
        }

        public bool RemoveItem(Item item) {
            if (items.Remove(item)) {
                RemoveItemEvent(item);
                return true;
            }
            return false;
        }

        public void RemoveItems<T>(int count) where T : Item {
            List<int> toRemoveIndexes = new List<int>(count);
            for (int i = 0, itemsCount = items.Count, c = 0; i < itemsCount; i++) {
                Item item = items[i];
                if (item is T) {
                    // Subtract c since the previous indexes are removed resulting in the indexes being shifted.
                    toRemoveIndexes.Add(i - c);
                    c++;
                    if (c == count)
                        break;
                }
            }
            foreach (int i in toRemoveIndexes) {
                Item item = items[i];
                items.RemoveAt(i);
                RemoveItemEvent(item);
            }
        }

        public bool Contains<T>(int count) where T : Item {
            int c = 0;
            foreach (Item i in items) {
                if (i is T) {
                    c++;
                    if (c == count)
                        return true;
                }
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
