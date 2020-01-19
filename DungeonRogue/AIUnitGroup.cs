using System.Collections;
using System.Collections.Generic;
using EsotericRogue;

namespace DungeonRogue {
    public class AIUnitGroup<T> : IReadOnlyList<T> where T : AIUnitBrain, IUnitGroup<T> {
        private readonly List<T> group = new List<T>();

        public T this[int index] => group[index];
        public int Count => group.Count;

        /// <param name="position">Position relative to origin (0, 0) of formation.</param>
        public virtual void Add(T unitBrain) {
            unitBrain.Group = this;
            unitBrain.GroupIndex = Count;
            group.Add(unitBrain);
        }

        public IEnumerator<T> GetEnumerator() {
            return group.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
