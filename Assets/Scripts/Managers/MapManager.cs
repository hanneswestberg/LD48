using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    [Header("Settings:")]
    [Range(0.2f, 2f), SerializeField] private float rootNodeDistance = 0.5f;

    [Header("Prefabs:")]
    [SerializeField] private MapPrefabs mapPrefabs;
    [SerializeField] private GameObject obstaclePrefab;

    [Header("References:")]
    [SerializeField] private Transform mapTransform;
    [SerializeField] private Transform mapRootNodesTransform;
    [SerializeField] private Transform obstacleTransform;
    [SerializeField] private GameObject terrain;
    [SerializeField] private PolygonCollider2D terrainCollider;
    [SerializeField] private PolygonCollider2D mapCollider;
    [SerializeField] private PolygonCollider2D[] obstacleColliders;

    private void Awake()
    {
        // Singleton 
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void GenerateMap(MapGrid mapGrid)
    {
        var nodeGoList = new List<MapNodeGameObject>();

        // First we calculate the bounds of our map
        float xMax = 0, xMin = 0, yMax = 0, yMin = 0;
        foreach (var point in mapCollider.points)
        {
            xMax = (xMax < point.x) ? point.x : xMax;
            xMin = (xMin > point.x) ? point.x : xMin;

            yMax = (yMax < point.y) ? point.y : yMax;
            yMin = (yMin > point.y) ? point.y : yMin;
        }

        // Calculate the needed iterations
        var iterationsX = Mathf.FloorToInt((Mathf.Abs(xMax) + Mathf.Abs(xMin)) / rootNodeDistance);
        var iterationsY = Mathf.FloorToInt((Mathf.Abs(yMax) + Mathf.Abs(yMin)) / rootNodeDistance);
        mapGrid.SetNodeDimensions(iterationsX, iterationsY);

        // Then we iterate through the bounds and create our root nodes
        for (int yCount = 0; yCount < iterationsY; yCount++)
        {
            for (int xCount = 0; xCount < iterationsX - 1; xCount++)
            {
                // Calculate position, justified by mapCollider pos
                var position = new Vector3(xMin + (rootNodeDistance * xCount), yMin + (rootNodeDistance * yCount)) + mapCollider.transform.position;

                // Check placement
                if (!mapCollider.OverlapPoint(position))
                    continue;

                bool collides = false;
                foreach (var o in obstacleColliders)
                {
                    if (o.OverlapPoint(position))
                    {
                        collides = true;
                        break;
                    }
                }

                if (collides)
                    continue;

                // Add a random vector
                var randAdjustedPosition = position + (new Vector3(Random.Range(0, 1f), Random.Range(0, 1f)) * rootNodeDistance / 2f);

                // Check placement again, just to be sure
                // Otherwise revert to original pos
                var spawnPos = mapCollider.OverlapPoint(position)
                    ? randAdjustedPosition
                    : position;

                // Create the data model
                var nodeModel = new MapNode() {
                    Position = new Vector2(xCount, yCount),
                    WorldPosition = spawnPos
                };

                // Spawn the node
                var rootNodeGo = Instantiate(mapPrefabs.rootNodePrefab, spawnPos, Quaternion.identity, mapRootNodesTransform);
                rootNodeGo.GetComponent<MapNodeGameObject>().Data = nodeModel;

                nodeGoList.Add(rootNodeGo.GetComponent<MapNodeGameObject>());

                // Add the node to the grid model
                mapGrid.AddNode(nodeModel);
            }
        }

        // Set neighbours for the node
        foreach (var node in mapGrid.ListNodes)
        {
            node.Neighbours = mapGrid.GetNeighbours(node);
            node.DeeperNeighbours = mapGrid.GetDeeperNeighbours(node);
        }

        mapCollider.enabled = false;
    }

    public void GenerateTerrain()
    {
        var controller = terrain.GetComponent<SpriteShapeController>();
        var spline = controller.spline;

        spline.Clear();

        for (int i = 0; i < terrainCollider.points.Length; i++)
        {
            var p = terrainCollider.points[i];

            spline.InsertPointAt(i, new Vector3(p.x, p.y, 0));
            spline.SetTangentMode(i, ShapeTangentMode.Continuous);
        }

        terrainCollider.enabled = false;
    }

    public void GenerateObstacles(int amount, float minRadius = 1.0f, float maxRadius = 1.5f)
    {
        List<Vector2> obstacles = new List<Vector2>();
        List<PolygonCollider2D> colliders = new List<PolygonCollider2D>();
        int numVertices = 8;

        float xMax = 0, xMin = 0, yMax = 0, yMin = 0;
        foreach (var point in mapCollider.points)
        {
            xMax = (xMax < point.x) ? point.x : xMax;
            xMin = (xMin > point.x) ? point.x : xMin;

            yMax = (yMax < point.y) ? point.y : yMax;
            yMin = (yMin > point.y) ? point.y : yMin;
        }

        yMax = yMin / 2;

        for (int i = 0; i < amount; i++)
        {
            Vector2 centroid = new Vector2(Random.Range(xMin, xMax), Random.Range(yMin, yMax));

            int maxTries = 20;
            while (minDist(obstacles, centroid) < maxRadius * 2 && mapCollider.OverlapPoint(centroid))
            {
                centroid = new Vector2(Random.Range(xMin, xMax), Random.Range(yMin, yMax));
                maxTries--;

                if (maxTries <= 0)
                {
                    Debug.LogWarning("Could not find place for obstacle");
                    return;
                }
            }

            float angle = 360 / numVertices;
            float magnitude = Random.Range(minRadius, maxRadius);

            var inst = Instantiate(obstaclePrefab, obstacleTransform);
            var spline = inst.GetComponent<SpriteShapeController>().spline;
            var collider = inst.GetComponent<PolygonCollider2D>();
            Vector2[] vertices = new Vector2[numVertices];
            spline.Clear();

            // Create polygon bounds by rotating clockwise around centroid
            for (int j = 0; j < numVertices; j++)
            {
                Vector2 pos = Quaternion.AngleAxis(angle * j, -Vector3.forward) * Vector2.left;
                magnitude += Random.Range(-0.1f, 0.1f);

                pos = centroid + (pos * magnitude);
                spline.InsertPointAt(j, pos);
                vertices[j] = pos;
            }

            for (int j = 0; j < numVertices; j++)
                spline.SetTangentMode(j, ShapeTangentMode.Continuous);

            collider.SetPath(0, vertices);
            obstacles.Add(centroid);
            colliders.Add(collider);

        }
        obstacleColliders = colliders.ToArray();
    }

    private float minDist(List<Vector2> points, Vector2 p)
    {
        float dist = 9999;
        foreach (var v in points)
        {
            float newDist = Vector2.Distance(v, p);
            dist = Mathf.Min(dist, newDist);
        }
        return dist;
    }
}
