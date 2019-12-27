using System;
using System.Collections.Generic;

namespace EsotericRogue {
    public abstract class AICharacterBrain : CharacterBrain {
        protected static readonly Random rng = new Random();

        public IList<Weapon> Weapons;

        private IEnumerable<Sprite> usedWeaponDescription;
        public void UseWeapon(int index, int weaponIndex, Character enemyCharacter) {
            Weapon weapon = Weapons[index];
            Character.Weapon.Equipped = weapon;
            if (weapon.Use(weaponIndex, enemyCharacter))
                usedWeaponDescription = weapon[weaponIndex].GetDescription();
            else
                usedWeaponDescription = new Sprite[] {
                   Sprite.CreateUI("Failed action!")
                };
        }

        public override IEnumerable<Sprite> GetDescription() {
            return usedWeaponDescription;
        }
    }
}
