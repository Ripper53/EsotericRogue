using System;

namespace DungeonRogue {
    class Program {
        static void Main(string[] args) {
            Console.Title = "Dungeon Rogue";
            Console.CursorVisible = false;
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);

            PlayGame();
        }

        private static void PlayGame() {
            DungeonGameManager gameManager;
            do {
                gameManager = new DungeonGameManager();

                gameManager.Run();
            } while (!gameManager.QuitGame);
        }
    }
}
