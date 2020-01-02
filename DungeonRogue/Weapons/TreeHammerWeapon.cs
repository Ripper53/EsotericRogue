using System.Collections.Generic;
using EsotericRogue;

namespace DungeonRogue.Weapons {
    public class TreeHammerWeapon : Weapon<TreeHammerWeapon> {
        public override string Name => "Tree Hammer";
        private readonly Weapon.Action[] actions = new Weapon.Action[] {
            new SwingAction(),
            new SmashAction()
        };
        protected override IList<Weapon.Action> Actions => actions;

        private class SwingAction : Action, Enchantment.IDamage, Enchantment.IStaminaCost {
            public int Damage { get; set; }
            public DamageType DamageType { get; set; }
            public int StaminaCost { get; set; }

            public SwingAction() {
                Damage = 5;
                DamageType = DamageType.Physical;
                StaminaCost = 3;
                Range = 4;
            }

            public override IEnumerable<Sprite> GetDescription() {
                return GetDamageStaminaDescription("Swing", this);
            }

            protected override bool Execute(TreeHammerWeapon source, Character targetCharacter, int distance) {
                return ExecuteDamageStaminaAction(source, this, targetCharacter);
            }
        }
        private class SmashAction : Action, Enchantment.IDamage, Enchantment.IStaminaCost {
            public int Damage { get; set; }
            public DamageType DamageType { get; set; }
            public int StaminaCost { get; set; }

            public SmashAction() {
                Damage = 8;
                DamageType = DamageType.Physical;
                StaminaCost = 5;
                Range = 4;
            }

            protected override bool Execute(TreeHammerWeapon source, Character targetCharacter, int distance) {
                return ExecuteDamageStaminaAction(source, this, targetCharacter);
            }

            public override IEnumerable<Sprite> GetDescription() {
                return GetDamageStaminaDescription("Smash", this);
            }
        }
    }
}
