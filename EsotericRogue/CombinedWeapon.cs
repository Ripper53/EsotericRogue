using System.Collections.Generic;

namespace EsotericRogue {
    public class CombinedWeapon : Weapon<CombinedWeapon> {
        protected static readonly System.Random rng = new System.Random();
        private readonly Action[] actions;
        protected override IList<Weapon.Action> Actions => actions;

        #region Actions
        private class DamageAction : Action, IDamage {
            public readonly string Description;
            public int Damage { get; set; }

            public DamageAction(string description) {
                Description = description;
            }

            public override void Execute(CombinedWeapon source, Character targetCharacter) {
                throw new System.NotImplementedException();
            }

            public override IEnumerable<Sprite> GetDescription() {
                return GetDamageDescription(Description, Damage);
            }
        }
        private class HealAction : Action, IHeal {
            public readonly string Description;
            public int Heal { get; set; }

            public HealAction(string description) {
                Description = description;
            }

            public override void Execute(CombinedWeapon source, Character targetCharacter) {
                throw new System.NotImplementedException();
            }

            public override IEnumerable<Sprite> GetDescription() {
                throw new System.NotImplementedException();
            }
        }
        private class DamageHealAction : Action, IDamage, IHeal {
            public readonly string Description;
            public int Damage { get; set; }
            public int Heal { get; set; }

            public DamageHealAction(string description) {
                Description = description;
            }

            public override void Execute(CombinedWeapon source, Character targetCharacter) {
                throw new System.NotImplementedException();
            }

            public override IEnumerable<Sprite> GetDescription() {
                throw new System.NotImplementedException();
            }
        }
        #endregion

        public CombinedWeapon(Weapon weapon1, Weapon weapon2) {
            SetName("Combined " + weapon1.Name);

            #region Prep
            int weapon1Count = weapon1.Count, weapon2Count = weapon2.Count;
            List<Action> actions = new List<Action>(weapon1Count + weapon2Count);
            GetActions(weapon1, actions);
            GetActions(weapon2, actions);
            #endregion

            #region Combine
            List<Action> combinedActions = new List<Action>();
            while (actions.Count > 1) {
                // Combine random actions.
                int index = rng.Next(actions.Count);
                Action action1 = actions[index];
                actions.RemoveAt(index);
                index = rng.Next(actions.Count);
                Action action2 = actions[index];
                actions.RemoveAt(index);
                Combine(action1, action2);
            }
            // If there is one action left, meaning there is nothing left to combine it with.
            if (actions.Count == 1)
                combinedActions.Add(actions[0]);
            #endregion

            this.actions = actions.ToArray();
        }

        private static void GetActions(Weapon weapon, List<Action> actions) {
            foreach (Action action in weapon)
                actions.Add(action);
        }

        private static Action Combine(Action action1, Action action2) {
            bool
                bothDamage = AreBoth(action1, action2, out IDamage damageAction1, out IDamage damageAction2),
                bothHeal = AreBoth(action1, action2, out IHeal healAction1, out IHeal healAction2);

            if (bothDamage && bothHeal) {
                DamageHealAction action = new DamageHealAction("");
                Combine(action, damageAction1, damageAction2);
                Combine(action, healAction1, healAction2);
                return action;
            } else if (bothDamage) {
                DamageAction action = new DamageAction("");
                Combine(action, damageAction1, damageAction2);
                return action;
            } else if (bothHeal) {
                HealAction action = new HealAction("");
                Combine(action, healAction1, healAction2);
                return action;
            } else {
                return rng.Next(2) == 0 ? action1 : action2;
            }
        }
        private static bool AreBoth<T>(Action action1, Action action2, out T actionT1, out T actionT2) where T : class {
            if (action1 is T t1) {
                actionT1 = t1;
                if (action2 is T t2) {
                    actionT2 = t2;
                    return true;
                } else {
                    actionT2 = null;
                }
            } else {
                actionT1 = null;
                actionT2 = null;
            }
            return false;
        }

        private static void Combine(IDamage action, IDamage damageAction1, IDamage damageAction2) {
            action.Damage = damageAction1.Damage + damageAction2.Damage;
        }
        private static void Combine(IHeal action, IHeal healAction1, IHeal healAction2) {
            action.Heal = healAction1.Heal + healAction2.Heal;
        }
    }
}
