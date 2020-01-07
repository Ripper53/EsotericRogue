using EsotericRogue;

namespace DungeonRogue.Potions {
    public class IntensityPotion : Potion, Enchantment.IEnergyAdd {
        public override string Name => "Intensity Potion";
        public int EnergyIncrease { get; set; } = 1;
        public int EnergyAdd { get; set; } = 1;

        protected override bool UseAction(Character character) {
            character.Energy.IncreaseMax(EnergyIncrease);
            character.Energy.Add(EnergyAdd);
            return true;
        }
    }
}
