using System;
using System.Collections.Generic;

namespace EsotericRogue {
    public class Scene {
        public static Sprite
            BorderSprite,
            GroundSprite,
            WallSprite,
            ExitSprite,
            FogSprite;

        static Scene() {
            GroundSprite = new Sprite(" ");
            WallSprite = new Sprite(" ", background: ConsoleColor.DarkGray);
            BorderSprite = WallSprite;
            ExitSprite = new Sprite("O", foreground: ConsoleColor.DarkMagenta);
            FogSprite = new Sprite(" ", background: ConsoleColor.Black);
        }

        public enum Tile {
            Wall, Ground, Exit
        };

        public Vector2 Size { get; private set; }
        private readonly Dictionary<Vector2, HashSet<Unit>> unitsMap = new Dictionary<Vector2, HashSet<Unit>>();
        private void RemoveUnitsMap(Unit unit) {
            if (unitsMap[unit.Position].Count == 1)
                unitsMap.Remove(unit.Position);
            else
                unitsMap[unit.Position].Remove(unit);
        }
        private Tile[,] tilesMap;
        private readonly HashSet<Unit> units = new HashSet<Unit>();
        public IReadOnlyCollection<Unit> Units => units;
        private readonly List<Vector2> groundPositions = new List<Vector2>();
        public IReadOnlyList<Vector2> GroundPositions => groundPositions;

        public IReadOnlyCollection<Unit> GetUnits(Vector2 position) {
            if (unitsMap.ContainsKey(position))
                return unitsMap[position];
            return null;
        }
        public Tile GetTile(Vector2 position) => tilesMap[position.x, position.y];
        public void SetUnit(Unit unit, Vector2 position) {
            unit.Position = position;
            if (unit.Brain != null)
                unit.Brain.Scene = this;
            if (unitsMap.ContainsKey(position)) {
                unitsMap[position].Add(unit);
            } else {
                unitsMap.Add(position, new HashSet<Unit>() {
                    unit
                });
            }
            units.Add(unit);
            //Renderer.Display(unit.Sprite, position + new Vector2(1, 1)); ? Don't display unit since it might be in fog!
        }
        public void SetTile(Tile tile, Vector2 position) {
            switch (tilesMap[position.x, position.y]) {
                case Tile.Ground:
                    groundPositions.Add(position);
                    break;
            }
            tilesMap[position.x, position.y] = tile;
            switch (tile) {
                case Tile.Ground:
                    groundPositions.Add(position);
                    break;
            }
        }

        public void SetSize(Vector2 size) {
            Size = size;
            tilesMap = new Tile[size.x, size.y];
        }

        public void Reset() {
            units.Clear();
            unitsMap.Clear();
            for (int y = 0; y < Size.y; y++) {
                for (int x = 0; x < Size.x; x++) {
                    tilesMap[x, y] = Tile.Wall;
                }
            }
        }

        public bool InBounds(Vector2 position) {
            return
                position.x >= 0 && position.y >= 0 &&
                position.x < Size.x && position.y < Size.y;
        }

        public Sprite GetTileSprite(Vector2 position) {
            switch (tilesMap[position.x, position.y]) {
                case Tile.Wall:
                    return WallSprite;
                case Tile.Ground:
                    return GroundSprite;
                default:
                    return ExitSprite;
            }
        }

        private void DisplaySprite(Sprite sprite, Vector2 position) {
            // Add (1, 1) to offset by border.
            Renderer.Display(sprite, position + new Vector2(1, 1));
        }

        public void DisplayFog(Vector2 position) {
            DisplaySprite(FogSprite, position);
        }

        public void DisplayTile(Vector2 position) {
            DisplaySprite(GetTileSprite(position), position);
        }

        public void DisplayUnit(Vector2 position) {
            IReadOnlyCollection<Unit> units = GetUnits(position);
            if (units != null) {
                foreach (Unit unit in units)
                    DisplaySprite(unit.Sprite, unit.Position);
            }
        }

        public void DestroyUnit(Unit unit) {
            if (units.Remove(unit)) {
                RemoveUnitsMap(unit);
                //DisplayTile(unit.Position);
            }
        }

        #region Internal
        internal void MoveUnit(Unit unit, Vector2 position) {
            RemoveUnitsMap(unit);
            //DisplayTile(oldPosition); ? Don't display tile, since the tile might not be in sight of player!
            SetUnit(unit, position);
        }
        #endregion

    }
}
