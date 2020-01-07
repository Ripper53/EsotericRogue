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

        public int MinNumberOfRoom = 10, MaxNumberOfRoom = 20;
        public Vector2 MinRoomSize = new Vector2(2, 2), MaxRoomSize = new Vector2(10, 10);

        public DungeonSceneGenerator(Scene scene) : base(scene) { }

        private int dungeonNumber = 0;
        public override void Generate() {
            dungeonNumber++;
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

            Spawn(GetRandomGroundPosition);
        }

        #region Spawns
        private delegate Vector2? GetGroundPositionFunc();
        public delegate void SpawnAction(Vector2 position);
        private readonly List<SpawnAction> spawnActions = new List<SpawnAction>();
        private void Spawn(GetGroundPositionFunc getGroundPositionFunc) {
            int numberOfEnemies = dungeonNumber;
            if (numberOfEnemies > 20)
                numberOfEnemies = rng.Next(2, 21);

            switch (dungeonNumber) {
                case 2:
                    spawnActions.Add(SpawnOrc);
                    break;
                case 3:
                    spawnActions.Add(SpawnFireTroll);
                    break;
                default:
                    spawnActions.Add(SpawnBandit);
                    break;
            }

            for (int i = 0, spawnCount = spawnActions.Count; i < numberOfEnemies; i++) {
                Vector2? position = getGroundPositionFunc();
                if (position.HasValue)
                    spawnActions[rng.Next(spawnCount)](position.Value);
                else
                    break;
            }
        }
        private void SpawnBandit(Vector2 position) => Scene.SetUnit(new BanditAIUnitBrain().Unit, position);
        private void SpawnOrc(Vector2 position) => Scene.SetUnit(new OrcAIUnitBrain().Unit, position);
        private void SpawnFireTroll(Vector2 position) => Scene.SetUnit(new FireTrollAIUnitBrain().Unit, position);

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
