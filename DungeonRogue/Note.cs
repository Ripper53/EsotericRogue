using EsotericRogue;
using System.Collections.Generic;

namespace DungeonRogue {
    public class Note : Item {
        private string name;
        public override string Name => name;
        public void SetName(string name) => this.name = name + " Note";
        public Sprite[] Sprites;

        public override bool Consumable => false;

        public Note(string name, Sprite[] sprites) {
            SetName(name);
            Sprites = sprites;
        }

        protected override bool UseAction(Character character) {
            return false;
        }

        public override IEnumerable<Sprite> GetDescription() {
            throw new System.NotImplementedException();
        }
    }
}
