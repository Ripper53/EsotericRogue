namespace EsotericRogue {
    public class PlayerInfo {
        public readonly Unit Unit;
        public readonly PlayerUnitBrain Brain;
        public readonly PlayerInput Input;
        public readonly InfoUI InfoUI;

        public PlayerInfo(GameManager gameManager, Unit playerUnit) {
            Unit = playerUnit;
            Brain = (PlayerUnitBrain)Unit.Brain;
            Input = new PlayerInput(gameManager, Brain);
            InfoUI = new InfoUI(playerUnit.Character);
            Input.PlayerUnitBrain.Scene = gameManager.Scene;
        }
    }
}
