using System.Collections.Generic;
using EsotericRogue;
using DungeonRogue.Weapons;
using DungeonRogue.Boots;
using DungeonRogue.Chestplates;
using DungeonRogue.Sleeves;
using DungeonRogue.Pants;
using DungeonRogue.Helmets;

namespace DungeonRogue.Units {
    public class OrcAIUnitBrain : AIUnitBrain, AIUnitBrain.IView, AIUnitBrain.IPathfinder {
        public HashSet<Vector2> View { get; set; }
        public LinkedList<Vector2> Path { get; set; }

        public OrcAIUnitBrain() {
            ArsenalAICharacterBrain characterBrain = new ArsenalAICharacterBrain();
            new MemoryUnit(CharacterUtility.Create(10, characterBrain), this) {
                Sprite = new Sprite("Ö")
            };

            Character character = Unit.Character;
            character.Name = "Orc";
            character.Stamina.IncreaseMax(5);
            character.Stamina.Add(5);
            character.Stamina.Regen = 1;

            character.Boot.BareItem.Speed = 2;
            character.Chestplate.BareItem.PhysicalDefense = 10;
            character.Pants.BareItem.PhysicalDefense = 5;

            Weapon weapon = new TreeHammerWeapon();
            character.Weapon.Equipped = weapon;
            character.Inventory.AddItem(weapon);

            characterBrain.Arsenal = new Weapon[] {
                weapon
            };

            dir = (Direction)rng.GetInt(4);
        }

        public override void Controls() {
            if (target.HasValue && Path != null && Path.First.Next != null)
                Move(Path.First.Next.Value);
            else if (moveTo != Unit.Position)
                Move(moveTo);
        }

        private Vector2? target;
        private Vector2 moveTo;
        private enum Direction { Up, Right, Down, Left };
        private Direction dir;
        public override void PreCalculate(GameManager gameManager) {
            CastViewBrain.CastView(this, 0);

            Vector2 playerPos = gameManager.PlayerInfo.Unit.Position;
            if (View.Contains(playerPos)) {
                int dis = Vector2.Distance(playerPos, Unit.Position);
                if (dis < 10)
                    target = playerPos;
            }

            if (target.HasValue && target != Unit.Position)
                IPathfinder.FindAStarPath(this, target.Value);
            else {
                target = null;
                switch (dir) {
                    case Direction.Up:
                        dir = Direction.Right;
                        moveTo = Unit.UpPosition;
                        break;
                    case Direction.Right:
                        dir = Direction.Down;
                        moveTo = Unit.RightPosition;
                        break;
                    case Direction.Down:
                        dir = Direction.Left;
                        moveTo = Unit.DownPosition;
                        break;
                    default:
                        dir = Direction.Up;
                        moveTo = Unit.LeftPosition;
                        break;
                }
            }
        }
    }
}
