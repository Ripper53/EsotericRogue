using System;
using System.Collections.Generic;

namespace EsotericRogue {
    public enum DamageType {
        True,
        Physical,
        Magical,
        Electrical
    };
    public class Character {
        public static Sprite
            HealthSprite = new Sprite("Health", ConsoleColor.Red, ConsoleColor.Black),
            StaminaSprite = new Sprite("Stamina", ConsoleColor.Magenta, ConsoleColor.Black),
            ManaSprite = new Sprite("Mana", ConsoleColor.Blue, ConsoleColor.Black),
            EnergySprite = new Sprite("Energy", ConsoleColor.Yellow, ConsoleColor.Black);

        public string Name { get; set; } = string.Empty;
        public int Distance = 0, RemainingDistance = 0;

        public int Health { get; private set; }
        public int MaxHealth { get; private set; }
        public readonly Resource
            Stamina,
            Mana,
            Energy;
        public readonly Inventory Inventory;
        public readonly CharacterBrain Brain;
        private readonly Dictionary<string, Enchantment> enchantments;

        public readonly Equipment<Weapon> Weapon;
        public readonly Equipment<Boot> Boot;
        public readonly Equipment<Chestplate> Chestplate;
        public readonly Equipment<Sleeve> Sleeve;
        public readonly Equipment<Pants> Pants;

        public Character(int health) {
            Health = health;
            MaxHealth = health;
            Stamina = new Resource();
            Mana = new Resource();
            Energy = new Resource();
            Inventory = new Inventory(this);
        }

        /// <param name="bareWeapon">Cannot be null.</param>
        /// <param name="bareBoot">Cannot be null.</param>
        /// <param name="bareChestplate">Cannot be null.</param>
        public Character(int health, CharacterBrain characterBrain, Weapon bareWeapon, Boot bareBoot, Chestplate bareChestplate, Sleeve bareSleeve, Pants barePants) : this(health) {
            Brain = characterBrain;
            Brain.Character = this;
            enchantments = new Dictionary<string, Enchantment>();
            Weapon = new Equipment<Weapon>(this, bareWeapon);
            Boot = new Equipment<Boot>(this, bareBoot);
            Chestplate = new Equipment<Chestplate>(this, bareChestplate);
            Sleeve = new Equipment<Sleeve>(this, bareSleeve);
            Pants = new Equipment<Pants>(this, barePants);
        }

        internal void Enchant(Enchantment enchantment) {
            if (enchantments.ContainsKey(enchantment.Name)) {
                enchantments[enchantment.Name].StepCount = enchantment.StepsDuration;
            } else {
                enchantment.Enchant();
                enchantments.Add(enchantment.Name, enchantment);
            }
        }
        public void Step() {
            List<Enchantment> toRemove = new List<Enchantment>(enchantments.Count);
            foreach (Enchantment enchantment in enchantments.Values) {
                enchantment.StepCount--;
                if (enchantment.StepCount == 0) {
                    enchantment.Disenchant();
                    toRemove.Add(enchantment);
                }
            }
            foreach (Enchantment enchantment in toRemove)
                enchantments.Remove(enchantment.Name);

            ResourceStep();
        }
        public void ResourceStep() {
            Stamina.Step();
            Mana.Step();
            Energy.Step();
        }

        private void Check() {
            if (Health <= 0) {
                Die();
                Health = 0;
            } else if (Health > MaxHealth)
                Health = MaxHealth;
        }

        public int GetPhysicalDefense() {
            return
                Chestplate.Equipped.PhysicalDefense +
                Sleeve.Equipped.PhysicalDefense;
        }
        public int GetMagicalDefense() {
            return
                Chestplate.Equipped.MagicalDefense +
                Sleeve.Equipped.MagicalDefense;
        }
        public int GetElectricalDefense() {
            return
                Chestplate.Equipped.ElectricalDefense +
                Sleeve.Equipped.ElectricalDefense;
        }

        public delegate void HealthChangedAction(Character character, int oldHealth);
        public event HealthChangedAction HealthChanged;

        public delegate void DamagedAction(Character character, int originalDamage, DamageType damageType, int damageTaken);
        public event DamagedAction Damaged;
        public void Damage(int damage, DamageType damageType) {
            int oldHealth = Health;
            int damageTaken;
            static int getDamage(int damage, int defense) => (int)MathF.Ceiling(damage / ((defense / 100f) + 1f));
            switch (damageType) {
                case DamageType.Physical:
                    damageTaken = getDamage(damage, GetPhysicalDefense());
                    break;
                case DamageType.Magical:
                    damageTaken = getDamage(damage, GetMagicalDefense());
                    break;
                case DamageType.Electrical:
                    damageTaken = getDamage(damage, GetElectricalDefense());
                    break;
                default:
                    damageTaken = damage;
                    break;
            }
            Health -= damageTaken;
            Check();
            Damaged?.Invoke(this, damage, damageType, damageTaken);
            HealthChanged?.Invoke(this, oldHealth);
        }

        public delegate void HealedAction(Character character, int heal);
        public event HealedAction Healed;
        public void Heal(int heal) {
            int oldHealth = Health;
            Health += heal;
            Check();
            Healed?.Invoke(this, heal);
            HealthChanged?.Invoke(this, oldHealth);
        }

        public void IncreaseMaxHealth(int maxHealthIncrease) {
            MaxHealth += maxHealthIncrease;
            if (MaxHealth < 0)
                MaxHealth = 0;
            else if (MaxHealth > Resource.MaxResourceValue)
                MaxHealth = Resource.MaxResourceValue;
            Check();
        }

        public delegate void DiedAction(Character character);
        public event DiedAction Died;
        public void Die() {
            Died?.Invoke(this);
        }
    }
}
