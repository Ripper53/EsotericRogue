using System;
using System.Collections.Generic;
using EsotericRogue;

namespace DungeonRogue {
    public class DungeonSceneGenerator : SceneGenerator {
        private static readonly Random rng = new Random();

        public int MinNumberOfRoom = 3, MaxNumberOfRoom = 10;
        public Vector2 MinRoomSize = new Vector2(2, 2), MaxRoomSize = new Vector2(10, 10);

        public DungeonSceneGenerator(Scene scene) : base(scene) { }

        public override void Generate() {
            Scene.Reset();
            int roomCount = rng.Next(MinNumberOfRoom, MaxNumberOfRoom);
            Vector2[] roomPositions = new Vector2[roomCount];
            for (int i = 0; i < roomCount; i++) {
                Vector2 pos = new Vector2(rng.Next(Scene.Size.x), rng.Next(Scene.Size.y));
                roomPositions[i] = pos;
                GenerateBox(pos, new Vector2(rng.Next(MinRoomSize.x, MaxRoomSize.x), rng.Next(MinRoomSize.y, MaxRoomSize.y)));
            }
            for (int i = 1; i < roomCount; i++) {
                GeneratePath(roomPositions[i - 1], roomPositions[i]);
            }
            GeneratePath(roomPositions[0], roomPositions[roomCount - 1]);

            List<Vector2> openPositions = new List<Vector2>(Scene.Size.x * Scene.Size.y);
            Vector2 GetRandomPosition() {
                int index = rng.Next(openPositions.Count);
                Vector2 pos = openPositions[index];
                openPositions.RemoveAt(index);
                return pos;
            }
            for (int y = 0; y < Scene.Size.y; y++) {
                for (int x = 0; x < Scene.Size.x; x++) {
                    Vector2 pos = new Vector2(x, y);
                    switch (Scene.GetTile(pos)) {
                        case Scene.Tile.Ground:
                            openPositions.Add(new Vector2(x, y));
                            break;
                    }
                }
            }

            Vector2 end = GetRandomPosition();
            Scene.SetTile(Scene.Tile.Exit, end);

            SpawnBandit(GetRandomPosition());

            SetPlayerUnit(GetRandomPosition());
        }

        private void SpawnBandit(Vector2 position) {
            AIUnitBrain unitBrain = new AIUnitBrain() {
                ControlsActions = new AIUnitBrain.ControlsAction[] {
                    Follow
                }
            };
            AICharacterBrain characterBrain = new RandomAICharacterBrain() {
                Weapons = new Weapon[] {
                    new BareWeapon()
                }
            };

            Unit unit = new Unit(new Character(3, characterBrain, new BareWeapon()) {
                Name = "Bandit"
            }, unitBrain) {
                Sprite = new Sprite("*")
            };
            Scene.SetUnit(unit, position);
        }

        #region AI Unit Brain Controls
        private void Follow(AIUnitBrain source) {
            Vector2 dir = PlayerUnit.Position - source.Unit.Position;
            bool moveX() {
                if (dir.x > 0)
                    return source.Move(source.Unit.RightPosition);
                else
                    return source.Move(source.Unit.LeftPosition);
            }
            bool moveY() {
                if (dir.y > 0)
                    return source.Move(source.Unit.DownPosition);
                else
                    return source.Move(source.Unit.UpPosition);
            }
            if (Math.Abs(dir.x) > Math.Abs(dir.y)) {
                if (!moveX())
                    moveY();
            } else {
                if (!moveY())
                    moveX();
            }
        }

        #endregion

        #region AI Character Brain Controls

        #endregion

    }
}
