using System.Collections.Generic;

namespace EsotericRogue {
    public partial class CustomWeapon : Weapon<CustomWeapon> {
        protected static readonly System.Random rng = new System.Random();
        private string name;
        public override string Name => name;
        public void SetName(string name) => this.name = "[C] " + name;
        private readonly Action[] actions;
        protected override IList<Weapon.Action> Actions => actions;

        #region Actions
        private abstract new class Action : Weapon<CustomWeapon>.Action {
            public readonly string Description;

            public Action(string description) => Description = description;
        }

        private class DamageAction : Action, Enchantment.IDamage {
            public int Damage { get; set; }
            public DamageType DamageType { get; set; }

            public DamageAction(string description) : base(description) { }

            protected override bool Execute(CustomWeapon source, Character targetCharacter, int distance) {
                throw new System.NotImplementedException();
            }

            public override IEnumerable<Sprite> GetDescription() {
                return GetDamageDescription(Description, this, Range);
            }
        }
        private class HealAction : Action, Enchantment.IHeal {
            public int Heal { get; set; }

            public HealAction(string description) : base(description) { }

            protected override bool Execute(CustomWeapon source, Character targetCharacter, int distance) {
                throw new System.NotImplementedException();
            }

            public override IEnumerable<Sprite> GetDescription() {
                throw new System.NotImplementedException();
            }
        }
        private class DamageHealAction : Action, Enchantment.IDamage, Enchantment.IHeal {
            public int Damage { get; set; }
            public DamageType DamageType { get; set; }
            public int Heal { get; set; }

            public DamageHealAction(string description) : base(description) { }

            protected override bool Execute(CustomWeapon source, Character targetCharacter, int distance) {
                throw new System.NotImplementedException();
            }

            public override IEnumerable<Sprite> GetDescription() {
                throw new System.NotImplementedException();
            }
        }
        #endregion
    }
}
