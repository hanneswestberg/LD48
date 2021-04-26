using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildRootUI : MonoBehaviour
{
    public MapNode From { get; set; }
    public MapNode To { get; set; }

    [SerializeField] private Text costText;

    private Button buildButton;
    private PlayerData playerData;

    private void Start()
    {
        buildButton = GetComponent<Button>();
        playerData = GameManager.Instance.PlayerData;
    }

    // Update is called once per frame
    void Update()
    {
        buildButton.interactable = playerData.ResourceSun > playerData.TreeData.RootCost;
        costText.text = playerData.TreeData.RootCost.ToString();
        costText.color = playerData.ResourceSun > playerData.TreeData.RootCost
            ? Color.green
            : Color.red;
    }

    public void BuildRoot()
    {
        GameManager.Instance.PlayerData.ResourceSun -= playerData.TreeData.RootCost;
        GameManager.Instance.PlayerData.TreeData.AddRoot(From, To);
        UIManager.Instance.Selected = null;
    }
}
