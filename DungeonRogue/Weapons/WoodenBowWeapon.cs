using System.Collections.Generic;
using EsotericRogue;
using DungeonRogue.Ammunition;

namespace DungeonRogue.Weapons {
    public class WoodenBowWeapon : Weapon<WoodenBowWeapon> {
        public override string Name => "Wooden Bow";
        private readonly Weapon.Action[] actions = new Weapon.Action[] {
            new ShootArrowAction(),
            new ShootTwoArrowsAction(),
            new SwingBowAction()
        };
        protected override IList<Weapon.Action> Actions => actions;

        private class ShootArrowAction : ActionAmmo<Arrow>, Enchantment.IDamage, Enchantment.IStaminaCost {
            public int Damage { get; set; }
            public DamageType DamageType { get; set; }
            public int StaminaCost { get; set; }

            public ShootArrowAction() {
                Damage = 4;
                DamageType = DamageType.Physical;
                StaminaCost = 2;
                Range = 10;
            }

            public override IEnumerable<Sprite> GetDescription() {
                return GetDamageStaminaDescription("Shoot arrow", this);
            }

            protected override bool Execute(WoodenBowWeapon source, Character targetCharacter, int distance) {
                return ExecuteDamageStaminaAction(source, this, targetCharacter);
            }
        }

        private class ShootTwoArrowsAction : ActionAmmo<Arrow>, Enchantment.IDamage, Enchantment.IStaminaCost {
            public int Damage { get; set; }
            public DamageType DamageType { get; set; }
            public int StaminaCost { get; set; }

            public ShootTwoArrowsAction() {
                Damage = 10;
                DamageType = DamageType.Physical;
                StaminaCost = 4;
                Range = 6;
                AmmoCost = 2;
            }

            public override IEnumerable<Sprite> GetDescription() {
                return GetDamageStaminaDescription("Shoot two arrows", this);
            }

            protected override bool Execute(WoodenBowWeapon source, Character targetCharacter, int distance) {
                return ExecuteDamageStaminaAction(source, this, targetCharacter);
            }
        }

        private class SwingBowAction : Action, Enchantment.IDamage {
            public int Damage { get; set; }
            public DamageType DamageType { get; set; }

            public SwingBowAction() {
                Damage = 2;
                DamageType = DamageType.Physical;
                Range = 2;
            }

            public override IEnumerable<Sprite> GetDescription() {
                return GetDamageDescription("Swing bow", this);
            }

            protected override bool Execute(WoodenBowWeapon source, Character targetCharacter, int distance) {
                return ExecuteDamageAction(this, targetCharacter);
            }
        }

    }
}
