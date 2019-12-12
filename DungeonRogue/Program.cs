﻿using System;
using EsotericRogue;

namespace DungeonRogue {
    class Program {
        static void Main(string[] args) {
            Console.CursorVisible = false;
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);

            PlayGame();
        }

        private static void PlayGame() {
            GameManager gameManager = new DungeonGameManager();

            gameManager.SceneGenerator = new DungeonSceneGenerator(gameManager.Scene) {
                PlayerUnit = gameManager.PlayerInfo.Input.PlayerUnitBrain.Unit
            };
            const int sizeX = 90, sizeY = 30;
            gameManager.Scene.SetSize(new Vector2(sizeX, sizeY));
            gameManager.PlayerInfo.InfoUI.Position = new Vector2(sizeX + 2, 0);

            gameManager.Run();
        }
    }
}
