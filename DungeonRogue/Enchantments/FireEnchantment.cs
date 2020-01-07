using System;
using System.Collections.Generic;
using EsotericRogue;

namespace DungeonRogue.Enchantments {
    public class FireEnchantment : Enchantment, Enchantment.IDamage {
        public override string Name => "Fire";
        private readonly int stepDuration;
        public override int StepsDuration => stepDuration;
        public int Damage { get; set; } = 1;
        public DamageType DamageType { get; set; } = DamageType.Magical;

        public FireEnchantment(Character character, int duration) : base(character) {
            stepDuration = duration;
        }

        public override IEnumerable<Sprite> GetDescription() {
            return new Sprite[] {
                Sprite.CreateUI("Deal "),
                new Sprite(Damage.ToString(), Weapon.GetDamageColor(DamageType), ConsoleColor.Black),
                Sprite.CreateUI(" damage each step.")
            };
        }

        protected override void Disenchant(Character character) {
            character.Stepped -= Character_Stepped;
        }

        protected override void Enchant(Character character) {
            character.Stepped += Character_Stepped;
        }

        private void Character_Stepped(Character character) {
            character.Damage(Damage, DamageType);
        }
    }
}
