namespace EsotericRogue {
    public class RandomAICharacterBrain : AICharacterBrain {

        public override void Controls(Character targetCharacter) {
            int
                index = rng.Next(Weapons.Count),
                weaponIndex = rng.Next(Weapons[index].Count);
            UseWeapon(index, weaponIndex, targetCharacter);
        }
    }
}
