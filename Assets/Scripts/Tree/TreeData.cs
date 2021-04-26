using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TreeData
{
    public List<TrunkNode> TrunkNodes { get; private set;}
    public List<BranchNode> BranchNodes { get; private set; }
    public List<MapNode> RootNodes { get; private set; }
    public List<List<Vector3>> RootLines { get; private set; }


    public UnityAction OnTrunkNodesUpdated { get; set; }
    public UnityAction<BranchNode> OnBranchNodesUpdated { get; set; }
    public UnityAction OnRootNodesUpdated { get; set; }
    public UnityAction OnTreeStrengthUpdated { get; set; }

    public int RootCost { get; private set; }
    public int BranchCost { get; private set; }
    public int TrunkCost { get; private set; }

    public TreeData(List<MapNode> startRoots)
    {
        TrunkNodes = new List<TrunkNode>();
        BranchNodes = new List<BranchNode>();
        RootNodes = startRoots;
        RootLines = new List<List<Vector3>>();
        startRoots.ForEach(x => RootLines.Add(new List<Vector3>() { new Vector3(0f, Random.Range(0.6f, 0.7f), 0f), x.WorldPosition }));

        // Starting data
        TrunkNodes.Add(new TrunkNode() {
            Position = 0
        });


        var firstSegment = new TrunkNode() {
            Position = 1,
            BranchLeft = new BranchNode() {
                HasBranch = false
            },
            BranchRight = new BranchNode() {
                HasBranch = false
            }
        };
        firstSegment.BranchLeft.AttachedTrunk = firstSegment;
        firstSegment.BranchRight.AttachedTrunk = firstSegment;

        BranchNodes.Add(firstSegment.BranchLeft);
        BranchNodes.Add(firstSegment.BranchRight);
        TrunkNodes.Add(firstSegment);

        RootCost = this.CalculateRootCost();
        BranchCost = this.CalculateBranchCost();
        TrunkCost = this.CalculateTrunkCost();
    }

    public void AddTrunk()
    {
        var newSegment = new TrunkNode() {
            Position = TrunkNodes.Count,
            BranchLeft = new BranchNode() {
                HasBranch = false
            },
            BranchRight = new BranchNode() {
                HasBranch = false
            }
        };
        newSegment.BranchLeft.AttachedTrunk = newSegment;
        newSegment.BranchRight.AttachedTrunk = newSegment;

        TrunkNodes.Add(newSegment);
        BranchNodes.Add(newSegment.BranchLeft);
        BranchNodes.Add(newSegment.BranchRight);
        TrunkCost = this.CalculateTrunkCost();

        OnTrunkNodesUpdated?.Invoke();
    }

    public void AddBranch(BranchNode branchNode)
    {
        branchNode.HasBranch = true;
        OnBranchNodesUpdated?.Invoke(branchNode);
        BranchCost = this.CalculateBranchCost();
    }

    public void AddRoot(MapNode fromNode, MapNode toNode)
    {
        if (toNode.Parent != null || !fromNode.Neighbours.Contains(toNode) || RootNodes.Contains(toNode))
            return;

        RootNodes.Add(toNode);
        RootCost = this.CalculateRootCost();

        if (fromNode.Children.Count > 0)
        {
            RootLines.Add(new List<Vector3>() { fromNode.WorldPosition, toNode.WorldPosition });
        }
        else
        {
            var line = RootLines.FirstOrDefault(x => x.Last().Equals(fromNode.WorldPosition));
            line.Add(toNode.WorldPosition);

            var lineStart = line.FirstOrDefault();

            // Check for a switch
            if (lineStart.magnitude > 1)
            {
                // Get lines
                var connectedLines = RootLines.Where(x => x.Contains(lineStart) && x != line).ToList();
                foreach (var connectedLine in connectedLines)
                {
                    // Get the index of
                    var indexOf = connectedLine.IndexOf(lineStart);
                    var remainingLength = connectedLine.Count - indexOf;
                    var remainingLine = connectedLine.Where(x => connectedLine.IndexOf(x) >= indexOf).ToList();

                    if (indexOf == 0)
                        continue;

                    if (line.Count > remainingLength)
                    {
                        // Do the switch
                        connectedLine.RemoveAll(x => connectedLine.IndexOf(x) >= indexOf);
                        connectedLine.AddRange(line);
                        RootLines.Remove(line);
                        RootLines.Add(remainingLine);
                        break;
                    }
                }
            }
        }

        toNode.Parent = fromNode;
        fromNode.Children.Add(toNode);

        toNode.OnConnectionsUpdated?.Invoke();
        fromNode.OnConnectionsUpdated?.Invoke();
        OnRootNodesUpdated?.Invoke();
    }
}
