using System;
using System.Collections;
using System.Collections.Generic;

namespace EsotericRogue {
    public abstract class Weapon : Item, IEnumerable<Weapon.Action> {
        private string name;
        public override string Name => name;
        public void SetName(string name) => this.name = name;

        public interface IDamage {
            int Damage { get; set; }
        }
        public interface IHeal {
            int Heal { get; set; }
        }

        public abstract class Action {
            public abstract IEnumerable<Sprite> GetDescription();

            public static Sprite[] GetDamageDescription(string attackDescription, int damage) {
                return new Sprite[] {
                    UI.CreateSprite(attackDescription + " for "),
                    new Sprite(damage.ToString(), ConsoleColor.Red, ConsoleColor.Black),
                    new Sprite(" damage.")
                };
            }
        }
        protected abstract IList<Action> Actions { get; }
        public int Count => Actions.Count;
        public Character Character { get; internal set; }
        public Action this[int index] => Actions[index];

        public override void Use(Character character) {
            character.EquippedWeapon = this;
        }

        public abstract void Use(int index, Character targetCharacter);

        public IEnumerator<Action> GetEnumerator() {
            return (IEnumerator<Action>)Actions;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
    public abstract class Weapon<T> : Weapon where T : Weapon<T> {
        public abstract new class Action : Weapon.Action {

            public abstract void Execute(T source, Character targetCharacter);
        }

        public override void Use(int index, Character targetCharacter) {
            ((Action)Actions[index]).Execute((T)this, targetCharacter);
        }
    }
}
