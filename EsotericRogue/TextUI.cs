using System.Collections.Generic;

namespace EsotericRogue {
    public class TextUI : UI {
        public IEnumerable<Sprite> Sprites;

        protected override void DisplayUI() {
            if (Sprites != null) {
                foreach (Sprite sprite in Sprites)
                    Renderer.Add(sprite);
            }
        }
    }
}
