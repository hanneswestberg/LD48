using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapHelpers
{
    public static List<MapNode> GetDeeperNeighbours(this MapGrid map, MapNode node)
    {
        var listNeighbours = new List<MapNode>();

        listNeighbours.AddIfNotNull(map.GetNode(node.Position + Vector2.left));
        listNeighbours.AddIfNotNull(map.GetNode(node.Position + Vector2.down + Vector2.left));
        listNeighbours.AddIfNotNull(map.GetNode(node.Position + Vector2.down));
        listNeighbours.AddIfNotNull(map.GetNode(node.Position + Vector2.down + Vector2.right));
        listNeighbours.AddIfNotNull(map.GetNode(node.Position + Vector2.right));

        return listNeighbours;
    }

    public static List<MapNode> GetNeighbours(this MapGrid map, MapNode node)
    {
        var listNeighbours = new List<MapNode>();

        listNeighbours.AddRange(map.GetDeeperNeighbours(node));

        listNeighbours.AddIfNotNull(map.GetNode(node.Position + Vector2.up + Vector2.left));
        listNeighbours.AddIfNotNull(map.GetNode(node.Position + Vector2.up));
        listNeighbours.AddIfNotNull(map.GetNode(node.Position + Vector2.up + Vector2.right));

        return listNeighbours;
    }

    public static MapNode GetNode(this MapGrid map, Vector2 pos)
    {
        return map.GetNode(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
    }

    public static MapNode GetNode(this MapGrid map, int x, int y)
    {
        if (!map.IsInsideBounds(x, y))
            return null;

        return map.Nodes[x, y];
    }

    public static bool IsInsideBounds(this MapGrid map, Vector2 pos)
    {
        return map.IsInsideBounds(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
    }

    public static bool IsInsideBounds(this MapGrid map, int x, int y)
    {
        return !(x < map.Nodes.GetLowerBound(0) 
            || x > map.Nodes.GetUpperBound(0) 
            || y < map.Nodes.GetLowerBound(1) 
            || y > map.Nodes.GetUpperBound(1));
    }
}
