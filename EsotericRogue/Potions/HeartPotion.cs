namespace EsotericRogue.Potions {
    public class HeartPotion : Potion, Enchantment.IHeal {
        public override string Name => "Heart Potion";
        public int Heal { get; set; } = 1;

        protected override void UseAction(Character character) {
            character.IncreaseMaxHealth(Heal);
        }
    }
}
