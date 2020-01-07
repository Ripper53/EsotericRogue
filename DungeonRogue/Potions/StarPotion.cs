using EsotericRogue;

namespace DungeonRogue.Potions {
    public class StarPotion : Potion, Enchantment.IManaAdd {
        public override string Name => "Star Potion";
        public int ManaIncrease { get; set; } = 1;
        public int ManaAdd { get; set; } = 1;

        protected override bool UseAction(Character character) {
            character.Mana.IncreaseMax(ManaIncrease);
            character.Mana.Add(ManaAdd);
            return true;
        }
    }
}
