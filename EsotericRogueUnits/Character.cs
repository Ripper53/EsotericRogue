using System;
using System.Collections.Generic;

namespace EsotericRogue {
    public class Character {
        public static Sprite
            HealthSprite = new Sprite("Health", ConsoleColor.Red, ConsoleColor.Black),
            StaminaSprite = new Sprite("Stamina", ConsoleColor.Magenta, ConsoleColor.Black),
            ManaSprite = new Sprite("Mana", ConsoleColor.Blue, ConsoleColor.Black),
            EnergySprite = new Sprite("Energy", ConsoleColor.Yellow, ConsoleColor.Black);

        public string Name { get; set; } = "";

        public int Health { get; private set; }
        public int MaxHealth { get; private set; }
        public readonly Resource
            Stamina,
            Mana,
            Energy;
        public readonly Inventory Inventory;
        public readonly CharacterBrain Brain;
        private readonly Dictionary<string, Enchantment> enchantments;

        private readonly Weapon bareWeapon;
        public delegate void WeaponEquippedAction(Character character, Weapon weapon, Weapon oldWeapon);
        public event WeaponEquippedAction WeaponEquipped;
        private Weapon equippedWeapon;
        public Weapon EquippedWeapon {
            get => equippedWeapon;
            set {
                Weapon oldWeapon = equippedWeapon;
                oldWeapon.Character = null;
                if (value == null)
                    value = bareWeapon;
                equippedWeapon = value;
                equippedWeapon.Character = this;
                WeaponEquipped?.Invoke(this, equippedWeapon, oldWeapon);
            }
        }

        public Character(int health, CharacterBrain characterBrain, Weapon bareWeapon) {
            Health = health;
            MaxHealth = health;
            Stamina = new Resource();
            Mana = new Resource();
            Energy = new Resource();
            Inventory = new Inventory(this);
            Brain = characterBrain;
            Brain.Character = this;
            enchantments = new Dictionary<string, Enchantment>();
            this.bareWeapon = bareWeapon;
            equippedWeapon = bareWeapon;
            Inventory.AddItem(bareWeapon);
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
        }

        private void Check() {
            if (Health <= 0) {
                Die();
                Health = 0;
            } else if (Health > MaxHealth)
                Health = MaxHealth;
        }

        public delegate void HealthChangedAction(Character character, int oldHealth);
        public event HealthChangedAction HealthChanged;

        public delegate void DamagedAction(Character character, int damage);
        public event DamagedAction Damaged;
        public void Damage(int damage) {
            int oldHealth = Health;
            Health -= damage;
            Check();
            Damaged?.Invoke(this, damage);
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
