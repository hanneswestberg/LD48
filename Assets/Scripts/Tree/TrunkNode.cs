using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrunkNode
{
    public int Position { get; set; }
    public Vector2 WorldPosition { get; set; }

    public BranchNode BranchLeft { get; set; }
    public BranchNode BranchRight { get; set; }
}
