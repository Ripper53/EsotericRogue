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

            public static Sprite[] GetDamageDescription(string attackDescription, Enchantment.IDamage enchantment, int range) {
                return new Sprite[] {
                    Sprite.CreateUI(attackDescription + " for "),
                    new Sprite(enchantment.Damage.ToString(), GetDamageColor(enchantment.DamageType), ConsoleColor.Black),
                    Sprite.CreateUI(" damage."),
                    Sprite.CreateUI($" [Range: {range}]")
                };
            }

            public static Sprite[] GetDamageDescription<T>(string attackDescription, T enchantment, int range) where T : Enchantment.IDamage, Enchantment.IStaminaCost {
                return new Sprite[] {
                    Sprite.CreateUI(attackDescription + " for "),
                    new Sprite(enchantment.Damage.ToString(), GetDamageColor(enchantment.DamageType), ConsoleColor.Black),
                    Sprite.CreateUI(" damage, "),
                    Sprite.CreateUI(" costs "),
                    new Sprite(enchantment.StaminaCost.ToString(), GetDamageColor(DamageType.Physical), ConsoleColor.Black),
                    Sprite.CreateUI(" stamina."),
                    Sprite.CreateUI($" [Range: {range}]")
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

            public bool Do(T source, Character targetCharacter, int distance) {
                if (distance > Range)
                    return false;
                return Execute(source, targetCharacter, distance);
            }
            protected abstract bool Execute(T source, Character targetCharacter, int distance);
        }

        public override bool Use(int index, Character targetCharacter) {
            return ((Action)Actions[index]).Do((T)this, targetCharacter, Math.Abs(Character.Distance - targetCharacter.Distance));
        }
    }
}
