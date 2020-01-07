namespace EsotericRogue {
    public class LootInventoryMenu : InventoryMenu {
        private readonly Inventory toInventory;

        public LootInventoryMenu(GameManager gameManager, Character character, Inventory toInventory) : base(gameManager, character) {
            this.toInventory = toInventory;
        }

        protected override Option.OptionAction GetOptionAction<T>(ItemsMenu<T> itemsMenu, T item) {
            return (menu, op) => {
                item = itemsMenu.ItemOptions[item.Name][0].Item;
                Character.Inventory.RemoveItem(item);
                RemoveEquipped(Character, item);
                toInventory.AddItem(item);
            };
        }

        private static void RemoveEquipped(Character character, Item item) {
            if (item is Weapon weapon)
                RemoveEquipped(character.Weapon, weapon);
            else if (item is Boot boot)
                RemoveEquipped(character.Boot, boot);
            else if (item is Chestplate chestplate)
                RemoveEquipped(character.Chestplate, chestplate);
            else if (item is Sleeve sleeve)
                RemoveEquipped(character.Sleeve, sleeve);
            else if (item is Pants pants)
                RemoveEquipped(character.Pants, pants);
        }
        private static void RemoveEquipped<T>(Equipment<T> equipment, T item) where T : EquippableItem {
            if (equipment.Equipped == item)
                equipment.Equipped = null;
        }
    }
}
