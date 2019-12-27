using System.Collections.Generic;
using EsotericRogue;

namespace DungeonRogue.Weapons {
    public class BareWeapon : Weapon<BareWeapon> {
        public override string Name => "Bare";
        private readonly Action[] actions = new Action[] {
            new PunchAction()
        };
        protected override IList<Weapon.Action> Actions => actions;

        private class PunchAction : Action, Enchantment.IDamage {
            public int Damage { get; set; }
            public DamageType DamageType { get; set; }

            public PunchAction() {
                Damage = 1;
                DamageType = DamageType.Physical;
                Range = 2;
            }

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
