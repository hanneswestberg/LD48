using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private float maxY;
    [SerializeField] private float minY;

    [SerializeField] private float scrollSpeed;
    [SerializeField] private float lerpSpeed;

    private Vector3 targetPos;

    private void Start()
    {
        targetPos = transform.position;
    }

    void Update()
    {
        if (Input.mouseScrollDelta.y < 0f)
        {
            targetPos = new Vector3(targetPos.x, Mathf.Clamp(targetPos.y - scrollSpeed, minY, maxY), targetPos.z);
            UIManager.Instance.Selected = null;
        }
        else if (Input.mouseScrollDelta.y > 0f)
        { 
            targetPos = new Vector3(targetPos.x, Mathf.Clamp(targetPos.y + scrollSpeed, minY, maxY), targetPos.z);
            UIManager.Instance.Selected = null;
        }

        // Do the lerping action
        transform.position = Vector3.Lerp(transform.position, targetPos, 0.05f);
    }
}
