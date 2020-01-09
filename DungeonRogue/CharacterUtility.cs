using System;
using System.Collections.Generic;
using System.Text;
using DungeonRogue.Boots;
using DungeonRogue.Chestplates;
using DungeonRogue.Helmets;
using DungeonRogue.Pants;
using DungeonRogue.Sleeves;
using DungeonRogue.Weapons;
using EsotericRogue;

namespace DungeonRogue {
    public static class CharacterUtility {

        public static Character Create(int health, CharacterBrain characterBrain) {
            return new Character(
                health,
                characterBrain,
                new BareWeapon(),
                new BareBoot(),
                new BareHelmet(),
                new BareChestplate(),
                new BareSleeve(),
                new BarePants()
            );
        }
    }
}
