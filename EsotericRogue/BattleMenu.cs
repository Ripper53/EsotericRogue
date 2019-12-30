using System;

namespace EsotericRogue {
    public class BattleMenu : Menu {
        public readonly GameManager GameManager;
        public readonly PlayerInfo PlayerInfo;
        public readonly Character PlayerCharacter;
        private Character enemyCharacter;
        private int oldDistance, oldSpeed;
        private bool inBattle = false;
        private readonly Sprite
            distanceSprite = Sprite.CreateUI(string.Empty),
            speedSprite = Sprite.CreateUI(string.Empty);

        private bool playerAlive = true, enemyTurn = false;
        public BattleMenu(GameManager gameManager, PlayerInfo playerInfo) {
            Sprites = new Sprite[] {
                Sprite.CreateUI("Distance: "),
                distanceSprite,
                Sprite.CreateUI(Environment.NewLine + "Speed: "),
                speedSprite,
                Sprite.CreateUI(Environment.NewLine)
            };

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
            AddOption(
                new Option() {
                    Sprites = new Sprite[] {
                        Sprite.CreateUI("Move")
                    },
                    Action = (menu, op) => {
                        Vector2 pos = new Vector2(6, 11);
                        string moveDis = Renderer.ReadLine(pos);
                        Renderer.Display(Sprite.CreateEmptyUI(moveDis.Length), pos);
                        if (int.TryParse(moveDis, out int move)) {
                            oldDistance = distanceSprite.Display.Length;
                            oldSpeed = speedSprite.Display.Length;
                            PlayerCharacter.Boot.Equipped.Move(PlayerCharacter.Distance > enemyCharacter.Distance ? -move : move);
                            UpdateDistanceSprite();
                        }
                    }
                }
            );

            PlayerCharacter.Weapon.ItemEquipped += PlayerCharacter_WeaponEquipped;
            PlayerCharacter_WeaponEquipped(PlayerCharacter, PlayerCharacter.Weapon.Equipped, null);
        }

        private int GetDistance() {
            return Math.Abs(PlayerCharacter.Distance - enemyCharacter.Distance);
        }

        private void PlayerCharacter_WeaponEquipped(Character character, Weapon weapon, Weapon oldWeapon) {
            ClearWeaponOptions();
            for (int i = 0; i < weapon.Count; i++) {
                Weapon.Action action = weapon[i];
                int index = i;
                AddOption(new Option() {
                    Sprites = action.GetDescription(),
                    Action = (menu, op) => {
                        if (weapon.Use(index, enemyCharacter)) {
                            PlayerInfo.CharacterBrain.Description = weapon[index].GetDescription();
                            enemyTurn = true;
                        }
                    }
                });
            }
            if (inBattle) {
                Clear();
                Display();
            }
        }

        private void ClearWeaponOptions() {
            const int index = 2;
            if (Options.Count > index)
                RemoveRangeOptions(index, Options.Count - index);
        }

        private void UpdateDistanceSprite() {
            Vector2 pos = new Vector2(Position.x + 10, Position.y);
            Renderer.Display(Sprite.CreateEmptyUI(oldDistance), pos);
            distanceSprite.Display = GetDistance().ToString();
            Renderer.Display(distanceSprite, pos);
        }

        private void UpdateSpeedSprite() {
            Vector2 pos = new Vector2(Position.x + 10, Position.y + 1);
            Renderer.Display(Sprite.CreateEmptyUI(oldSpeed), pos);
            speedSprite.Display = PlayerCharacter.RemainingDistance.ToString();
            Renderer.Display(speedSprite, pos);
        }

