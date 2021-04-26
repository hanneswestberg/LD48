using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MapNode
{
    public Vector2 Position { get; set; }
    public Vector3 WorldPosition { get; set; }

    public List<MapNode> Neighbours { get; set; }
    public List<MapNode> DeeperNeighbours { get; set; }

    public MapNode Parent { get; set; }
    public List<MapNode> Children { get; set; }

    public UnityAction OnConnectionsUpdated { get; set; }

    public MapNode()
    {
        Neighbours = new List<MapNode>();
        DeeperNeighbours = new List<MapNode>();
        Children = new List<MapNode>();
    }
}
