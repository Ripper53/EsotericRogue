using System;
using System.Collections.Generic;
using System.Text;

namespace EsotericRogue {
    public class InfoUI : UI {
        public readonly Sprite NameSprite;
        public IEnumerable<Sprite> Sprites;
        public readonly Character Character;
        public int OffsetY;

        public InfoUI(Character character) {
            NameSprite = new Sprite(character.Name);
            Character = character;
            OffsetY = 1;
            Init();
        }

        private void SetName() {
            int newLineLength = Environment.NewLine.Length;
            StringBuilder stringBuilder = new StringBuilder(Character.Name, Character.Name.Length + newLineLength);
            const int maxLength = 17;
            if (stringBuilder.Length > maxLength) {
                string n = stringBuilder.ToString(0, maxLength - 1);
                stringBuilder.Clear();
                stringBuilder.Append(n);
                stringBuilder.Append(ContinuedString);
            }
            for (int i = 0, count = maxLength - stringBuilder.Length; i < count; i++)
                stringBuilder.Append(' ');
            stringBuilder.Append(Environment.NewLine);
            NameSprite.Display = stringBuilder.ToString();
        }

        private static Sprite GetSprite(Resource resource) {
            return GetSprite(resource.Value, resource.Max);
        }
        private static Sprite GetSprite(int value, int max) {
            StringBuilder stringBuilder = new StringBuilder(Resource.MaxResourceStringLength);
            string stringValue = value.ToString();
            stringBuilder.Append(stringValue);
            for (int i = 0, count = Resource.MaxResourceStringLength - stringValue.Length; i < count; i++)
                stringBuilder.Append(' ');
            stringBuilder.Append('/');
            string stringMax = max.ToString();
            stringBuilder.Append(stringMax);
            for (int i = 0, count = Resource.MaxResourceStringLength - stringMax.Length; i < count; i++)
                stringBuilder.Append(' ');
            return new Sprite(stringBuilder.ToString(), ConsoleColor.White, ConsoleColor.Black);
        }

        public void Init() {
            Character.HealthChanged += Character_HealthChanged;
            Character.Stamina.ValueChanged += Stamina_ValueChanged;
            Character.Mana.ValueChanged += Mana_ValueChanged;
            Character.Energy.ValueChanged += Energy_ValueChanged;
        }

        public void Destroy() {
            Character.HealthChanged -= Character_HealthChanged;
            Character.Stamina.ValueChanged -= Stamina_ValueChanged;
            Character.Mana.ValueChanged -= Mana_ValueChanged;
            Character.Energy.ValueChanged -= Energy_ValueChanged;
        }

        #region Events
        private void Character_HealthChanged(Character character, int oldHealth) {
            Renderer.Display(GetSprite(character.Health, character.MaxHealth), Position + new Vector2(8, OffsetY));
        }
        private void Stamina_ValueChanged(Resource resource, int oldValue) {
            Renderer.Display(GetSprite(resource), Position + new Vector2(8, OffsetY + 1));
        }
        private void Mana_ValueChanged(Resource resource, int oldValue) {
            Renderer.Display(GetSprite(resource), Position + new Vector2(8, OffsetY + 2));
        }
        private void Energy_ValueChanged(Resource resource, int oldValue) {
            Renderer.Display(GetSprite(resource), Position + new Vector2(8, OffsetY + 3));
        }
        #endregion

        protected override void DisplayUI() {
            SetName();
            Renderer.Add(NameSprite);
            if (Sprites != null) {
                foreach (Sprite sprite in Sprites)
                    Renderer.Add(sprite);
            }
            Renderer.Add(Character.HealthSprite);
            Renderer.Add(Environment.NewLine.PadLeft(4));
            Renderer.Add(Character.StaminaSprite);
            Renderer.Add(Environment.NewLine.PadLeft(3));
            Renderer.Add(Character.ManaSprite);
            Renderer.Add(Environment.NewLine.PadLeft(6));
            Renderer.Add(Character.EnergySprite);
            Renderer.Add("  ");

            Character_HealthChanged(Character, Character.Health);
            Stamina_ValueChanged(Character.Stamina, Character.Stamina.Value);
            Mana_ValueChanged(Character.Mana, Character.Mana.Value);
            Energy_ValueChanged(Character.Energy, Character.Energy.Value);
        }
    }
}
