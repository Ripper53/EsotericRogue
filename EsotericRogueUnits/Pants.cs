namespace EsotericRogue {
    public abstract class Pants : ProtectiveItem {

        protected override bool UseAction(Character character) {
            character.Pants.Equipped = this;
            return true;
        }
    }
}
