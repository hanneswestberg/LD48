using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNodeLine : MonoBehaviour
{
    public MapNode From { get; set; }
    public MapNode To { get; set; }

    [SerializeField] private Color buildColor;
    [SerializeField] private Color obstructedColor;

    private LineRenderer lineRenderer;

    private void Update()
    {
        lineRenderer = GetComponent<LineRenderer>();
        var treedata = GameManager.Instance.PlayerData.TreeData;

        if (treedata.RootNodes.Contains(To) || !treedata.RootNodes.Contains(From))
        {
            lineRenderer.startColor = obstructedColor;
            lineRenderer.endColor = obstructedColor;
        }
        else
        {
            lineRenderer.startColor = buildColor;
            lineRenderer.endColor = buildColor;
        }
    }
}
