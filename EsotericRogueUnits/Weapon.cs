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

            private static Sprite GetRangeSprite(int range) => Sprite.CreateUI($"[Range: {range}]");

            public static Sprite[] GetDamageDescription<T>(string attackDescription, T enchantment) where T : Action, Enchantment.IDamage {
                return new Sprite[] {
                    Sprite.CreateUI(attackDescription + " for "),
                    new Sprite(enchantment.Damage.ToString(), GetDamageColor(enchantment.DamageType), ConsoleColor.Black),
                    Sprite.CreateUI(" damage. "),
                    GetRangeSprite(enchantment.Range)
                };
            }

            public static Sprite[] GetDamageStaminaDescription<T>(string attackDescription, T enchantment) where T : Action, Enchantment.IDamage, Enchantment.IStaminaCost {
                return new Sprite[] {
                    Sprite.CreateUI(attackDescription + " for "),
                    new Sprite(enchantment.Damage.ToString(), GetDamageColor(enchantment.DamageType), ConsoleColor.Black),
                    Sprite.CreateUI(" damage, costs "),
                    new Sprite(enchantment.StaminaCost.ToString(), GetDamageColor(DamageType.Physical), ConsoleColor.Black),
                    Sprite.CreateUI(" stamina. "),
                    GetRangeSprite(enchantment.Range)
                };
            }
        }
        protected abstract IList<Action> Actions { get; }
        public int Count => Actions.Count;
        public Action this[int index] => Actions[index];

        protected override bool UseAction(Character character) {
            character.Weapon.Equipped = this;
            return true;
        }

        public abstract bool Use(int index, Character targetCharacter);

        public IEnumerator<Action> GetEnumerator() {
            return (IEnumerator<Action>)Actions;
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
            protected static bool ExecuteDamageStaminaAction<ActionT>(T weapon, ActionT source, Character targetCharacter) where ActionT : Action, Enchantment.IDamage, Enchantment.IStaminaCost {
                if (weapon.Character.Stamina.Use(source.StaminaCost)) {
                    targetCharacter.Damage(source.Damage, source.DamageType);
                    return true;
                }
                return false;
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
