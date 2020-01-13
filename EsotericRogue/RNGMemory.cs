using System;
using System.Collections.Generic;

namespace EsotericRogue {
    public class RNGMemory {
        private readonly Random rng = new Random();
        private readonly List<int> generatedInts = new List<int>();
        public int Index = 0;

        public void Clear() {
            Index = 0;
            generatedInts.Clear();
        }

        public int GetInt(int minValue, int maxValue) {
            if (!GetInt(out int randomNum, minValue, maxValue))
                SetInt(out randomNum, rng.Next(minValue, maxValue));
            return randomNum;
        }

        public int GetInt(int maxValue) {
            Index++;
            if (!GetInt(out int randomNum, 0, maxValue))
                SetInt(out randomNum, rng.Next(maxValue));
            return randomNum;
        }

        private void SetInt(out int randomNum, int value) {
            randomNum = value;
            generatedInts.Add(randomNum);
        }

        private bool GetInt(out int randomNum, int minValue, int maxValue) {
            if (Index < generatedInts.Count) {
                randomNum = generatedInts[Index];
                if (randomNum < minValue)
                    randomNum = minValue;
                else if (randomNum > maxValue)
                    randomNum = maxValue;
                return true;
            } else {
                randomNum = 0;
                return false;
            }
        }

    }
}
