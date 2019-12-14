namespace EsotericRogue {
    public class PlayerInfo {
        public readonly Unit Unit;
        public readonly PlayerUnitBrain UnitBrain;
        public readonly Character Character;
        public readonly PlayerCharacterBrain CharacterBrain;
        public readonly PlayerInput Input;
        public readonly InfoUI InfoUI;
        public readonly InventoryMenu InventoryMenu;

        public PlayerInfo(GameManager gameManager, Unit playerUnit) {
            Unit = playerUnit;
            UnitBrain = (PlayerUnitBrain)Unit.Brain;
            Character = Unit.Character;
            CharacterBrain = (PlayerCharacterBrain)Character.Brain;
            Input = new PlayerInput(gameManager, UnitBrain);
            InfoUI = new InfoUI(playerUnit.Character);
            InventoryMenu = new InventoryMenu(Character);
            CharacterBrain.PlayerInfo = this;
            gameManager.AddUI(InfoUI);
            gameManager.AddUI(InventoryMenu);
        }
    }
}
