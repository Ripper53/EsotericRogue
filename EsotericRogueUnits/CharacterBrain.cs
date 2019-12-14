using System.Collections.Generic;

namespace EsotericRogue {
    public abstract class CharacterBrain {
        public Character Character { get; internal set; }

        public abstract void Controls(Character enemyCharacter);
        /// <summary>
        /// Decription of what occured in Controls method.
        /// </summary>
        public abstract IEnumerable<Sprite> GetDescription();
    }
}
