using EsotericRogue;
using System.Collections.Generic;

namespace DungeonRogue {
    public static class CommonWeaponActions<T> where T : Weapon<T> {

        public class PunchAction : Weapon<T>.Action, Enchantment.IDamage, Enchantment.IStaminaCost {
            public int Damage { get; set; }
            public DamageType DamageType { get; set; }
            public int StaminaCost { get; set; }

            public PunchAction() {
                Damage = 4;
                DamageType = DamageType.Physical;
                StaminaCost = 2;
                Range = 2;
            }

            public override IEnumerable<Sprite> GetDescription() {
                return GetDamageStaminaDescription("Punch", this);
            }

            protected override bool Execute(T source, Character targetCharacter, int distance) {
                return ExecuteDamageStaminaAction(source, this, targetCharacter);
            }
        }

        public class SmashAction : Weapon<T>.Action, Enchantment.IDamage, Enchantment.IStaminaCost {
            public int Damage { get; set; }
            public DamageType DamageType { get; set; }
            public int StaminaCost { get; set; }

            public SmashAction() {
                Damage = 6;
                DamageType = DamageType.Physical;
                StaminaCost = 3;
                Range = 2;
            }

            public override IEnumerable<Sprite> GetDescription() {
                return GetDamageStaminaDescription("Smash", this);
            }

            protected override bool Execute(T source, Character targetCharacter, int distance) {
                return ExecuteDamageStaminaAction(source, this, targetCharacter);
            }
        }

        public class StompAction : Weapon<T>.Action, Enchantment.IDamage, Enchantment.IStaminaCost {
            public int Damage { get; set; }
            public DamageType DamageType { get; set; }
            public int StaminaCost { get; set; }

            public StompAction() {
                Damage = 10;
                DamageType = DamageType.Physical;
                StaminaCost = 5;
                Range = 2;
            }

            protected override bool Execute(T source, Character targetCharacter, int distance) {
                return ExecuteDamageStaminaAction(source, this, targetCharacter);
            }

            public override IEnumerable<Sprite> GetDescription() {
                return GetDamageStaminaDescription("Stomp", this);
            }
        }

    }
}
