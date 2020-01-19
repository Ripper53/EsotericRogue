using System;

namespace EsotericRogue {
    public static class RandomUtility {
        private static readonly Random rng = new Random();

        public static int GetInt(int maxValue) => rng.Next(maxValue);
        public static int GetInt(int minValue, int maxValue) => rng.Next(minValue, maxValue);
    }
}
