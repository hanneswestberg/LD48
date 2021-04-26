using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TreeHelpers
{
    public static int CalculateTreeStrength(this TreeData treeData)
    {
        return treeData.RootNodes.Count * 10;
    }

    public static int CalculateSunGain(this TreeData treeData)
    {
        return treeData.BranchNodes.Where(x => x.HasBranch).Count() * 5 + treeData.TrunkNodes.Count;
    }

    public static int CalculateRainGain(this TreeData treeData)
    {
        int maxY = (int)GameManager.Instance.highestYPos;
        int value = 0;
        int depth = 0;
        foreach (var node in treeData.RootNodes)
        {
            depth = maxY - (int)node.Position.y;
            value += depth + 1;
        }
        return value;
    }

    public static int CalculateRootCost(this TreeData treeData)
    {
        return 10 * treeData.RootNodes.Count;
    }

    public static int CalculateTrunkCost(this TreeData treeData)
    {
        return 100 * (treeData.TrunkNodes.Count - 1);
    }

    public static int CalculateBranchCost(this TreeData treeData)
    {
        return 25 + 25 * treeData.BranchNodes.Where(x => x.HasBranch).Count();
    }
}
