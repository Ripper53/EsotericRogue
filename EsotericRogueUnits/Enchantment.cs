using System;
using System.Collections.Generic;
using System.Text;

namespace EsotericRogue {
    /// <summary>
    /// Enchantments cannot stack, but they can refresh their duration.
    /// </summary>
    public abstract class Enchantment {
        public abstract string Name { get; }
        public abstract int StepsDuration { get; }
        public int StepCount;
        private readonly Character character;

        public Enchantment(Character targetCharacter) {
            StepCount = StepsDuration;
            character = targetCharacter;
            character.Enchant(this);
        }

        public void Enchant() => Enchant(character);
        protected abstract void Enchant(Character character);
        public void Disenchant() => Disenchant(character);
        protected abstract void Disenchant(Character character);
        public abstract IEnumerable<Sprite> GetDescription();

        public interface IDamage {
            int Damage { get; set; }
            DamageType DamageType { get; set; }
        }
        public interface IHeal {
            int Heal { get; set; }
        }

        public interface IStaminaCost {
            int StaminaCost { get; set; }
        }
        public interface IManaCost {
            int ManaCost { get; set; }
        }
        public interface IEnergyCost {
            int EnergyCost { get; set; }
        }

        public interface IStaminaAdd {
            int StaminaAdd { get; set; }
        }
        public interface IManaAdd {
            int ManaAdd { get; set; }
        }
        public interface IEnergyAdd {
            int EnergyAdd { get; set; }
        }

        public interface IPhysicalDefense {
            int PhysicalDefense { get; set; }
        }
        public interface IMagicalDefense {
            int MagicalDefense { get; set; }
        }
        public interface IElectricalDefense {
            int ElectricalDefense { get; set; }
        }

    }
}
