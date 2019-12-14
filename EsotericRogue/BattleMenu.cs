using System;
using System.Collections.Generic;

namespace EsotericRogue {
    public class BattleMenu : Menu {
        public readonly GameManager GameManager;
        public readonly PlayerInfo PlayerInfo;
        public readonly Character PlayerCharacter;

        private bool playerAlive = true, enemyTurn = false;
        public BattleMenu(GameManager gameManager, PlayerInfo playerInfo) {
            GameManager = gameManager;
            PlayerInfo = playerInfo;
            PlayerCharacter = PlayerInfo.Character;
            PlayerCharacter.Died += source => playerAlive = false;
            Position = new Vector2(0, 6);
            Options = new List<Option>(2) {
                new Option() {
                    Sprites = new Sprite[] {
                        CreateSprite("Skip")
                    },
                    Action = (menu, op) => {
                        enemyTurn = true;
                        PlayerInfo.CharacterBrain.Description = new Sprite[] {
                            CreateSprite("Skip.")
                        };
                    }
                }
            };
        }

        /// <summary>
        /// Returns true if battle was won, otherwise false.
        /// </summary>
        public bool Battle(Character character) {
            // Prep
            GameManager.AddUI(this);

            TextUI vsText = new TextUI() {
                Sprites = new Sprite[] {
                    new Sprite("vs", ConsoleColor.White, ConsoleColor.Black)
                },
                Position = new Vector2(18, 0)
            };
            GameManager.AddUI(vsText);

            const int playerDescriptionPosX = 64, enemyDescriptionPosX = 40;
            TextUI playerNameDescriptionText = new TextUI() {
                Sprites = new Sprite[] {
                    new Sprite(PlayerCharacter.Name + " Actions:")
                },
                Position = new Vector2(playerDescriptionPosX, 0)
            };
            GameManager.AddUI(playerNameDescriptionText);

            TextUI enemyNameDescriptionText = new TextUI() {
                Sprites = new Sprite[] {
                    new Sprite(character.Name + " Actions:")
                },
                Position = new Vector2(enemyDescriptionPosX, 0)
            };
            GameManager.AddUI(enemyNameDescriptionText);

            TextUI playerDescriptionText = new TextUI() {
                Position = new Vector2(playerDescriptionPosX, 1),
                Width = 20
            };
            GameManager.AddUI(playerDescriptionText);

            TextUI enemyDescriptionText = new TextUI() {
                Position = new Vector2(enemyDescriptionPosX, 1),
                Width = 20
            };
            GameManager.AddUI(enemyDescriptionText);


            InfoUI enemyInfo = new InfoUI(character) {
                Position = new Vector2(21, 0)
            };
            GameManager.AddUI(enemyInfo);

            Vector2 previousInfoUIPos = GameManager.PlayerInfo.InfoUI.Position;
            GameManager.PlayerInfo.InfoUI.Position = new Vector2(0, 0);
            Vector2 previousInventoryMenuPos = GameManager.PlayerInfo.InventoryMenu.Position;
            GameManager.PlayerInfo.InventoryMenu.Position = new Vector2(84, 0);

            // Battle
            GameManager.Display();
            bool enemyAlive = true;
            character.Died += source => enemyAlive = false;
            while (enemyAlive) {
                if (!playerAlive) {
                    GameManager.RemoveUI(this);
                    Renderer.Clear();
                    GameManager.DisplayUI();
                    Renderer.Display(
                        CreateSprite("Game Over!" + Environment.NewLine),
                        new Vector2(0, 7)
                    );
                    Renderer.ReadLine();
                    return false;
                }
                if (PlayerInfo.Input.SelectedUI == null)
                    PlayerInfo.Input.SelectedUI = this;
                PlayerCharacter.Brain.Controls(character);
                if (enemyAlive && enemyTurn) {
                    enemyTurn = false;
                    character.Brain.Controls(PlayerCharacter);
                    enemyDescriptionText.Sprites = character.Brain.GetDescription();
                    enemyDescriptionText.Display();
                    playerDescriptionText.Sprites = PlayerCharacter.Brain.GetDescription();
                    playerDescriptionText.Display();
                }
            }

            // Drop
            PlayerCharacter.Inventory.Gold += character.Inventory.Gold;

            // Clean up
            GameManager.RemoveUI(this);
            GameManager.PlayerInfo.InfoUI.Position = previousInfoUIPos;
            GameManager.PlayerInfo.InventoryMenu.Position = previousInventoryMenuPos;
            GameManager.RemoveUI(enemyInfo);
            GameManager.RemoveUI(vsText);
            GameManager.RemoveUI(playerNameDescriptionText);
            GameManager.RemoveUI(enemyNameDescriptionText);
            GameManager.RemoveUI(playerDescriptionText);
            GameManager.RemoveUI(enemyDescriptionText);
            return true;
        }
    }
}
