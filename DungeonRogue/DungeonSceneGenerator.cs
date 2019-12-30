using System;
using System.Collections.Generic;
using EsotericRogue;
using DungeonRogue.Units;
using DungeonRogue.Weapons;
using DungeonRogue.Boots;
using DungeonRogue.Chestplates;
using DungeonRogue.Pants;
using DungeonRogue.Sleeves;

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
                    Spawn(pos.Value);
                else
                    break;
            }
            SpawnShop(GetRandomGroundPosition().Value);
        }

        #region Spawns
        private void Spawn(Vector2 position) {
            Scene.SetUnit(new BanditAIUnitBrain().Unit, position);
        }

        private void SpawnShop(Vector2 position) {
            JadeAIUnitBrain unitBrain = new JadeAIUnitBrain();
            Scene.SetUnit(unitBrain.Unit, position);
            Menu menu = ((FriendlyUnit)unitBrain.Unit).Menu;
            unitBrain.TalkSprite.Display = "I've got some items you might be interested in.";

            Inventory inven = PlayerUnit.Character.Inventory;
            int gold = inven.Gold;

            void AddItem(Item item, int goldCost) {
                menu.AddOption(new Menu.Option() {
                    Sprites = new Sprite[] { Sprite.CreateUI($"Buy {item.Name} for {goldCost} gold.") },
                    Action = (menu, op) => {
                        if (inven.Buy(item, goldCost))
                            menu.RemoveOption(op);
                    }
                });
            }

            if (gold >= 0) {
                AddItem(new ClothPants(), 10);
            }
        }
        #endregion

    }
}
