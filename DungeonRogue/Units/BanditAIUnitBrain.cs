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

            character.Inventory.Gold = rng.GetInt(1, 6);
            Weapon weapon = new SteelSwordWeapon();
            character.Weapon.Equipped = weapon;
            character.Inventory.AddItem(weapon);

            characterBrain.Arsenal = new Weapon[] {
                weapon
            };
        }

        public override void Controls() {
            if (Path != null && Path.First.Next != null)
                Move(Path.First.Next.Value);
        }

        private Vector2? target;
        private void GetRandomTarget() => target = Scene.GroundPositions[rng.GetInt(Scene.GroundPositions.Count)];
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
