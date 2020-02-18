using System.Collections.Generic;
using EsotericRogue;

namespace DungeonRogue.Weapons.Dragon {
    public class DragonWeapon : Weapon<DragonWeapon> {
        public override string Name => "Bare";
        private readonly Weapon.Action[] actions = new Weapon.Action[] {
                new Bite(),
                new TailSwipe()
            };
        protected override IReadOnlyList<Weapon.Action> Actions => actions;

        private class Bite : Action, Enchantment.IDamage {
            public int Damage { get; set; }
            public DamageType DamageType { get; set; }

            public override IEnumerable<Sprite> GetDescription() {
                return GetDamageDescription("Bite", this);
            }

            protected override bool Execute(DragonWeapon source, Character targetCharacter, int distance) {
                return ExecuteDamageAction(this, targetCharacter);
            }
        }

        private class TailSwipe : Action, Enchantment.IDamage {
            public int Damage { get; set; }
            public DamageType DamageType { get; set; }

            public override IEnumerable<Sprite> GetDescription() {
                return GetDamageDescription("Tail swipe", this);
            }

            protected override bool Execute(DragonWeapon source, Character targetCharacter, int distance) {
                return ExecuteDamageAction(this, targetCharacter);
            }
        }
    }
}
