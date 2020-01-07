using EsotericRogue;
using System.Collections.Generic;
using DungeonRogue.Ammunition;

namespace DungeonRogue.Weapons {
    public class LaserWeapon : Weapon<LaserWeapon> {
        public override string Name => "Laser";
        private readonly Action[] actions = new Action[] {
            new LaserCapacitorAction(),
            new LaserAction()
        };
        protected override IReadOnlyList<Weapon.Action> Actions => actions;

        private class LaserCapacitorAction : ActionAmmo<EnergyCapacitor>, Enchantment.IDamage {
            public int Damage { get; set; }
            public DamageType DamageType { get; set; }

            public LaserCapacitorAction() {
                Damage = 20;
                DamageType = DamageType.Electrical;
                Range = 100;
            }

            public override IEnumerable<Sprite> GetDescription() {
                return GetDamageDescription("Laser via energy capacitor", this);
            }

            protected override bool Execute(LaserWeapon source, Character targetCharacter, int distance) {
                return ExecuteDamageAction(this, targetCharacter);
            }
        }

        private class LaserAction : Action, Enchantment.IDamage, Enchantment.IEnergyCost {
            public int Damage { get; set; }
            public DamageType DamageType { get; set; }
            public int EnergyCost { get; set; }

            public override IEnumerable<Sprite> GetDescription() {
                return GetDamageEnergyDescription("Laser", this);
            }

            protected override bool Execute(LaserWeapon source, Character targetCharacter, int distance) {
                return ExecuteDamageEnergyAction(source, this, targetCharacter);
            }
        }
    }
}
