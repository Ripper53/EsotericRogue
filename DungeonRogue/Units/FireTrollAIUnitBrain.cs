using System.Collections.Generic;
using EsotericRogue;
using DungeonRogue.Weapons;

namespace DungeonRogue.Units {
    public class FireTrollAIUnitBrain : AIUnitBrain, AIUnitBrain.IView, AIUnitBrain.IPathfinder {
        public HashSet<Vector2> View { get; set; }
        public LinkedList<Vector2> Path { get; set; }

        public FireTrollAIUnitBrain() {
            ArsenalAICharacterBrain characterBrain = new ArsenalAICharacterBrain();
            new MemoryUnit(CharacterUtility.Create(30, characterBrain), this) {
                Sprite = new Sprite("ŧ")
            };

            Character character = Unit.Character;
            character.Name = "Fire Troll";
            character.Mana.IncreaseMax(10);
            character.Mana.Add(10);
            character.Mana.Regen = 1;

            character.Inventory.Gold = RandomUtility.GetInt(20, 26);
            Weapon weapon = new FireStaffWeapon();
            character.Weapon.Equipped = weapon;
            character.Inventory.AddItem(weapon);

            characterBrain.Arsenal = new Weapon[] {
                weapon
            };
        }

        public override void Controls() {
            
        }

        public override void PreCalculate(GameManager gameManager) {
            
        }
    }
}