        /// <summary>
        /// Returns true if battle was won, otherwise false.
        /// </summary>
        public bool Battle(Character character) {
            enemyTurn = false;
            enemyCharacter = character;
            // Prep
            GameManager.AddUI(this);

            TextUI vsText = new TextUI() {
                Sprites = new Sprite[] {
                    Sprite.CreateUI(" vs ")
                },
                Position = new Vector2(PlayerInfo.InfoUI.MaxWidth + 1, 0)
            };
            GameManager.AddUI(vsText);

            int playerDescriptionPosX = PlayerInfo.InfoUI.MaxWidth + 55, enemyDescriptionPosX = playerDescriptionPosX - 30;
            const int descriptionWidth = 20, descriptionMaxLength = 100;
            TextUI playerNameDescriptionText = new TextUI() {
                Sprites = new Sprite[] {
                    new Sprite(GetContinuedString(PlayerCharacter.Name, 12) + " Actions:")
                },
                Position = new Vector2(playerDescriptionPosX, 0),
                Width = descriptionWidth,
                MaxLength = descriptionMaxLength
            };
            GameManager.AddUI(playerNameDescriptionText);

            TextUI enemyNameDescriptionText = new TextUI() {
                Sprites = new Sprite[] {
                    new Sprite(GetContinuedString(character.Name, 12) + " Actions:")
                },
                Position = new Vector2(enemyDescriptionPosX, 0),
                Width = descriptionWidth,
                MaxLength = descriptionMaxLength
            };
            GameManager.AddUI(enemyNameDescriptionText);

            TextUI playerDescriptionText = new TextUI() {
                Position = new Vector2(playerDescriptionPosX, 1),
                Width = descriptionWidth,
                MaxLength = descriptionMaxLength
            };
            GameManager.AddUI(playerDescriptionText);

            TextUI enemyDescriptionText = new TextUI() {
                Position = new Vector2(enemyDescriptionPosX, 1),
                Width = descriptionWidth,
                MaxLength = descriptionMaxLength
            };
            GameManager.AddUI(enemyDescriptionText);


            InfoUI enemyInfo = new InfoUI(character) {
                Position = new Vector2(PlayerInfo.InfoUI.MaxWidth + 4, 0)
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
            const int startDistance = 1;
            PlayerCharacter.Distance = startDistance;
            character.Distance = -startDistance;
            UpdateDistanceSprite();
            inBattle = true;
            PlayerCharacter.RemainingDistance = PlayerCharacter.Boot.Equipped.Speed;
            PlayerInfo.Input.UIOnly = true;
            while (enemyAlive) {
                if (!playerAlive) {
                    PlayerInfo.Input.DeselectUI();
                    GameManager.RemoveUI(this);
                    Renderer.Clear();
                    GameManager.DisplayUI();
                    Renderer.Display(
                        Sprite.CreateUI($"Game Over!{Environment.NewLine}Enter 'E' to continue...{Environment.NewLine}"),
                        new Vector2(0, 8)
                    );
                    string exitCode;
                    do {
                        exitCode = Renderer.ReadLine().ToUpper();
                    } while (exitCode != "E" && exitCode != "'E'");
                    return false;
                }
                UpdateSpeedSprite();
                PlayerCharacter.Brain.Controls(character);
                if (enemyAlive && enemyTurn) {
                    enemyTurn = false;
                    character.RemainingDistance = character.Boot.Equipped.Speed;
                    character.Step();
                    oldDistance = GetDistance();
                    character.Brain.Controls(PlayerCharacter);
                    PlayerCharacter.RemainingDistance = PlayerCharacter.Boot.Equipped.Speed;
                    PlayerCharacter.Step();
                    // Display
                    UpdateDistanceSprite();
                    enemyDescriptionText.Clear();
                    playerDescriptionText.Clear();
                    enemyDescriptionText.Sprites = character.Brain.GetDescription();
                    playerDescriptionText.Sprites = PlayerCharacter.Brain.GetDescription();
                    enemyDescriptionText.Display();
                    playerDescriptionText.Display();
                }
            }
            PlayerInfo.Input.UIOnly = false;
            inBattle = false;

            // Drop
            PlayerCharacter.Inventory.Gold += character.Inventory.Gold;
            foreach (Item item in character.Inventory) {
                PlayerCharacter.Inventory.AddItem(item);
            }

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
            GameManager.PlayerInfo.Input.DeselectUI();
            Renderer.Clear();
            GameManager.Display();
            return true;
        }
    }
}
