using System;
using System.Collections.Generic;

namespace EsotericRogue {
    public class Scene {
        public static Sprite
            BorderSprite,
            GroundSprite,
            WallSprite,
            ExitSprite;

        static Scene() {
            GroundSprite = new Sprite(" ");
            WallSprite = new Sprite(" ", background: ConsoleColor.Black);
            BorderSprite = WallSprite;
            ExitSprite = new Sprite("X");
        }

        public enum Tile {
            Wall, Ground, Exit
        };

        public Vector2 Size { get; private set; }
        private Unit[,] unitsMap;
        private Tile[,] tilesMap;
        private readonly HashSet<Unit> units = new HashSet<Unit>();
        public IEnumerable<Unit> Units => units;

        public Unit GetUnit(Vector2 position) => unitsMap[position.x, position.y];
        public Tile GetTile(Vector2 position) => tilesMap[position.x, position.y];
        public void SetUnit(Unit unit, Vector2 position) {
            unitsMap[position.x, position.y] = unit;
            unit.Position = position;
            unit.Brain.Scene = this;
            units.Add(unit);
            Renderer.Display(unit.Sprite, position + new Vector2(1, 1));
        }
        public void SetTile(Tile tile, Vector2 position) => tilesMap[position.x, position.y] = tile;

        public void SetSize(Vector2 size) {
            Size = size;
            unitsMap = new Unit[size.x, size.y];
            tilesMap = new Tile[size.x, size.y];
        }

        public void Reset() {
            units.Clear();
            for (int y = 0; y < Size.y; y++) {
                for (int x = 0; x < Size.x; x++) {
                    unitsMap[x, y] = null;
                    tilesMap[x, y] = Tile.Wall;
                }
            }
        }

        public bool InBounds(Vector2 position) {
            return
                position.x >= 0 && position.y >= 0 &&
                position.x < Size.x && position.y < Size.y;
        }

        public void Display() {
            // Offset by 2 because of left border and Size.x + 1 to reach the right side.
            for (int x = -2; x < Size.x; x++)
                Renderer.Add(BorderSprite);
            Renderer.Add(Environment.NewLine);
            for (int y = 0; y < Size.y; y++) {
                Renderer.Add(BorderSprite);
                for (int x = 0; x < Size.x; x++) {
                    Unit unit = unitsMap[x, y];
                    if (unit != null) {
                        Renderer.Add(unit.Sprite);
                    } else {
                        Renderer.Add(GetTileSprite(x, y));
                    }
                }
                Renderer.Add(BorderSprite);
                Renderer.Add(Environment.NewLine);
            }
            for (int x = -2; x < Size.x; x++)
                Renderer.Add(BorderSprite);
            Renderer.Add(Environment.NewLine);
            Renderer.Clear();
            Renderer.Display();
        }

        private Sprite GetTileSprite(int x, int y) {
            switch (tilesMap[x, y]) {
                case Tile.Wall:
                    return WallSprite;
                case Tile.Ground:
                    return GroundSprite;
                default:
                    return ExitSprite;
            }
        }

        private void DisplayTile(Vector2 position) {
            // Add (1, 1) to offset by border.
            Renderer.Display(GetTileSprite(position.x, position.y), position + new Vector2(1, 1));
        }

        public void DestroyUnit(Unit unit) {
            if (units.Remove(unit)) {
                unitsMap[unit.Position.x, unit.Position.y] = null;
                DisplayTile(unit.Position);
            }
        }

        #region Internal
        internal bool MoveUnit(Unit unit, Vector2 position) {
            if (InBounds(position) && tilesMap[position.x, position.y] != Tile.Wall) {
                Vector2 oldPosition = unit.Position;
                unitsMap[oldPosition.x, oldPosition.y] = null;
                DisplayTile(oldPosition);
                SetUnit(unit, position);
                return true;
            }
            return false;
        }
        #endregion

    }
}
