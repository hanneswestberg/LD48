using System.Collections.Generic;
using UnityEngine.Events;

public class PlayerData
{
    // Resourcess
    private float resourceSun;
    public float ResourceSun {
        get {
            return resourceSun;
        }
        set {
            resourceSun = value;
            OnResourcesUpdated?.Invoke();
        }
    }

    private float resourceRain;
    public float ResourceRain {
        get {
            return resourceRain;
        }
        set {
            resourceRain = value;
            OnResourcesUpdated?.Invoke();
        }
    }

    private int resourceFirmness;
    public int ResourceFirmness
    {
        get
        {
            return resourceFirmness;
        }
        set
        {
            resourceFirmness = value;
            OnResourcesUpdated?.Invoke();
        }
    }

    public int SunGain { get; private set; }
    public int RainGain { get; private set; }
    public int TreeStrength { get; private set; }

    // Tree
    public TreeData TreeData { get; set; }

    // Nodes/Map
    private List<MapNode> exploredNodes;
    public List<MapNode> ExploredNodes {
        get {
            return exploredNodes;
        }
        set {
            exploredNodes = value;
            OnNewNodeExplored?.Invoke();
        }
    }

    public UnityAction OnResourcesUpdated { get; set; }
    public UnityAction OnNewNodeExplored { get; set; }

    public PlayerData(TreeData data) {
        exploredNodes = new List<MapNode>();
        TreeData = data;
        TreeStrength = TreeData.CalculateTreeStrength();
        SunGain = TreeData.CalculateSunGain();
        RainGain = TreeData.CalculateRainGain();

        TreeData.OnTrunkNodesUpdated += () => {
            SunGain = TreeData.CalculateSunGain();
            TreeStrength = TreeData.CalculateTreeStrength();
        };

        TreeData.OnBranchNodesUpdated += (node) => {
            SunGain = TreeData.CalculateSunGain();
            TreeStrength = TreeData.CalculateTreeStrength();
        };

        TreeData.OnRootNodesUpdated += () => {
            TreeStrength = TreeData.CalculateTreeStrength();
            RainGain = TreeData.CalculateRainGain();
        };
    }
}
