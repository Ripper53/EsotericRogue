using System.Collections.Generic;
using EsotericRogue;
using DungeonRogue.Weapons;
using DungeonRogue.Boots;
using DungeonRogue.Chestplates;
using DungeonRogue.Sleeves;
using DungeonRogue.Pants;

namespace DungeonRogue.Units {
    public class BanditAIUnitBrain : AIUnitBrain, AIUnitBrain.IView, AIUnitBrain.IPathfinder {
        public HashSet<Vector2> View { get; set; }
        public LinkedList<Vector2> Path { get; set; }

        private class BanditAICharacterBrain : ArsenalAICharacterBrain {

            public override void Controls(Character enemyCharacter) {
                Controls(this, enemyCharacter);
            }
        }

        public BanditAIUnitBrain() {
            Weapon weapon = new SteelSwordWeapon();
            BanditAICharacterBrain characterBrain = new BanditAICharacterBrain();
            new Unit(new Character(3, characterBrain, weapon, new WolfFurBoot(), new WoodenChestplate(), new RubberSleeve(), new ClothPants()), this) {
                Sprite = new Sprite("ƃ")
            };
            Character character = Unit.Character;
            character.Name = "Bandit";
            character.Stamina.IncreaseMax(2);
            character.Stamina.Add(2);
            character.Stamina.Regen = 1;
            character.Inventory.Gold = rng.Next(1, 6);
            character.Inventory.AddItem(weapon);

            ArsenalAICharacterBrain.ArsenalConstructor(characterBrain);
        }

        public override void Controls() {
            if (Path != null && Path.First.Next != null)
                Move(Path.First.Next.Value);
        }

        private Vector2? target;
        private void GetRandomTarget() => target = Scene.GroundPositions[rng.Next(Scene.GroundPositions.Count)];
        public override void PreCalculate(GameManager gameManager) {
            if (!target.HasValue || Unit.Position == target) {
                GetRandomTarget();
            }

            CastViewBrain.CastView(this, 0);
            Vector2 playerPos = gameManager.PlayerInfo.Unit.Position;
            if (View.Contains(playerPos)) {
                target = playerPos;
            }
            FindAStarPath(this, target.Value);
            if (Path == null)
                GetRandomTarget();
        }
    }
}
