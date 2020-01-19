using EsotericRogue;

namespace DungeonRogue {
    public interface IUnitGroup<T> where T : AIUnitBrain, IUnitGroup<T> {
        public AIUnitGroup<T> Group { get; set; }
        public int GroupIndex { get; set; }
    }
}
