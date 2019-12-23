using System.Collections.Generic;
using EsotericRogue;

namespace DungeonRogue.Weapons {
    public class BareWeapon : Weapon<BareWeapon> {
        public override string Name => "Bare";
        private readonly Action[] actions = new Action[] { new PunchAction() };
        protected override IList<Weapon.Action> Actions => actions;

        private class PunchAction : Action, Enchantment.IDamage {
            public int Damage { get; set; } = 1;
            public DamageType DamageType { get; set; } = DamageType.Physical;

            public override IEnumerable<Sprite> GetDescription() {
                return GetDamageDescription("Punch", this, Range);
            }

            protected override bool Execute(BareWeapon source, Character targetCharacter, int distance) {
                targetCharacter.Damage(Damage, DamageType.Physical);
                return true;
            }
        }
    }
}
