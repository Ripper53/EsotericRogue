namespace EsotericRogue {
    public class RandomAICharacterBrain : ArsenalAICharacterBrain {

        public override void Controls(Character targetCharacter) {
            int
                index = rng.Next(Arsenal.Count),
                weaponIndex = rng.Next(Arsenal[index].Count);
            if (!UseWeapon(index, weaponIndex, targetCharacter)) {
                usedWeaponDescription = new Sprite[] {
                    Sprite.CreateUI("Failed action!")
                };
            }
        }
    }
}
