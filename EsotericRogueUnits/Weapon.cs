using System;
using System.Collections;
using System.Collections.Generic;

namespace EsotericRogue {
    public abstract class Weapon : EquippableItem, IReadOnlyList<Weapon.Action> {
        public override bool Consumable => false;

        public static ConsoleColor GetDamageColor(DamageType damageType) {
            switch (damageType) {
                case DamageType.Physical:
                    return Character.StaminaSprite.Foreground;
                case DamageType.Magical:
                    return Character.ManaSprite.Foreground;
                case DamageType.Electrical:
                    return Character.EnergySprite.Foreground;
                default:
                    return ConsoleColor.White;
            }
        }

        public abstract class Action {
            public int Range = 1;
            public abstract IEnumerable<Sprite> GetDescription();

            #region Static
            private static Sprite GetRangeSprite(int range) => Sprite.CreateUI($"[Range: {range}]");

            public static Sprite[] GetDamageDescription<T>(string attackDescription, T enchantment) where T : Action, Enchantment.IDamage {
                return new Sprite[] {
                    Sprite.CreateUI(attackDescription + " for "),
                    new Sprite(enchantment.Damage.ToString(), GetDamageColor(enchantment.DamageType), ConsoleColor.Black),
                    Sprite.CreateUI(" damage. "),
                    GetRangeSprite(enchantment.Range)
                };
            }

            private static Sprite[] GetDamageCostDescription<T>(string attackDescription, T enchantment, int cost, string costName) where T : Action, Enchantment.IDamage {
                return new Sprite[] {
                    Sprite.CreateUI(attackDescription + " for "),
                    new Sprite(enchantment.Damage.ToString(), GetDamageColor(enchantment.DamageType), ConsoleColor.Black),
                    Sprite.CreateUI(" damage, costs "),
                    new Sprite(cost.ToString(), GetDamageColor(enchantment.DamageType), ConsoleColor.Black),
                    Sprite.CreateUI($" {costName}. "),
                    GetRangeSprite(enchantment.Range)
                };
            }

            protected static Sprite[] GetDamageStaminaDescription<T>(string attackDescription, T enchantment) where T : Action, Enchantment.IDamage, Enchantment.IStaminaCost {
                return GetDamageCostDescription(attackDescription, enchantment, enchantment.StaminaCost, "stamina");
            }

            protected static Sprite[] GetDamageManaDescription<T>(string attackDescription, T enchantment) where T : Action, Enchantment.IDamage, Enchantment.IManaCost {
                return GetDamageCostDescription(attackDescription, enchantment, enchantment.ManaCost, "mana");
            }

            protected static Sprite[] GetDamageEnergyDescription<T>(string attackDescription, T enchantment) where T : Action, Enchantment.IDamage, Enchantment.IEnergyCost {
                return GetDamageCostDescription(attackDescription, enchantment, enchantment.EnergyCost, "energy");
            }
            #endregion
        }
        protected abstract IReadOnlyList<Action> Actions { get; }
        public int Count => Actions.Count;
        public Action this[int index] => Actions[index];

        protected override bool UseAction(Character character) {
            character.Weapon.Equipped = this;
            return true;
        }

        public abstract bool Use(int index, Character targetCharacter);

        public override IEnumerable<Sprite> GetDescription() {
            List<Sprite> sprites = new List<Sprite>();
            Sprite newLineSprite = new Sprite(Environment.NewLine);
            foreach (Action action in Actions) {
                sprites.AddRange(action.GetDescription());
                sprites.Add(newLineSprite);
            }
            return sprites;
        }

        public IEnumerator<Action> GetEnumerator() {
            return Actions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
    public abstract class Weapon<T> : Weapon where T : Weapon<T> {
        public abstract new class Action : Weapon.Action {

            public virtual bool Do(T source, Character targetCharacter, int distance) {
                if (ExecuteCheck(source, targetCharacter, distance))
                    return Execute(source, targetCharacter, distance);
                return false;
            }
            protected virtual bool ExecuteCheck(T source, Character targetCharacter, int distance) {
                return distance <= Range;
            }
            protected abstract bool Execute(T source, Character targetCharacter, int distance);

            #region Static
            protected static bool ExecuteDamageAction<ActionT>(ActionT source, Character targetCharacter) where ActionT : Action, Enchantment.IDamage {
                targetCharacter.Damage(source.Damage, source.DamageType);
                return true;
            }

            private static bool ExecuteDamageResourceAction<ActionT>(Resource resource, int cost, ActionT source, Character targetCharacter) where ActionT : Action, Enchantment.IDamage {
                if (resource.Use(cost)) {
                    targetCharacter.Damage(source.Damage, source.DamageType);
                    return true;
                }
                return false;
            }
            protected static bool ExecuteDamageStaminaAction<ActionT>(T weapon, ActionT source, Character targetCharacter) where ActionT : Action, Enchantment.IDamage, Enchantment.IStaminaCost {
                return ExecuteDamageResourceAction(weapon.Character.Stamina, source.StaminaCost, source, targetCharacter);
            }
            protected static bool ExecuteDamageManaAction<ActionT>(T weapon, ActionT source, Character targetCharacter) where ActionT : Action, Enchantment.IDamage, Enchantment.IManaCost {
                return ExecuteDamageResourceAction(weapon.Character.Mana, source.ManaCost, source, targetCharacter);
            }
            protected static bool ExecuteDamageEnergyAction<ActionT>(T weapon, ActionT source, Character targetCharacter) where ActionT : Action, Enchantment.IDamage, Enchantment.IEnergyCost {
                return ExecuteDamageResourceAction(weapon.Character.Energy, source.EnergyCost, source, targetCharacter);
            }
            #endregion
        }
        public abstract class ActionAmmo : Action {
            public int AmmoCost { get; set; } = 1;
        }
        public abstract class ActionAmmo<A> : ActionAmmo where A : Ammo {

            protected override bool ExecuteCheck(T source, Character targetCharacter, int distance) {
                return base.ExecuteCheck(source, targetCharacter, distance) && source.Inventory.Contains<A>(AmmoCost);
            }

            public override bool Do(T source, Character targetCharacter, int distance) {
                if (base.Do(source, targetCharacter, distance)) {
                    source.Inventory.RemoveItems<A>(AmmoCost);
                    return true;
                }
                return false;
            }
        }

        public override bool Use(int index, Character targetCharacter) {
            return ((Action)Actions[index]).Do((T)this, targetCharacter, Math.Abs(Character.Distance - targetCharacter.Distance));
        }
    }
}
