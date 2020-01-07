using System.Collections.Generic;
using EsotericRogue;
using DungeonRogue.Boots;
using DungeonRogue.Chestplates;
using DungeonRogue.Pants;
using DungeonRogue.Sleeves;
using DungeonRogue.Weapons;

namespace DungeonRogue.Units {
    public class FireTrollAIUnitBrain : AIUnitBrain, AIUnitBrain.IView, AIUnitBrain.IPathfinder {
        public HashSet<Vector2> View { get; set; }
        public LinkedList<Vector2> Path { get; set; }

        private class FireTrollAICharacterBrain : ArsenalAICharacterBrain {

            public override void Controls(Character enemyCharacter) {
                Controls(this, enemyCharacter);
            }
        }

        public FireTrollAIUnitBrain() {
            Weapon weapon = new FireStaffWeapon();
            FireTrollAICharacterBrain characterBrain = new FireTrollAICharacterBrain();
            new Unit(new Character(
                20,
                characterBrain,
                weapon,
                new FoxFurBoot(),
                new ChainChestplate(),
                new ChainSleeve(),
                new ChainPants()
            ), this) {
                Sprite = new Sprite("ŧ")
            };

            Character character = Unit.Character;
            character.Name = "Fire Troll";
            character.Mana.IncreaseMax(10);
            character.Mana.Add(10);
            character.Mana.Regen = 1;
            character.Inventory.Gold = rng.Next(20, 26);

            ArsenalAICharacterBrain.ArsenalConstructor(characterBrain);
        }

        public override void Controls() {
            
        }

        public override void PreCalculate(GameManager gameManager) {
            
        }
    }
}
