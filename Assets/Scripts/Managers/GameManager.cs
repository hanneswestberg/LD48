using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public PlayerData PlayerData { get; set; }
    public MapGrid MapGrid { get; set; }
    public float highestYPos { get; private set; }
    public float WindForce { get; set; }

    [Header("References:")]
    [SerializeField] private Transform treeParent;
    [SerializeField] private GameObject treePrefab;
    [SerializeField] private List<ParticleSystemForceField> windField;

    // Rigidbodys for wind
    private List<Rigidbody2D> rigidbody2Ds;
    private Rigidbody2D treeBase;

    public bool debugMode = true;

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
        MapGrid = new MapGrid();

        MapManager.Instance.GenerateTerrain();
        MapManager.Instance.GenerateObstacles(3);
        MapManager.Instance.GenerateMap(MapGrid);

        highestYPos = MapGrid.ListNodes.OrderByDescending(x => x.Position.y).FirstOrDefault().Position.y;

        PlayerData = new PlayerData(new TreeData(MapGrid.ListNodes.Where(x => x.Position.y == highestYPos).ToList()));
        PlayerData.OnResourcesUpdated += UIManager.Instance.UpdateResourceText;
        PlayerData.TreeData.OnTreeStrengthUpdated += UIManager.Instance.UpdateResourceText;

        // Create the prefab
        var treeGO = Instantiate(treePrefab, treeParent);
        treeGO.GetComponent<TreeGameObject>().Data = PlayerData.TreeData;
        treeBase = treeGO.GetComponent<TreeGameObject>().treeBase;

        StartCoroutine(MainGameLoop());
    }

    private IEnumerator MainGameLoop()
    {
        var windTimer = -1f;
        var windDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-0.2f, 0.2f), 0f);
        var windMagnitude = 1f;

        WindForce = 0f;

        UIManager.Instance.UpdateWind(windDirection, windMagnitude);

        Debug.Log($"Wind direction: {windDirection.ToString()}");
        var gameOn = true;

        while (true)
        {
            // BAD BAD BAD
            rigidbody2Ds = FindObjectsOfType<Rigidbody2D>().ToList();

            if (windTimer < 0f)
            { 
                windDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-0.1f, 0.1f));
                windTimer = Random.Range(5f, 10f);

                UIManager.Instance.UpdateWind(windDirection, windMagnitude);
                Debug.Log($"Wind direction: {windDirection.ToString()}");

                foreach (var field in windField)
                {
                    field.directionX = windDirection.x * 30f * windMagnitude;
                    field.directionY = windDirection.y * 30f * windMagnitude;
                }
            }

            // Apply wind
            foreach (var rigidbody in rigidbody2Ds)
            {
                rigidbody.AddForce(windDirection * windMagnitude);
            }

            windTimer -= Time.deltaTime;
            windMagnitude += Time.deltaTime / 45f;

            if (WindForce > PlayerData.TreeStrength)
            {
                if (gameOn) { 
                    windMagnitude += 100f;

                    foreach (var rigid in rigidbody2Ds)
                    {
                        rigid.gravityScale = 0f;
                    }

                    windTimer = 100f;
                    UIManager.Instance.endText.SetActive(true);
                    UIManager.Instance.finalScore.text = PlayerData.TreeStrength.ToString();
                }

                treeBase.bodyType = RigidbodyType2D.Dynamic;
                windMagnitude += Time.deltaTime;
                gameOn = false;
            }

            if (!gameOn && Input.GetKeyDown(KeyCode.Space))
            {
                var scene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(scene.name);
            }

            if (gameOn)
            {
                PlayerData.ResourceSun += PlayerData.SunGain * Time.deltaTime;
                PlayerData.ResourceRain += PlayerData.RainGain * Time.deltaTime;
                WindForce += Time.deltaTime;
            }

            if (debugMode)
            {
                PlayerData.ResourceSun += 100;
                PlayerData.ResourceRain += 100;
            }

            yield return null;
        }
    }
}
