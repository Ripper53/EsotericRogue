using EsotericRogue;

namespace DungeonRogue.Potions {
    public class HeartPotion : Potion, Enchantment.IHeal {
        public override string Name => "Heart Potion";
        public int HealthIncrease { get; set; } = 1;
        public int Heal { get; set; } = 1;

        protected override bool UseAction(Character character) {
            character.IncreaseMaxHealth(HealthIncrease);
            character.Heal(Heal);
            return true;
        }
    }
}
