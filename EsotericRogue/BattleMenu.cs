using System;

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
            Position = new Vector2(0, 8);
            AddOption(
                new Option() {
                    Sprites = new Sprite[] {
                        Sprite.CreateUI("Skip")
                    },
                    Action = (menu, op) => {
                        enemyTurn = true;
                        PlayerInfo.CharacterBrain.Description = new Sprite[] {
                            Sprite.CreateUI("Skip.")
                        };
                    }
                }
            );
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

            const int playerDescriptionPosX = 70, enemyDescriptionPosX = 40;
            TextUI playerNameDescriptionText = new TextUI() {
                Sprites = new Sprite[] {
                    new Sprite(GetContinuedString(PlayerCharacter.Name, 12) + " Actions:")
                },
                Position = new Vector2(playerDescriptionPosX, 0),
                Width = 20
            };
            GameManager.AddUI(playerNameDescriptionText);

            TextUI enemyNameDescriptionText = new TextUI() {
                Sprites = new Sprite[] {
                    new Sprite(GetContinuedString(character.Name, 12) + " Actions:")
                },
                Position = new Vector2(enemyDescriptionPosX, 0),
                Width = 20
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
            GameManager.PlayerInfo.InventoryMenu.Position = new Vector2(playerDescriptionPosX + (playerDescriptionPosX - enemyDescriptionPosX), 0);

            // Battle
            GameManager.PlayerInfo.Input.SelectedUIIndex = GameManager.GetSelectableUIIndex(this);
            GameManager.PlayerInfo.InfoUI.Clear();
            GameManager.PlayerInfo.InventoryMenu.Clear();
            GameManager.DisplayUI();
            bool enemyAlive = true;
            character.Died += source => enemyAlive = false;
            while (enemyAlive) {
                if (!playerAlive) {
                    PlayerInfo.Input.DeselectUI();
                    GameManager.RemoveUI(this);
                    Renderer.Clear();
                    GameManager.DisplayUI();
                    Renderer.Display(
                        Sprite.CreateUI($"Game Over!{Environment.NewLine}Enter 'E' to continue...{Environment.NewLine}"),
                        new Vector2(0, 7)
                    );
                    string exitCode;
                    do {
                        exitCode = Renderer.ReadLine().ToUpper();
                    } while (exitCode != "E" && exitCode != "'E'");
                    return false;
                }
                if (PlayerInfo.Input.SelectedUI == null)
                    PlayerInfo.Input.SelectedUIIndex = 0;
                PlayerCharacter.Brain.Controls(character);
                if (enemyAlive && enemyTurn) {
                    PlayerCharacter.Step();
                    enemyTurn = false;
                    character.Brain.Controls(PlayerCharacter);
                    character.Step();
                    // Display
                    enemyDescriptionText.Clear();
                    playerDescriptionText.Clear();
                    enemyDescriptionText.Sprites = character.Brain.GetDescription();
                    playerDescriptionText.Sprites = PlayerCharacter.Brain.GetDescription();
                    enemyDescriptionText.Display();
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
