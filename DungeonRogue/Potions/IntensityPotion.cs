using System;
using EsotericRogue;
using System.Collections.Generic;

namespace DungeonRogue.Potions {
    public class IntensityPotion : Potion, Enchantment.IEnergyAdd {
        public override string Name => "Intensity Potion";
        public int EnergyIncrease { get; set; } = 1;
        public int EnergyAdd { get; set; } = 1;

        public override IEnumerable<Sprite> GetDescription() {
            return new Sprite[] {
                SpriteUtility.Rarity(Rarity),
                Sprite.CreateUI("Gain "),
                new Sprite(EnergyIncrease.ToString(), Character.EnergySprite.Foreground, ConsoleColor.Black),
                Sprite.CreateUI(" maximum energy, then add for "),
                new Sprite(EnergyAdd.ToString(), Character.EnergySprite.Foreground, ConsoleColor.Black),
                Sprite.CreateUI(".")
            };
        }

        protected override bool UseAction(Character character) {
            character.Energy.IncreaseMax(EnergyIncrease);
            character.Energy.Add(EnergyAdd);
            return true;
        }
    }
}
