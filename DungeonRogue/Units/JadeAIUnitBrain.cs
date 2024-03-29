﻿using System;
using System.Collections.Generic;
using EsotericRogue;

namespace DungeonRogue.Units {
    /// <summary>
    /// Travelling merchant.
    /// </summary>
    public class JadeFriendlyAIUnitBrain : FriendlyAIUnitBrain, AIUnitBrain.IView, AIUnitBrain.IPathfinder {
        public HashSet<Vector2> View { get; set; }
        public LinkedList<Vector2> Path { get; set; }

        public readonly Sprite TalkSprite;

        public JadeFriendlyAIUnitBrain() {
            TalkSprite = Sprite.CreateUI(string.Empty);

            FriendlyUnit friendlyUnit = new FriendlyUnit(new Character(1), this) {
                Sprite = new Sprite("J")
            };
            const string name = "Jade";
            friendlyUnit.Character.Name = name;
            friendlyUnit.Menu.Sprites = new Sprite[] {
                new Sprite(name + Environment.NewLine),
                TalkSprite,
                Sprite.CreateUI(Environment.NewLine)
            };
            friendlyUnit.Interacted += (source, player) => collidedWithPlayer = true;
        }

        public override void Controls() {
            if (collidedWithPlayer) return;
            MoveToPath(this);
        }

        private bool collidedWithPlayer = false;
        public override void PreCalculate(GameManager gameManager) {
            if (collidedWithPlayer) return;
            IView.CastView(this, 0);
            Vector2 playerPos = gameManager.PlayerInfo.Unit.Position;
            if (View.Contains(playerPos)) {
                IPathfinder.FindAStarPath(this, playerPos);
            } else {
                Path = null;
            }
        }
    }
}
