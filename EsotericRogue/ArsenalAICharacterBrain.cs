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

        #region Static
        protected static void Controls(ArsenalAICharacterBrain characterBrain, Character enemyCharacter) {
            for (int i = 0, count = characterBrain.Arsenal.Count; i < count; i++) {
                Weapon weapon = characterBrain.Arsenal[i];
                List<int> indexes = new List<int>(weapon.Count);
                for (int k = 0, kCount = weapon.Count; k < kCount; k++) {
                    indexes.Add(k);
                }
                bool UseWeaponAction() {
                    int pickedIndex;
                    do {
                        if (indexes.Count == 0)
                            return false;
                        pickedIndex = rng.Next(indexes.Count);
                        indexes.RemoveAt(pickedIndex);
                    } while (!characterBrain.UseWeapon(i, pickedIndex, enemyCharacter));
                    return true;
                }
                if (UseWeaponAction())
                    return;
            }
            characterBrain.usedWeaponDescription = new Sprite[] { Sprite.CreateUI("Resting…") };
        }

        public static void ArsenalConstructor(ArsenalAICharacterBrain characterBrain) {
            characterBrain.Arsenal = new Weapon[] {
                characterBrain.Character.Weapon.BareItem
            };
        }
        #endregion
    }
}
