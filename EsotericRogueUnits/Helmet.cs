namespace EsotericRogue {
    public abstract class Helmet : ProtectiveItem {

        protected override bool UseAction(Character character) {
            character.Helmet.Equipped = this;
            return true;
        }
    }
}
