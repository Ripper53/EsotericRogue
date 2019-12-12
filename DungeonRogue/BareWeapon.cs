using System.Collections.Generic;
using EsotericRogue;

namespace DungeonRogue {
    public class BareWeapon : Weapon<BareWeapon> {
        private readonly Action[] actions = new Action[] { new PunchAction() };
        protected override IList<Weapon.Action> Actions => actions;

        private class PunchAction : Action, IDamage {
            public int Damage { get; set; } = 1;

            public override IEnumerable<Sprite> GetDescription() {
                return GetDamageDescription("Punch", Damage);
            }

            public override void Execute(BareWeapon source, Character targetCharacter) {
                targetCharacter.Damage(Damage);
            }
        }

        public BareWeapon() {
            SetName("Bare");
        }
    }
}
