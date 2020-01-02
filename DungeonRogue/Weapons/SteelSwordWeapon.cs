using System.Collections.Generic;
using EsotericRogue;

namespace DungeonRogue.Weapons {
    public class SteelSwordWeapon : Weapon<SteelSwordWeapon> {
        public override string Name => "Steel Sword";
        private readonly Weapon.Action[] actions = new Weapon.Action[] {
            new SwingAction()
        };
        protected override IList<Weapon.Action> Actions => actions;

        private class SwingAction : Action, Enchantment.IDamage, Enchantment.IStaminaCost {
            public int Damage { get; set; }
            public DamageType DamageType { get; set; }
            public int StaminaCost { get; set; }

            public SwingAction() {
                Damage = 3;
                DamageType = DamageType.Physical;
                StaminaCost = 1;
                Range = 3;
            }

            public override IEnumerable<Sprite> GetDescription() {
                return GetDamageStaminaDescription("Swing", this);
            }

            protected override bool Execute(SteelSwordWeapon source, Character targetCharacter, int distance) {
                return ExecuteDamageStaminaAction(source, this, targetCharacter);
            }
        }
    }
}
