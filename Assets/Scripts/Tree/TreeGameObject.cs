using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

public class TreeGameObject : MonoBehaviour
{
    public TreeData Data { get; set; }

    [Header("References:")]
    [SerializeField] private SpriteShapeController trunkShape;
    [SerializeField] private Transform segmentParent;
    [SerializeField] private Transform nodeParent;
    [SerializeField] private Transform rootParent;

    [SerializeField] private GameObject segmentPrefab;
    [SerializeField] private GameObject branchNodePrefab;
    [SerializeField] private GameObject trunkNodePrefab;
    [SerializeField] private GameObject branchPrefab;
    [SerializeField] private GameObject rootPrefab;

    public Rigidbody2D treeBase;

    private List<Transform> segmentJoints = new List<Transform>();
    private Vector3 startPos;
    private TreeData treeData;
    private List<RootGameObject> rootGameObjects = new List<RootGameObject>();

    private Dictionary<BranchNode, GameObject> branches = new Dictionary<BranchNode, GameObject>();

    private Vector3 endPoint;

    private void Start()
    {
        foreach (Transform child in segmentParent)
        { 
            segmentJoints.Add(child);
        }

        treeData = GameManager.Instance.PlayerData.TreeData;

        // Create first trunk node
        UpdateTrunks();
        UpdateTreeNodes();
        UpdateRoots();

        // Event registrations
        treeData.OnTrunkNodesUpdated += () => {
            UpdateTrunks();
            UpdateTreeNodes();
        };

        treeData.OnBranchNodesUpdated += (node) => {
            UpdateBranch(node);
            UpdateTreeNodes();
        };

        treeData.OnRootNodesUpdated += () => {
            UpdateRoots();
        };

        startPos = transform.position;
    }

    void Update()
    {
        UpdateJointsWorldPositions();
        UpdateTrunkSpline();
    }

    void UpdateTrunks()
    {
        for (int i = segmentJoints.Count; i < Data.TrunkNodes.Count; i++)
        {
            var newTrunkGO = Instantiate(segmentPrefab, segmentParent);
            newTrunkGO.GetComponent<SpringJoint2D>().connectedBody = segmentJoints[i - 1].GetComponent<Rigidbody2D>();
            var dir = (segmentJoints[i - 1].position - segmentJoints[i - 2].position).normalized;
            newTrunkGO.transform.position = segmentJoints[i - 1].position + dir * 2f;
            segmentJoints.Add(newTrunkGO.transform);
        }

        foreach (var trunk in segmentJoints)
        {
            var index = segmentJoints.IndexOf(trunk) + 1;
            var rigidBody = trunk.GetComponent<Rigidbody2D>();
            rigidBody.mass = 100f / index;
            rigidBody.gravityScale = -0.5f / index;
        }

        foreach (var branch in branches)
        {
            var index = branch.Key.AttachedTrunk.Position;
            var targetScale = branch.Value.transform.localScale + (Vector3.one / (10f + index * 2f));

            StartCoroutine(UpdateBranch(branch.Value.transform, branch.Value.transform.localScale, targetScale));
        }
    }

    void UpdateBranch(BranchNode node)
    {
        var parent = segmentJoints[node.AttachedTrunk.Position];
        var branchGO = Instantiate(branchPrefab, parent);
        branchGO.transform.rotation = Quaternion.Euler(0f, 0f, node.AttachedTrunk.BranchLeft == node ? Random.Range(160f, 200f) : Random.Range(-20f, 20f));
        StartCoroutine(UpdateBranch(branchGO.transform, Vector3.zero, Vector3.one * Random.Range(0.8f, 1.2f)));
        branches.Add(node, branchGO);
    }

    void UpdateRoots() {

        foreach (var rootLine in treeData.RootLines)
        {
            // Create the root
            if (!rootGameObjects.Any(x => x.DataList.FirstOrDefault() == rootLine.FirstOrDefault()))
            {
                var rootsGO = Instantiate(rootPrefab, rootParent).GetComponent<RootGameObject>();
                rootLine.ForEach(x => rootsGO.AddMapNode(x));
                rootGameObjects.Add(rootsGO.GetComponent<RootGameObject>());
            }

            var roots = rootGameObjects.FirstOrDefault(x => x.DataList.FirstOrDefault() == rootLine.FirstOrDefault());

            foreach (var rootPos in rootLine)
            {
                if(!roots.DataList.Contains(rootPos))
                    roots.AddMapNode(rootPos);
            }
        }

        foreach (var rootGo in rootGameObjects)
        {
            rootGo.DataList = treeData.RootLines[rootGameObjects.IndexOf(rootGo)];
            rootGo.UpdateRoots();
        }
    }

