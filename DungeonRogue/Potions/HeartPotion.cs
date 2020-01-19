using System;
using EsotericRogue;
using System.Collections.Generic;

namespace DungeonRogue.Potions {
    public class HeartPotion : Potion, Enchantment.IHeal {
        public override string Name => "Heart Potion";
        public int HealthIncrease { get; set; } = 1;
        public int Heal { get; set; } = 1;

        public override IEnumerable<Sprite> GetDescription() {
            return new Sprite[] {
                SpriteUtility.Rarity(Rarity),
                Sprite.CreateUI("Gain "),
                new Sprite(HealthIncrease.ToString(), Character.HealthSprite.Foreground, ConsoleColor.Black),
                Sprite.CreateUI(" maximum health, then heal for "),
                new Sprite(Heal.ToString(), Character.HealthSprite.Foreground, ConsoleColor.Black),
                Sprite.CreateUI(".")
            };
        }

        protected override bool UseAction(Character character) {
            character.IncreaseMaxHealth(HealthIncrease);
            character.Heal(Heal);
            return true;
        }
    }
}
