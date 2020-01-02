using EsotericRogue;

namespace DungeonRogue.Potions {
    public class HeartPotion : Potion, Enchantment.IHeal {
        public override string Name => "Heart Potion";
        public int Heal { get; set; } = 1;

        protected override bool UseAction(Character character) {
            character.IncreaseMaxHealth(Heal);
            character.Heal(Heal);
            return true;
        }
    }
}
