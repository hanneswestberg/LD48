using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchNode
{
    public TrunkNode AttachedTrunk { get; set; }
    public Vector3 WorldPosition { get; set; }
    public bool HasBranch { get; set; }
}
