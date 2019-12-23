namespace EsotericRogue {
    public abstract class Sleeve : ProtectiveItem {

        protected override bool UseAction(Character character) {
            character.Sleeve.Equipped = this;
            return true;
        }
    }
}
