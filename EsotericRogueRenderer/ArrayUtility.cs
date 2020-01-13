using System.Collections.Generic;

namespace EsotericRogue {
    public static class ArrayUtility {

        public static T[] Copy<T>(IReadOnlyCollection<T> toCopy) {
            T[] array = new T[toCopy.Count];
            int i = 0;
            foreach (T t in toCopy) {
                array[i] = t;
                i++;
            }
            return array;
        }
    }
}
