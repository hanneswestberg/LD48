using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildBranchUI : MonoBehaviour
{
    public BranchNode Data { get; set; }

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
        buildButton.interactable = playerData.ResourceRain > playerData.TreeData.BranchCost;
        costText.text = playerData.TreeData.BranchCost.ToString();
        costText.color = playerData.ResourceRain > playerData.TreeData.BranchCost
            ? Color.green
            : Color.red;
    }

    public void BuildBranch()
    {
        GameManager.Instance.PlayerData.ResourceRain -= playerData.TreeData.BranchCost;
        GameManager.Instance.PlayerData.TreeData.AddBranch(Data);
        UIManager.Instance.Selected = null;
    }
}
