using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

[RequireComponent(typeof(CircleCollider2D))]
public class MapNodeGameObject : MonoBehaviour, ISelectable
{
    [Header("References:")]
    [SerializeField] private Transform lineRendererParent;
    [SerializeField] private GameObject lineRendererPrefab;

    public MapNode Data { get; set; }
    public UnityAction OnSelect { get; set; }
    public UnityAction OnDeselect { get; set; }

    private SpriteRenderer spriteRenderer;
    private Color orgColor;

    private void Start()
    {
        spriteRenderer = transform.GetComponentInChildren<SpriteRenderer>();
        orgColor = spriteRenderer.color;

        OnDeselect += () => {
            spriteRenderer.color = orgColor;
            lineRendererParent.gameObject.SetActive(false);
        };

        foreach (var neighbor in Data.DeeperNeighbours)
        {
            var lineRendererGO = Instantiate(lineRendererPrefab, lineRendererParent);
            var lineRenderer = lineRendererGO.GetComponent<LineRenderer>();
            lineRendererGO.GetComponent<MapNodeLine>().From = Data;
            lineRendererGO.GetComponent<MapNodeLine>().To = neighbor;

            lineRenderer.positionCount = 2;
            lineRenderer.SetPositions(new Vector3[2] {
                Data.WorldPosition,
                neighbor.WorldPosition
            });
        }
        lineRendererParent.gameObject.SetActive(false);
    }

    public void OnMouseDown()
    {
        if (!(UIManager.Instance.Selected is MapNodeGameObject node) || !Data.Neighbours.Contains(node.Data))
        {
            UIManager.Instance.Selected = this;
        }
    }

    public void OnMouseOver()
    {
        if (!(UIManager.Instance.Selected is MapNodeGameObject))
        {
            spriteRenderer.color = GameManager.Instance.PlayerData.TreeData.RootNodes.Contains(Data)
                ? Color.green
                : Color.red;

            lineRendererParent.gameObject.SetActive(true);
        }
    }

    public void OnMouseExit()
    {
        if ((object)UIManager.Instance.Selected != this)
        {
            spriteRenderer.color = orgColor;
            lineRendererParent.gameObject.SetActive(false);
        }
    }
}
