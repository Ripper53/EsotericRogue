using System.Collections.Generic;
using EsotericRogue;

namespace DungeonRogue {
    public class BareWeapon : Weapon<BareWeapon> {
        public override string Name => "Bare";
        private readonly Action[] actions = new Action[] { new PunchAction() };
        protected override IList<Weapon.Action> Actions => actions;

        private class PunchAction : Action, Enchantment.IDamage {
            public int Damage { get; set; } = 1;

            public override IEnumerable<Sprite> GetDescription() {
                return GetDamageDescription("Punch", Damage);
            }

            public override void Execute(BareWeapon source, Character targetCharacter) {
                targetCharacter.Damage(Damage);
            }
        }
    }
}
