using System;
using System.Collections.Generic;

namespace EsotericRogue {
    public abstract class ProtectiveItem : EquippableItem, Enchantment.IPhysicalDefense, Enchantment.IMagicalDefense, Enchantment.IElectricalDefense {
        public int PhysicalDefense { get; set; }
        public int MagicalDefense { get; set; }
        public int ElectricalDefense { get; set; }

        public override IEnumerable<Sprite> GetDescription() {
            return new Sprite[] {
                new Sprite("PD: " + PhysicalDefense + Environment.NewLine, Weapon.GetDamageColor(DamageType.Physical)),
                new Sprite("MD: " + MagicalDefense + Environment.NewLine, Weapon.GetDamageColor(DamageType.Magical)),
                new Sprite("ED: " + ElectricalDefense, Weapon.GetDamageColor(DamageType.Electrical))
            };
        }
    }
}
