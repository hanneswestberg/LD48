using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

public class RootGameObject : MonoBehaviour
{
    public List<Vector3> DataList { get; set; }

    private SpriteShapeController rootShape;
    private Vector3 endPoint;
    private void Start()
    {
        endPoint = Vector3.zero;
    }

    public void AddMapNode(Vector3 node)
    {
        // Bad practice :(
        if (DataList == null) {
            DataList = new List<Vector3>();
            rootShape = GetComponent<SpriteShapeController>();
        }

        DataList.Add(node);
        UpdateRoots();
    }

    public void UpdateRoots()
    { 
        StartCoroutine(LerpBuild());
    }

    public IEnumerator LerpBuild()
    {
        transform.position = DataList.FirstOrDefault();

        while (Vector3.Distance(endPoint, DataList.Last()) > 0.05f)
        {
            rootShape.spline.Clear();


            for (int i = 0; i < DataList.Count; i++)
            {
                var pos = DataList[i] - transform.position;
                var left_tangent = (transform.position - pos).normalized / 5f;
                var right_tangent = (pos - transform.position).normalized / 5f;

                if (DataList.Count > 1 && i == DataList.Count - 1)
                {
                    endPoint = Vector3.Lerp(endPoint, pos, 0.03f);
                    
                    if (Vector3.Distance(endPoint, DataList[i - 1]) > 0.2f)
                    {
                        rootShape.spline.InsertPointAt(i, endPoint);
                        rootShape.spline.SetHeight(i, Mathf.Clamp(0.1f + DataList.Count * 0.05f, 0f, 3f) / ((i / 3f) + 1f));
                        rootShape.spline.SetTangentMode(i, ShapeTangentMode.Continuous);
                        rootShape.spline.SetLeftTangent(i, left_tangent);
                        rootShape.spline.SetRightTangent(i, right_tangent);
                    }
                }
                else
                { 
                    rootShape.spline.InsertPointAt(i, pos);
                    rootShape.spline.SetHeight(i, Mathf.Clamp(0.1f + DataList.Count * 0.05f, 0f, 3f) / ((i / 3f) + 1f));
                    rootShape.spline.SetTangentMode(i, ShapeTangentMode.Continuous);
                    rootShape.spline.SetLeftTangent(i, left_tangent);
                    rootShape.spline.SetRightTangent(i, right_tangent);
                }
            }

            yield return null;
        }
    }
}
