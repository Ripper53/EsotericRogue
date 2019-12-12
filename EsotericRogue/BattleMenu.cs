using System;
using System.Collections.Generic;

namespace EsotericRogue {
    public class BattleMenu : Menu {
        public readonly GameManager GameManager;
        public readonly Character PlayerCharacter;

        private bool enemyTurn = false;
        public BattleMenu(GameManager gameManager, Character playerCharacter) {
            GameManager = gameManager;
            PlayerCharacter = playerCharacter;
            Position = new Vector2(0, 5);
            Options = new List<Option>(2) {
                new Option() {
                    Sprites = new Sprite[] {
                        CreateSprite("Skip")
                    },
                    Action = (menu, op) => {
                        enemyTurn = true;
                    }
                }
            };
        }

        public void Battle(Character character) {
            // Prep
            GameManager.AddUI(this);

            TextUI vsText = new TextUI() {
                Sprites = new Sprite[] {
                    new Sprite("vs", ConsoleColor.White, ConsoleColor.Black)
                },
                Position = new Vector2(18, 0)
            };
            GameManager.AddUI(vsText);

            TextUI descriptionText = new TextUI() {
                Position = new Vector2(40, 0)
            };
            GameManager.AddUI(descriptionText);

            InfoUI enemyInfo = new InfoUI(character) {
                Position = new Vector2(21, 0)
            };
            GameManager.AddUI(enemyInfo);

            Vector2 previousPos = GameManager.PlayerInfo.InfoUI.Position;
            GameManager.PlayerInfo.InfoUI.Position = new Vector2(0, 0);

            void CleanUp() {
                GameManager.RemoveUI(this);
                GameManager.PlayerInfo.InfoUI.Position = previousPos;
                enemyInfo.Destroy();
                GameManager.RemoveUI(enemyInfo);
                GameManager.RemoveUI(vsText);
                GameManager.RemoveUI(descriptionText);
            }

            // Battle
            GameManager.Display();
            while (character.Health > 0) {
                if (PlayerCharacter.Health <= 0) {
                    CleanUp();
                    Renderer.Clear();
                    GameManager.PlayerInfo.InfoUI.Position = new Vector2(0, 0);
                    GameManager.PlayerInfo.InfoUI.Display();
                    enemyInfo.Display();
                    descriptionText.Display();
                    Renderer.Display(
                        CreateSprite("Game Over!" + Environment.NewLine),
                        new Vector2(0, 7)
                    );
                    Renderer.ReadLine();
                    return;
                }

                if (GameManager.PlayerInfo.Input.SelectedUI == null)
                    GameManager.PlayerInfo.Input.SelectedUI = this;
                GameManager.PlayerInfo.Input.UIControls();
                if (enemyTurn) {
                    enemyTurn = false;
                    character.Brain.Controls(PlayerCharacter);
                    descriptionText.Sprites = character.Brain.GetDescription();
                    descriptionText.Display();
                }
            }

            // Drop


            // Clean up
            CleanUp();
        }
    }
}
