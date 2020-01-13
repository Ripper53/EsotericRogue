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

        public static ConsoleColor GetForegroundColor(DamageType damageType) {
            switch (damageType) {
                case DamageType.Physical:
                    return Character.StaminaSprite.Foreground;
                case DamageType.Magical:
                    return Character.ManaSprite.Foreground;
                case DamageType.Electrical:
                    return Character.EnergySprite.Foreground;
                default:
                    return ConsoleColor.White;
            }
        }

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
