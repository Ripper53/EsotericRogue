using System.Collections.Generic;
using EsotericRogue;
using DungeonRogue.Weapons;

namespace DungeonRogue.Units {
    public class BanditAIUnitBrain : AIUnitBrain, AIUnitBrain.IView, AIUnitBrain.IPathfinder {
        public HashSet<Vector2> View { get; set; }
        public LinkedList<Vector2> Path { get; set; }

        public BanditAIUnitBrain() {
            ArsenalAICharacterBrain characterBrain = new ArsenalAICharacterBrain();
            new MemoryUnit(CharacterUtility.Create(3, characterBrain), this) {
                Sprite = new Sprite("ƃ")
            };
            Character character = Unit.Character;
            character.Name = "Bandit";
            character.Stamina.IncreaseMax(2);
            character.Stamina.Add(2);
            character.Stamina.Regen = 1;

            character.Boot.BareItem.Speed = 2;

            character.Inventory.Gold = RandomUtility.GetInt(1, 6);
            Weapon weapon = new SteelSwordWeapon();
            character.Weapon.Equipped = weapon;
            character.Inventory.AddItem(weapon);

            characterBrain.Arsenal = new Weapon[] {
                weapon
            };

            randomPositionGetter = new RandomPositionGetter<BanditAIUnitBrain>(this);
        }

        public override void Controls() {
            MoveToPath(this);
        }

        private readonly RandomPositionGetter<BanditAIUnitBrain> randomPositionGetter;
        public override void PreCalculate(GameManager gameManager) {
            IView.CastView(this, 0);
            Vector2 playerPos = gameManager.PlayerInfo.Unit.Position, target;
            if (View.Contains(playerPos)) {
                target = playerPos;
            } else {
                target = randomPositionGetter.GetTarget();
            }
            IPathfinder.FindAStarPath(this, target);
        }
    }
}
