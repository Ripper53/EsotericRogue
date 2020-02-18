using EsotericRogue;
using DungeonRogue.Weapons.Dragon;

namespace DungeonRogue.Units {
    public class RedDragonAIUnitBrain : FlyMinDistanceAIUnitBrain {

        public RedDragonAIUnitBrain() {
            ArsenalAICharacterBrain characterBrain = new ArsenalAICharacterBrain();
            GroundedSprite = new Sprite("D");
            new MemoryUnit(CharacterUtility.Create(100, characterBrain), this) {
                Sprite = GroundedSprite
            };
            FlyingSprite = new Sprite("Y");

            Character character = Unit.Character;
            character.Stamina.IncreaseMax(50);
            character.Stamina.Add(50);
            character.Mana.IncreaseMax(50);
            character.Mana.Add(50);

            Weapon weapon = new DragonWeapon();
            character.Weapon.Equipped = weapon;

            characterBrain.Arsenal = new Weapon[] {
                weapon
            };
        }
    }
}
