namespace EsotericRogue {
    public class Resource {
        public static int MaxResourceValue { get; private set; }
        public static int MaxResourceStringLength { get; private set; }
        public static void SetMax(int max) {
            MaxResourceValue = max;
            MaxResourceStringLength = max.ToString().Length;
        }

        static Resource() {
            SetMax(9999);
        }

        public int Value { get; private set; }
        public int Max { get; private set; }
        public int Regen { get; set; }

        public Resource() {
            Value = 0;
            Max = 0;
        }

        public Resource(int value) {
            Value = value;
            Max = value;
        }

        private void Check() {
            if (Value < 0)
                Value = 0;
            else if (Value > Max)
                Value = Max;
        }

        public delegate void ValueChangedAction(Resource resource, int oldValue);
        public event ValueChangedAction ValueChanged;

        public delegate void AddedAction(Resource resource, int addedAmount);
        public event AddedAction Added;

        internal void Step() {
            Add(Regen);
        }

        public void Add(int amount) {
            int oldValue = Value;
            Value += amount;
            Check();
            Added?.Invoke(this, amount);
            ValueChanged?.Invoke(this, oldValue);
        }

        public delegate void RemovedAction(Resource resource, int removedAmount);
        public event RemovedAction Removed;

        public void Remove(int amount) {
            int oldValue = Value;
            Value -= amount;
            Check();
            Removed?.Invoke(this, amount);
            ValueChanged?.Invoke(this, oldValue);
        }

        public bool Use(int amount) {
            if (amount > Value) return false;
            Remove(amount);
            return true;
        }

        public void IncreaseMax(int amount) {
            Max += amount;
            if (Max < 0)
                Max = 0;
            else if (Max > MaxResourceValue)
                Max = MaxResourceValue;
            Check();
        }
    }
}
