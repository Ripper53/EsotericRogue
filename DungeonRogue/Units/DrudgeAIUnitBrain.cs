using System;
using System.Collections.Generic;
using System.Text;
using EsotericRogue;

namespace DungeonRogue.Units {
    public class DrudgeAIUnitBrain : AIUnitBrain {

        public DrudgeAIUnitBrain() {
            ArsenalAICharacterBrain characterBrain = new ArsenalAICharacterBrain();
            new MemoryUnit(CharacterUtility.Create(50, characterBrain), this) {
                Sprite = new Sprite("Ɗ")
            };
        }

        public override void Controls() {
            throw new NotImplementedException();
        }

        public override void PreCalculate(GameManager gameManager) {
            throw new NotImplementedException();
        }
    }
}
