using System.Collections.Generic;

namespace EsotericRogue {
    public class PlayerCharacterBrain : CharacterBrain {
        public PlayerInfo PlayerInfo { get; internal set; }
        public IEnumerable<Sprite> Description;

        public override void Controls(Character enemyCharacter) {
            PlayerInfo.Input.UIControls();
        }

        public override IEnumerable<Sprite> GetDescription() {
            return Description;
        }
    }
}
