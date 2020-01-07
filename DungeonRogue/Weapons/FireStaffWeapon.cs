using System.Collections.Generic;
using DungeonRogue.Ammunition;
using EsotericRogue;
using DungeonRogue.Enchantments;

namespace DungeonRogue.Weapons {
    public class FireStaffWeapon : Weapon<FireStaffWeapon> {
        public override string Name => "Fire Staff";
        private readonly Weapon.Action[] actions = new Weapon.Action[] {
            new FireBallAction(),
            new FlameAction(),
            new FireArrowAction()
        };
        protected override IReadOnlyList<Weapon.Action> Actions => actions;

        private class FireBallAction : Action, Enchantment.IDamage, Enchantment.IManaCost {
            public int Damage { get; set; }
            public DamageType DamageType { get; set; }
            public int ManaCost { get; set; }

            public FireBallAction() {
                Damage = 10;
                DamageType = DamageType.Magical;
                ManaCost = 5;
                Range = 20;
            }

            public override IEnumerable<Sprite> GetDescription() {
                return GetDamageManaDescription("Throw fireball", this);
            }

            protected override bool Execute(FireStaffWeapon source, Character targetCharacter, int distance) {
                if (ExecuteDamageManaAction(source, this, targetCharacter)) {
                    new FireEnchantment(targetCharacter, 2);
                    return true;
                }
                return false;
            }
        }

        private class FlameAction : Action, Enchantment.IDamage, Enchantment.IManaCost {
            public int Damage { get; set; }
            public DamageType DamageType { get; set; }
            public int ManaCost { get; set; }

            public FlameAction() {
                Damage = 15;
                DamageType = DamageType.Magical;
                ManaCost = 5;
                Range = 20;
            }

            public override IEnumerable<Sprite> GetDescription() {
                return GetDamageManaDescription("Throw flame", this);
            }

            protected override bool Execute(FireStaffWeapon source, Character targetCharacter, int distance) {
                if (ExecuteDamageManaAction(source, this, targetCharacter)) {
                    new FireEnchantment(targetCharacter, 2) {
                        Damage = 4
                    };
                    return true;
                }
                return false;
            }
        }

        private class FireArrowAction : ActionAmmo<Arrow>, Enchantment.IDamage, Enchantment.IManaCost {
            public int Damage { get; set; }
            public DamageType DamageType { get; set; }
            public int ManaCost { get; set; }

            public FireArrowAction() {
                Damage = 20;
                DamageType = DamageType.Physical;
                ManaCost = 5;
                Range = 30;
            }

            public override IEnumerable<Sprite> GetDescription() {
                return GetDamageManaDescription("Fire arrow", this);
            }

            protected override bool Execute(FireStaffWeapon source, Character targetCharacter, int distance) {
                return ExecuteDamageManaAction(source, this, targetCharacter);
            }
        }
    }
}
