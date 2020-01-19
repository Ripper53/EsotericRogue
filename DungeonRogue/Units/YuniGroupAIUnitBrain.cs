using System;
using System.Collections.Generic;
using System.Text;
using EsotericRogue;

namespace DungeonRogue.Units {
    public class YuniGroupAIUnitBrain : FriendlyAIUnitBrain, IUnitGroup<YuniGroupAIUnitBrain>, AIUnitBrain.IView, AIUnitBrain.IPathfinder {
        public HashSet<Vector2> View { get; set; }
        public LinkedList<Vector2> Path { get; set; }
        public AIUnitGroup<YuniGroupAIUnitBrain> Group { get; set; }
        public int GroupIndex { get; set; }

        public readonly List<Sprite> TalkSprites;

        public class YuniAIUnitGroup : AIUnitFormationGroup<YuniGroupAIUnitBrain>, IScene, IPosition {
            public Vector2 Target;
            private readonly RandomPositionGetter<YuniAIUnitGroup> randomPositionGetter;
            public Scene Scene => this[0].Scene;
            Vector2 IPosition.Position => Position;

            public YuniAIUnitGroup(Vector2 position) : base(position) {
                randomPositionGetter = new RandomPositionGetter<YuniAIUnitGroup>(this);
            }

            public void UpdateTarget() {
                if (this[0].Path == null || this[0].Position == Target + GetPosition(0))
                    Target = Position;
                Target = randomPositionGetter.GetTarget();
            }
        }

        public readonly YuniAIUnitGroup YuniGroup;
        public YuniGroupAIUnitBrain(YuniAIUnitGroup group) {
            YuniGroup = group;

            FriendlyUnit unit = new FriendlyUnit(new Character(1), this) {
                Sprite = new Sprite(")")
            };

            TalkSprites = new List<Sprite>();
            unit.Menu.Sprites = TalkSprites;
        }

        public override void Controls() {
            MoveToPath(this);
        }

        public override void PreCalculate(GameManager gameManager) {
            CastViewBrain.CastView(this, 0);
            YuniGroup.UpdateTarget();
            IPathfinder.FindAStarPath(this, YuniGroup.Target + YuniGroup.GetPosition(GroupIndex));
        }

    }
}
