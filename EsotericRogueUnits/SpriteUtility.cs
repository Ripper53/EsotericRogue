using System;
using EsotericRogue;

namespace EsotericRogue {
    public static class SpriteUtility {

        public static Sprite Rarity(Rarity rarity) {
            return Sprite.CreateUI("Rarity: " + rarity + Environment.NewLine);
        }
    }
}
