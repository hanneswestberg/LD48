using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGrid
{
    public MapNode[,] Nodes { get; private set; }
    public List<MapNode> ListNodes { get; private set; }

    public void SetNodeDimensions(int x, int y)
    {
        Nodes = new MapNode[x, y];
        ListNodes = new List<MapNode>();
    }

    public void AddNode(MapNode node)
    {
        var posX = Mathf.RoundToInt(node.Position.x);
        var posY = Mathf.RoundToInt(node.Position.y);

        // Don't add if the node is outside the map bounds
        if (node == null || !this.IsInsideBounds(posX, posY))
            return;

        Nodes[posX, posY] = node;
        ListNodes.Add(node);
    }
}
