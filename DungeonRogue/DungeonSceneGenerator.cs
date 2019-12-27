using System;
using System.Collections.Generic;
using EsotericRogue;
using DungeonRogue.Weapons;
using DungeonRogue.Boots;
using DungeonRogue.Chestplates;
using DungeonRogue.Sleeves;
using DungeonRogue.Pants;

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
                Vector2
                    pos = new Vector2(rng.Next(Scene.Size.x), rng.Next(Scene.Size.y)),
                    size = new Vector2(rng.Next(MinRoomSize.x, MaxRoomSize.x), rng.Next(MinRoomSize.y, MaxRoomSize.y));
                Vector2 bounds = Scene.Size - (pos + size);
                if (bounds.x <= 0)
                    size.x = 0;
                if (bounds.y <= 0)
                    size.y = 0;
                roomPositions[i] = pos + new Vector2(size.x > 0 ? rng.Next(size.x) : 0, size.y > 0 ? rng.Next(size.y) : 0);
                GenerateBox(pos, size);
            }
            for (int i = 1; i < roomCount; i++) {
                GeneratePath(roomPositions[i - 1], roomPositions[i]);
            }
            GeneratePath(roomPositions[0], roomPositions[roomCount - 1]);

            List<Vector2>
                groundPositions = new List<Vector2>(Scene.Size.x * Scene.Size.y),
                wallPositions = new List<Vector2>(Scene.Size.x * Scene.Size.y);

            static Vector2? GetRandomPosition(List<Vector2> tileList) {
                if (tileList.Count > 0) {
                    int index = rng.Next(tileList.Count);
                    Vector2 pos = tileList[index];
                    tileList.RemoveAt(index);
                    return pos;
                }
                return null;
            }
            Vector2? GetRandomGroundPosition() => GetRandomPosition(groundPositions);
            Vector2? GetRandomWallPosition() => GetRandomPosition(wallPositions);
            for (int y = 0; y < Scene.Size.y; y++) {
                for (int x = 0; x < Scene.Size.x; x++) {
                    Vector2 pos = new Vector2(x, y);
                    switch (Scene.GetTile(pos)) {
                        case Scene.Tile.Ground:
                            groundPositions.Add(pos);
                            break;
                        case Scene.Tile.Wall:
                            wallPositions.Add(pos);
                            break;
                    }
                }
            }

            Vector2? start = GetRandomGroundPosition();
            if (start.HasValue)
                SetPlayerUnit(start.Value);
            else {
                start = GetRandomWallPosition();
                if (start.HasValue) {
                    Scene.SetTile(Scene.Tile.Ground, start.Value);
                    SetPlayerUnit(start.Value);
                } else {
                    return;
                }
            }
            Vector2? end = GetRandomGroundPosition();
            if (end.HasValue)
                Scene.SetTile(Scene.Tile.Exit, end.Value);
            else {
                end = GetRandomWallPosition();
                if (end.HasValue) {
                    GeneratePath(start.Value, end.Value);
                    Scene.SetTile(Scene.Tile.Exit, end.Value);
                }
            }

            for (int i = 0; i < 1; i++) {
                Vector2? pos = GetRandomGroundPosition();
                if (pos.HasValue)
                    SpawnBandit(pos.Value);
                else
                    break;
            }
        }

        private void SpawnBandit(Vector2 position) {
            AIUnitBrain unitBrain = new AIUnitBrain() {
                ControlsActions = new AIUnitBrain.ControlsAction[] {
                    Follow
                }
            };
            Weapon bareWeapon = new SteelSwordWeapon();
            AICharacterBrain characterBrain = new RandomAICharacterBrain() {
                Weapons = new Weapon[] {
                    bareWeapon
                }
            };

            Unit unit = new Unit(new Character(3, characterBrain, bareWeapon, new BareBoot(), new BareChestplate(), new BareSleeve(), new BarePants()) {
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
            if (MathF.Abs(dir.x) > MathF.Abs(dir.y)) {
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