    void UpdateTreeNodes()
    {
        foreach (Transform child in nodeParent) { Destroy(child.gameObject); }

        // Add a trunk node to the last trunk
        var trunkNodeGO = Instantiate(trunkNodePrefab, nodeParent);
        trunkNodeGO.GetComponent<TrunkNodeGameObject>().Data = Data.TrunkNodes.Last();

        // Add branch nodes
        foreach (var trunk in Data.TrunkNodes.Where(x => x.Position != 0))
        {
            if (!trunk.BranchLeft.HasBranch) {
                var branchNodeGO = Instantiate(branchNodePrefab, nodeParent);
                branchNodeGO.transform.position = trunk.WorldPosition + Vector2.left / 3f;
                branchNodeGO.GetComponent<BranchNodeGameObject>().Data = trunk.BranchLeft;
            }

            if (!trunk.BranchRight.HasBranch)
            {
                var branchNodeGO = Instantiate(branchNodePrefab, nodeParent);
                branchNodeGO.transform.position = trunk.WorldPosition + Vector2.right / 3f;
                branchNodeGO.GetComponent<BranchNodeGameObject>().Data = trunk.BranchRight;
            }
        }
    }

    void UpdateJointsWorldPositions()
    {
        foreach (var trunk in Data.TrunkNodes.Where(x => x.Position != 0))
        {
            var joint = segmentJoints[Data.TrunkNodes.IndexOf(trunk)];
            trunk.WorldPosition = joint.position;

            if (!trunk.BranchLeft.HasBranch)
            {
                trunk.BranchLeft.WorldPosition = trunk.WorldPosition + Vector2.left / 3f;
            }

            if (!trunk.BranchRight.HasBranch)
            {
                trunk.BranchRight.WorldPosition = trunk.WorldPosition + Vector2.right / 3f;
            }
        }
    }

    void UpdateTrunkSpline()
    {
        var tangent_distance = 1f;
        trunkShape.spline.Clear();

        for (int i = 0; i < segmentJoints.Count; i++)
        {
            var pos = segmentJoints[i].position - startPos;
            var left_tangent = (startPos - pos).normalized * tangent_distance;
            var right_tangent = (pos - startPos).normalized * tangent_distance;

            if (i == 0)
            {
                right_tangent = Vector3.up.normalized * tangent_distance;
            }

            if (i != 0 && Vector3.Distance(pos, segmentJoints[i - 1].position) < 0.2f)
                continue;

            if (i == segmentJoints.Count - 1)
            {
                endPoint = Vector3.Lerp(endPoint,  pos, 0.03f);

                if (Vector3.Distance(endPoint, segmentJoints[i - 1].position) > 0.2f)
                {
                    trunkShape.spline.InsertPointAt(i, endPoint);
                    trunkShape.spline.SetHeight(i, Mathf.Clamp(0.3f + segmentJoints.Count * 0.1f, 0f, 3f) / ((i / 3f) + 1f));
                    trunkShape.spline.SetTangentMode(i, ShapeTangentMode.Continuous);
                    trunkShape.spline.SetLeftTangent(i, left_tangent);
                    trunkShape.spline.SetRightTangent(i, right_tangent);
                }
            }
            else
            {
                trunkShape.spline.InsertPointAt(i, pos);
                trunkShape.spline.SetHeight(i, Mathf.Clamp(0.3f + segmentJoints.Count * 0.1f, 0f, 3f) / ((i / 3f) + 1f));
                trunkShape.spline.SetTangentMode(i, ShapeTangentMode.Continuous);
                trunkShape.spline.SetLeftTangent(i, left_tangent);
                trunkShape.spline.SetRightTangent(i, right_tangent);
            }
        }
    }

    IEnumerator UpdateBranch(Transform branch, Vector3 startScale, Vector3 targetScale)
    {
        branch.localScale = startScale;

        while (Vector3.Distance(branch.localScale, targetScale) > 0.03f)
        {
            branch.localScale = Vector3.Lerp(branch.localScale, targetScale, 0.02f);

            if (Vector3.Distance(branch.localScale, targetScale) < 0.03f)
            {
                branch.localScale = targetScale;
            }

            yield return null;
        }
    }
}
