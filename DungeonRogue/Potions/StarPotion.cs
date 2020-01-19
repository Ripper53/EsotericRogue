using System;
using EsotericRogue;
using System.Collections.Generic;

namespace DungeonRogue.Potions {
    public class StarPotion : Potion, Enchantment.IManaAdd {
        public override string Name => "Star Potion";
        public int ManaIncrease { get; set; } = 1;
        public int ManaAdd { get; set; } = 1;

        public override IEnumerable<Sprite> GetDescription() {
            return new Sprite[] {
                SpriteUtility.Rarity(Rarity),
                Sprite.CreateUI("Gain "),
                new Sprite(ManaIncrease.ToString(), Character.ManaSprite.Foreground, ConsoleColor.Black),
                Sprite.CreateUI(" maximum mana, then add for "),
                new Sprite(ManaAdd.ToString(), Character.ManaSprite.Foreground, ConsoleColor.Black),
                Sprite.CreateUI(".")
            };
        }

        protected override bool UseAction(Character character) {
            character.Mana.IncreaseMax(ManaIncrease);
            character.Mana.Add(ManaAdd);
            return true;
        }
    }
}
