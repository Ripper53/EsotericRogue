namespace EsotericRogue {
    public class ManageInventoryMenu : InventoryMenu {
        public ManageInventoryMenu(GameManager gameManager, Character character) : base(gameManager, character) { }

        protected override Option.OptionAction GetOptionAction<T>(ItemsMenu<T> itemsMenu, T item) {
            return (menu, op) => itemsMenu.ItemOptions[item.Name][0].Item.Use(Character);
        }
    }
}
