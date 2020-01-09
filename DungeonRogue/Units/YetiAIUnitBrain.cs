using System.Collections.Generic;
using EsotericRogue;

namespace DungeonRogue.Units {
    public class YetiAIUnitBrain : AIUnitBrain {

        #region Item
        private class YetiWeapon : Weapon<YetiWeapon> {
            public override string Name => "Bare";
            private readonly Weapon.Action[] actions = new Weapon.Action[] {
                new CommonWeaponActions<YetiWeapon>.PunchAction() {
                    Damage = 4,
                    StaminaCost = 2
                },
                new CommonWeaponActions<YetiWeapon>.SmashAction() {
                    Damage = 6,
                    StaminaCost = 3
                },
                new CommonWeaponActions<YetiWeapon>.StompAction() {
                    Damage = 10,
                    StaminaCost = 5
                }
            };
            protected override IReadOnlyList<Weapon.Action> Actions => actions;
        }
        #endregion

        public YetiAIUnitBrain() {
            ArsenalAICharacterBrain characterBrain = new ArsenalAICharacterBrain();
            new Unit(CharacterUtility.Create(20, characterBrain), this) {
                Sprite = new Sprite("Ÿ")
            };

            Character character = Unit.Character;
            character.Name = "Yeti";
            character.Stamina.IncreaseMax(10);
            character.Stamina.Add(10);
            character.Mana.IncreaseMax(10);
            character.Mana.Add(10);

            character.Boot.BareItem.Speed = 2;
            character.Helmet.BareItem.PhysicalDefense = 5;
            character.Chestplate.BareItem.PhysicalDefense = 10;
            character.Chestplate.BareItem.MagicalDefense = 5;
            character.Sleeve.BareItem.PhysicalDefense = 5;
            character.Pants.BareItem.PhysicalDefense = 5;

            Weapon weapon = new YetiWeapon();
            character.Weapon.Equipped = weapon;

            characterBrain.Arsenal = new Weapon[] {
                weapon
            };
        }

        public override void Controls() {

        }

        public override void PreCalculate(GameManager gameManager) {
            
        }
    }
}
