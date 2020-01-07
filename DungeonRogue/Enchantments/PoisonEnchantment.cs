using EsotericRogue;
using System.Collections.Generic;

namespace DungeonRogue.Enchantments {
    public class PoisonEnchantment : Enchantment, Enchantment.IDamage {
        public override string Name => "Poison";
        private readonly int stepsDuration;
        public override int StepsDuration => stepsDuration;
        public int Damage { get; set; } = 1;
        public DamageType DamageType { get; set; } = DamageType.Physical;

        public PoisonEnchantment(Character targetCharacter, int duration) : base(targetCharacter) => stepsDuration = duration;

        protected override void Disenchant(Character character) {
            character.Stepped -= Character_Stepped;
        }

        protected override void Enchant(Character character) {
            character.Stepped += Character_Stepped;
        }

        private void Character_Stepped(Character character) {
            character.Damage(Damage, DamageType);
        }

        public override IEnumerable<Sprite> GetDescription() {
            return new Sprite[] {
                Sprite.CreateUI("Deal "),
                new Sprite(Damage.ToString(), Weapon.GetDamageColor(DamageType), System.ConsoleColor.Black),
                Sprite.CreateUI(" damage each step.")
            };
        }
    }
}
