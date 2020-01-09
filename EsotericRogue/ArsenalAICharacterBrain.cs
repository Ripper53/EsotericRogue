using System.Collections.Generic;

namespace EsotericRogue {
    public class ArsenalAICharacterBrain : AICharacterBrain {
        public IReadOnlyList<Weapon> Arsenal;

        protected IEnumerable<Sprite> usedWeaponDescription;
        public bool UseWeapon(int weaponIndex, Character enemyCharacter) {
            Weapon weapon = Character.Weapon.Equipped;
            if (weapon.Use(weaponIndex, enemyCharacter)) {
                usedWeaponDescription = weapon[weaponIndex].GetDescription();
                return true;
            }
            return false;
        }

        public override IEnumerable<Sprite> GetDescription() {
            return usedWeaponDescription;
        }

        private int index = 0;
        private void EquipNewWeapon() {
            index++;
            if (index == Arsenal.Count)
                index = 0;
            Character.Weapon.Equipped = Arsenal[index];
        }
        private bool UseWeapon(Character enemyCharacter) {
            Weapon weapon = Character.Weapon.Equipped;
            List<int> indexes = new List<int>(weapon.Count);
            for (int k = 0, kCount = weapon.Count; k < kCount; k++) {
                indexes.Add(k);
            }
            int pickedIndex;
            do {
                if (indexes.Count == 0)
                    return false;
                pickedIndex = rng.Next(indexes.Count);
                indexes.RemoveAt(pickedIndex);
            } while (!UseWeapon(pickedIndex, enemyCharacter));
            return true;
        }
        public override void Controls(Character enemyCharacter) {
            if (!UseWeapon(enemyCharacter))
                usedWeaponDescription = new Sprite[] { Sprite.CreateUI("Resting…") };

            EquipNewWeapon();
        }

    }
}
