using EsotericRogue;
using System;
using System.Collections.Generic;
using System.Text;

namespace DungeonRogue.Units {
    public class GhostAIUnitBrain : FlyAIUnitBrain, AIUnitBrain.IView, AIUnitBrain.IPathfinder {
        public HashSet<Vector2> View { get; set; }
        public LinkedList<Vector2> Path { get; set; }

        private class GhostWeapon : Weapon<GhostWeapon> {
            public override string Name => "Bare";
            private readonly Weapon.Action[] actions = new Weapon.Action[] {

            };
            protected override IReadOnlyList<Weapon.Action> Actions => actions;


        }

        private readonly RandomPositionGetter<GhostAIUnitBrain> randomPositionGetter;

        public GhostAIUnitBrain() {
            ArsenalAICharacterBrain characterBrain = new ArsenalAICharacterBrain();
            GroundedSprite = new Sprite("g");
            FlyingSprite = new Sprite("g");
            new MemoryUnit(CharacterUtility.Create(20, characterBrain), this) {
                Sprite = GroundedSprite
            };

            Weapon weapon = new GhostWeapon();
            Character character = Unit.Character;
            character.Weapon.Equipped = weapon;

            character.Mana.IncreaseMax(10);
            character.Mana.Add(10);

            characterBrain.Arsenal = new Weapon[] {
                weapon
            };

            randomPositionGetter = new RandomPositionGetter<GhostAIUnitBrain>(this);
        }

        public override void Controls() {
            
        }

        private Vector2? target;
        public override void PreCalculate(GameManager gameManager) {
            if (IsFlying) {

            } else {
                IView.CastView(this, 0);

                Vector2 playerPos = gameManager.PlayerInfo.Unit.Position;
                if (View.Contains(playerPos)) {
                    target = playerPos;
                } else {
                    target = null;
                }

                if (Scene.GetTile(Unit.UpPosition) == Scene.Tile.Wall) {

                } else if (Scene.GetTile(Unit.RightPosition) == Scene.Tile.Wall) {

                } else if (Scene.GetTile(Unit.DownPosition) == Scene.Tile.Wall) {

                } else if (Scene.GetTile(Unit.LeftPosition) == Scene.Tile.Wall) {

                }
            }
        }
    }
}
