namespace EsotericRogue {
    public abstract class SelectableUI : UI {
        private bool selected = false;
        public bool Selected {
            get => selected;
            set {
                bool procEvents = selected != value;
                selected = value;
                if (procEvents) {
                    if (selected)
                        OnSelected?.Invoke(this);
                    else
                        OnDeselected?.Invoke(this);
                }
            }
        }
        public delegate void SelectedAction(SelectableUI source);
        public event SelectedAction OnSelected;
        public delegate void DeselectedAction(SelectableUI source);
        public event DeselectedAction OnDeselected;

        public abstract void Up();
        public abstract void Right();
        public abstract void Down();
        public abstract void Left();

        public abstract void Enter();
    }
}
