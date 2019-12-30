using System.Collections.Generic;

namespace EsotericRogue {
    public abstract class ArsenalAICharacterBrain : AICharacterBrain {
        public IReadOnlyList<Weapon> Arsenal;

        protected IEnumerable<Sprite> usedWeaponDescription;
        public bool UseWeapon(int index, int weaponIndex, Character enemyCharacter) {
            Weapon weapon = Arsenal[index];
            Character.Weapon.Equipped = weapon;
            if (weapon.Use(weaponIndex, enemyCharacter)) {
                usedWeaponDescription = weapon[weaponIndex].GetDescription();
                return true;
            } else
                return false;
        }

        public override IEnumerable<Sprite> GetDescription() {
            return usedWeaponDescription;
        }
    }
}
