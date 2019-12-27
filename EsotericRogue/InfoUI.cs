using System;
using System.Collections.Generic;
using System.Text;

namespace EsotericRogue {
    public class InfoUI : UI {
        public readonly Sprite NameSprite;
        public IEnumerable<Sprite> Sprites;
        public readonly Character Character;
        public int OffsetY;

        private const int maxWidth = 20;

        public InfoUI(Character character) {
            NameSprite = new Sprite(character.Name);
            Character = character;
            OffsetY = 1;
            Activated += source => {
                Character.HealthChanged += Character_HealthChanged;
                Character.Stamina.ValueChanged += Stamina_ValueChanged;
                Character.Mana.ValueChanged += Mana_ValueChanged;
                Character.Energy.ValueChanged += Energy_ValueChanged;
                Character.Weapon.ItemEquipped += Character_WeaponEquipped;
            };
            Deactivated += source => {
                Character.HealthChanged -= Character_HealthChanged;
                Character.Stamina.ValueChanged -= Stamina_ValueChanged;
                Character.Mana.ValueChanged -= Mana_ValueChanged;
                Character.Energy.ValueChanged -= Energy_ValueChanged;
                Character.Weapon.ItemEquipped -= Character_WeaponEquipped;
            };
        }

        #region Events
        private static Sprite GetHealthSprite(Character character) => GetSprite(character.Health, character.MaxHealth);
        private Vector2 GetHealthPosition(Vector2 position) => position + new Vector2(8, OffsetY);
        private void Character_HealthChanged(Character character, int oldHealth) {
            Renderer.Display(GetSprite(character.Health, character.MaxHealth), GetHealthPosition(Position));
        }
        private Vector2 GetStaminaPosition(Vector2 position) => position + new Vector2(8, OffsetY + 1);
        private void Stamina_ValueChanged(Resource resource, int oldValue) {
            Renderer.Display(GetSprite(resource), GetStaminaPosition(Position));
        }
        private Vector2 GetManaPosition(Vector2 position) => position + new Vector2(8, OffsetY + 2);
        private void Mana_ValueChanged(Resource resource, int oldValue) {
            Renderer.Display(GetSprite(resource), GetManaPosition(Position));
        }
        private Vector2 GetEnergyPosition(Vector2 position) => position + new Vector2(8, OffsetY + 3);
        private void Energy_ValueChanged(Resource resource, int oldValue) {
            Renderer.Display(GetSprite(resource), GetEnergyPosition(Position));
        }
        private Vector2 GetWeaponEquippedPosition(Vector2 position) => position + new Vector2(8, OffsetY + 5);
        private string GetWeaponName(Weapon weapon) => GetContinuedString(weapon.Name.PadRight(lastWeaponName.Length), maxWidth - 8);
        private string lastWeaponName = string.Empty;
        private void Character_WeaponEquipped(Character character, Weapon weapon, Weapon oldWeapon) {
            lastWeaponName = GetWeaponName(weapon);
            Renderer.Display(Sprite.CreateUI(lastWeaponName), GetWeaponEquippedPosition(Position));
        }
        #endregion

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

        protected override void DisplayUI() {
            NameSprite.Display = GetStringPadRight(GetContinuedString(Character.Name, maxWidth), maxWidth) + Environment.NewLine;
            Renderer.Add(NameSprite);
            if (Sprites != null) {
                foreach (Sprite s in Sprites)
                    Renderer.Add(s);
            }
            Renderer.Add(Character.HealthSprite);
            Renderer.Add("  " + Environment.NewLine);
            Renderer.Add(Character.StaminaSprite);
            Renderer.Add(" " + Environment.NewLine);
            Renderer.Add(Character.ManaSprite);
            Renderer.Add("    " + Environment.NewLine);
            Renderer.Add(Character.EnergySprite);
            Renderer.Add("  " + Environment.NewLine);

            Renderer.Add(new Sprite("Equipment" + Environment.NewLine));
            Renderer.Add(new Sprite("Weapon: "));

            Character_HealthChanged(Character, Character.Health);
            Stamina_ValueChanged(Character.Stamina, Character.Stamina.Value);
            Mana_ValueChanged(Character.Mana, Character.Mana.Value);
            Energy_ValueChanged(Character.Energy, Character.Energy.Value);
            Character_WeaponEquipped(Character, Character.Weapon.Equipped, Character.Weapon.Equipped);
        }

        public override void Clear() {
            base.Clear();
            Renderer.Display(Sprite.CreateEmptyUI(GetHealthSprite(Character).Display.Length), GetHealthPosition(ClearPosition));
            Renderer.Display(Sprite.CreateEmptyUI(GetSprite(Character.Stamina).Display.Length), GetStaminaPosition(ClearPosition));
            Renderer.Display(Sprite.CreateEmptyUI(GetSprite(Character.Mana).Display.Length), GetManaPosition(ClearPosition));
            Renderer.Display(Sprite.CreateEmptyUI(GetSprite(Character.Energy).Display.Length), GetEnergyPosition(ClearPosition));
            Renderer.Display(Sprite.CreateEmptyUI(GetWeaponName(Character.Weapon.Equipped).Length), GetWeaponEquippedPosition(ClearPosition));
        }
    }
}
