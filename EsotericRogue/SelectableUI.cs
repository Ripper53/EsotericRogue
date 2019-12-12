namespace EsotericRogue {
    public abstract class SelectableUI : UI {
        private bool selected;
        public bool Selected {
            get => selected;
            set {
                bool procEvents = selected != value;
                selected = value;
                if (procEvents) {
                    if (selected)
                        OnSelected();
                    else
                        OnDeselected();
                }
            }
        }

        protected abstract void OnSelected();
        protected abstract void OnDeselected();

        public abstract void Up();
        public abstract void Right();
        public abstract void Down();
        public abstract void Left();

        public abstract void Enter();
    }
}
