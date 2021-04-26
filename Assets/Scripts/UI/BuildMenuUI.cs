using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMenuUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera cam;
    [SerializeField] private Transform optionParent;

    public void SetOptions<T>(GameObject[] options, T dataModel)
    {
        ClearOptions();

        Vector3 pos = new Vector3(0, 75, 0);
        foreach (var opt in options)
        {
            var inst = Instantiate(opt, optionParent);

            if (dataModel is TrunkNode trunkNode)
            {
                inst.GetComponent<BuildTrunkUI>().Data = trunkNode;
            }
            else if (dataModel is BranchNode branchNode)
            {
                inst.GetComponent<BuildBranchUI>().Data = branchNode;
            }
            
            inst.transform.position += pos;
            pos = Quaternion.Euler(0, 0, -60) * pos;
        }
    }

    private void ClearOptions()
    {
        foreach (Transform child in optionParent)
        {
            Destroy(child.gameObject);
        }
    }
}
