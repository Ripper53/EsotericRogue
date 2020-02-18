using System;
using System.Collections.Generic;

namespace EsotericRogue {
    public class AStarPathfinder : Pathfinder {

        private class Node {
            public Vector2 Position;
            /// <summary>
            /// The distance from previous node to this node.
            /// </summary>
            public int G = 0;
            /// <summary>
            /// The actual distance to the end (estimate).
            /// </summary>
            public int H = 0;
            /// <summary>
            /// F = G + H
            /// </summary>
            public int F => G + H;
            public Node Parent = null;

            public Node(Vector2 position) => Position = position;
        }

        private static int Heuristic(Vector2 start, Vector2 end) {
            Vector2 dis = end - start;
            return Math.Abs(dis.x) + Math.Abs(dis.y);
        }

        public override void FindPath(LinkedList<Vector2> path, Scene scene, Vector2 start, Vector2 end) {
            List<Node>
                open = new List<Node>() {
                    new Node(start)
                },
                closed = new List<Node>();

            while (open.Count > 0) {
                // Evaluate open set.
                int winnerIndex = 0;
                for (int i = 1, count = open.Count; i < count; i++) {
                    if (open[i].F < open[winnerIndex].F)
                        winnerIndex = i;
                }

                Node winner = open[winnerIndex];

                if (winner.Position == end) {
                    // DONE!
                    while (winner != null) {
                        path.AddFirst(winner.Position);
                        winner = winner.Parent;
                    }
                    return;
                }

                closed.Add(winner);
                open.RemoveAt(winnerIndex);

                void AddNeighbor(Vector2 position) {
                    position += winner.Position;
                    if (scene.InBounds(position) && scene.GetTile(position) == Scene.Tile.Ground && closed.Find(v => v.Position == position) == null) {
                        Node node = open.Find(v => v.Position == position);
                        int g = winner.G + 1;
                        if (node != null) {
                            // If node at position already exists, update values.
                            if (g < node.G) {
                                node.G = g;
                                node.Parent = winner;
                            }
                        } else {
                            // If node does not exist at position, create node with values.
                            node = new Node(position) {
                                G = g,
                                H = Heuristic(position, end)
                            };
                            open.Add(node);
                            node.Parent = winner;
                        }
                    }
                }

                // Add neighbors to open set.
                AddNeighbor(new Vector2(0, -1));
                AddNeighbor(new Vector2(1, 0));
                AddNeighbor(new Vector2(0, 1));
                AddNeighbor(new Vector2(-1, 0));
            }

        }

    }
}
