namespace EsotericRogue {
    public abstract class Chestplate : ProtectiveItem {

        protected override bool UseAction(Character character) {
            character.Chestplate.Equipped = this;
            return true;
        }
    }
}
