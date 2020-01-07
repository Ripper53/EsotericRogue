namespace DungeonRogue.Weapons {
    public class WoodenBowWeapon : BowWeapon {
        public override string Name => "Wooden Bow";

        public WoodenBowWeapon() {
            shootArrowAction.Damage = 4;
            shootArrowAction.Range = 10;

            shootTwoArrowsAction.Damage = 10;
            shootTwoArrowsAction.Range = 4;

            swingBowAction.Damage = 2;
        }

    }
}
