using System.Collections.Generic;
using DungeonRogue.Ammunition;
using EsotericRogue;

namespace DungeonRogue.Weapons {
    public abstract class BowWeapon : Weapon<BowWeapon> {
        private readonly Action[] actions;
        protected override IReadOnlyList<Weapon.Action> Actions => actions;

        protected readonly ShootArrowAction shootArrowAction;
        protected readonly ShootTwoArrowsAction shootTwoArrowsAction;
        protected readonly SwingBowAction swingBowAction;

        public BowWeapon() {
            shootArrowAction = new ShootArrowAction();
            shootTwoArrowsAction = new ShootTwoArrowsAction();
            swingBowAction = new SwingBowAction();

            actions = new Action[] {
                shootArrowAction,
                shootTwoArrowsAction,
                swingBowAction
            };
        }

        protected class ShootArrowAction : ActionAmmo<Arrow>, Enchantment.IDamage, Enchantment.IStaminaCost {
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

            protected override bool Execute(BowWeapon source, Character targetCharacter, int distance) {
                return ExecuteDamageStaminaAction(source, this, targetCharacter);
            }
        }

        protected class ShootTwoArrowsAction : ActionAmmo<Arrow>, Enchantment.IDamage, Enchantment.IStaminaCost {
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

            protected override bool Execute(BowWeapon source, Character targetCharacter, int distance) {
                return ExecuteDamageStaminaAction(source, this, targetCharacter);
            }
        }

        protected class SwingBowAction : Action, Enchantment.IDamage {
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

            protected override bool Execute(BowWeapon source, Character targetCharacter, int distance) {
                return ExecuteDamageAction(this, targetCharacter);
            }
        }
    }
}
