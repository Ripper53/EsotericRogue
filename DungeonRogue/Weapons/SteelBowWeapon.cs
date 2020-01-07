namespace DungeonRogue.Weapons {
    public class SteelBowWeapon : BowWeapon {
        public override string Name => "Steel Bow";

        public SteelBowWeapon() {
            shootArrowAction.Damage = 20;
            shootArrowAction.Range = 20;

            shootTwoArrowsAction.Damage = 20;
            shootTwoArrowsAction.Range = 10;

            swingBowAction.Damage = 4;
        }


    }
}
