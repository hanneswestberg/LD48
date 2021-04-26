using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildTrunkUI : MonoBehaviour
{
    public TrunkNode Data { get; set; }

    private Button buildButton;
    private PlayerData playerData;

    [SerializeField] private Text costText;

    // Start is called before the first frame update
    void Start()
    {
        buildButton = GetComponent<Button>();
        playerData = GameManager.Instance.PlayerData;
    }

    // Update is called once per frame
    void Update()
    {
        buildButton.interactable = playerData.ResourceRain > playerData.TreeData.TrunkCost;
        costText.text = playerData.TreeData.TrunkCost.ToString();
        costText.color = playerData.ResourceRain > playerData.TreeData.TrunkCost
            ? Color.green
            : Color.red;
    }

    public void BuildTrunk()
    {
        GameManager.Instance.PlayerData.ResourceRain -= playerData.TreeData.TrunkCost;
        GameManager.Instance.PlayerData.TreeData.AddTrunk();
        UIManager.Instance.Selected = null;
    }
}
