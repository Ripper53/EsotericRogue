using System.Collections.Generic;
using EsotericRogue;
using DungeonRogue.Weapons;
using DungeonRogue.Boots;
using DungeonRogue.Chestplates;
using DungeonRogue.Sleeves;
using DungeonRogue.Pants;

namespace DungeonRogue.Units {
    public class OrcAIUnitBrain : AIUnitBrain, AIUnitBrain.IView, AIUnitBrain.IPathfinder {
        public HashSet<Vector2> View { get; set; }
        public LinkedList<Vector2> Path { get; set; }

        private class OrcAICharacterBrain : ArsenalAICharacterBrain {

            public override void Controls(Character enemyCharacter) {
                Controls(this, enemyCharacter);
            }
        }

        public OrcAIUnitBrain() {
            OrcAICharacterBrain characterBrain = new OrcAICharacterBrain();
            new Unit(
                new Character(
                    10,
                    characterBrain,
                    new TreeHammerWeapon(),
                    new HikerBoot(),
                    new SteelChestplate(),
                    new RubberSleeve(),
                    new ClothPants()
                ),
                this
            ) {
                Sprite = new Sprite("Ö")
            };

            Character character = Unit.Character;
            character.Name = "Orc";
            character.Stamina.IncreaseMax(5);
            character.Stamina.Add(5);
            character.Stamina.Regen = 1;

            characterBrain.Arsenal = new Weapon[] {
                character.Weapon.BareItem
            };

            dir = (Direction)rng.Next(4);
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
                FindAStarPath(this, target.Value);
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
