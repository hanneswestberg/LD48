using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] Camera cam;
    [SerializeField] GameObject windIndicator;

    [Header("Text")]
    [SerializeField] Text sunText;
    [SerializeField] Text waterText;
    [SerializeField] Text firmnessText;
    [SerializeField] Text windForce;

    [Header("Containers")]
    [SerializeField] Transform buildButtonParent;

    [Header("Menu")]
    [SerializeField] BuildMenuUI buildMenu;

    [Header("Prefabs")]
    [SerializeField] GameObject buildButton;
    [SerializeField] GameObject[] trunkBuildOptions;
    [SerializeField] GameObject[] branchBuildOptions;

    public GameObject endText;

    public UnityAction OnSelectedChange { get; set; }

    private ISelectable selected;
    public ISelectable Selected {
        get { return selected; }
        set {
            if (selected != null && selected != value)
            {
                selected.OnDeselect?.Invoke();
                selected = value;
                value?.OnSelect?.Invoke();
                OnSelectedChange?.Invoke();
            }
            else if (selected == null)
            {
                selected = value;
                value?.OnSelect?.Invoke();
                OnSelectedChange?.Invoke();
            }
        }
    }

    private void Awake()
    {
        // Singleton 
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        buildMenu.gameObject.SetActive(false);

        OnSelectedChange += UpdateBuildMenu;
    }

    private void UpdateBuildMenu()
    {
        ClearChildren(buildButtonParent);

        // Show menu
        if (selected != null)
        {
            bool active = false;
            Vector3 pos = Vector3.zero;
            
            if (selected is TrunkNodeGameObject trunkNode)
            {
                active = true;
                pos = trunkNode.transform.position;
                buildMenu.SetOptions(trunkBuildOptions, trunkNode.Data);
            }

            else if (selected is BranchNodeGameObject branchNode)
            {
                active = true;
                pos = branchNode.transform.position;
                buildMenu.SetOptions(branchBuildOptions, branchNode.Data);
            }

            else if (selected is MapNodeGameObject mapNode)
            {
                var data = mapNode.Data;
                var neighbours = data.DeeperNeighbours;

                if (GameManager.Instance.PlayerData.TreeData.RootNodes.Contains(mapNode.Data))
                {
                    foreach (var node in neighbours.Where(x => !GameManager.Instance.PlayerData.TreeData.RootNodes.Contains(x)))
                    {
                        Vector3 v = cam.WorldToScreenPoint(node.WorldPosition);
                        var buildButtonGo = Instantiate(buildButton, v, Quaternion.identity, buildButtonParent);
                        buildButtonGo.GetComponent<BuildRootUI>().From = mapNode.Data;
                        buildButtonGo.GetComponent<BuildRootUI>().To = node;
                    }
                }
            }

            buildMenu.gameObject.SetActive(active);
            buildMenu.transform.position = cam.WorldToScreenPoint(pos);
        }
        else
        {
            buildMenu.gameObject.SetActive(false);
        }
    }

    public void UpdateWind(Vector3 direction, float magnitude)
    {
        float angle = 0.0f;
        if (direction.x < 0)
        {
            angle = Vector3.Angle(Vector3.left, direction);
            windIndicator.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            angle = Vector3.Angle(Vector3.right, direction);
            windIndicator.transform.localScale = new Vector3(1, 1, 1);
        }
        windIndicator.transform.rotation = Quaternion.Euler(0, 0, angle);
        //windIndicator.transform.localScale = new Vector3(magnitude, magnitude, 1);
    }

    public void UpdateResourceText()
    {
        var data = GameManager.Instance.PlayerData;
        sunText.text = "" + Mathf.RoundToInt(data.ResourceSun);
        waterText.text = "" + Mathf.RoundToInt(data.ResourceRain);
        firmnessText.text = "" + data.TreeStrength;
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider == null)
                Selected = null;
        }

        windForce.text = Mathf.RoundToInt(GameManager.Instance.WindForce).ToString();
        windForce.color = GameManager.Instance.WindForce > (float)GameManager.Instance.PlayerData.TreeStrength * 0.5f
            ? GameManager.Instance.WindForce > (float)GameManager.Instance.PlayerData.TreeStrength * 0.75f
                ? Color.red
                : Color.yellow
            : Color.green;

    }

    private void ClearChildren(Transform t)
    {
        foreach (Transform child in t)
        {
            Destroy(child.gameObject);
        }
    }
}
