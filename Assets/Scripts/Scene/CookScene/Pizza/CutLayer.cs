using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class CutLayer : MonoBehaviour
{
    private List<GameObject> lines = new List<GameObject>();

    public void AddCut(GameObject cutline, quaternion rotation)
    {
        cutline.transform.SetParent(transform);
        cutline.transform.position = transform.position;
        cutline.transform.rotation = rotation;
        lines.Add(cutline);
    }

    public void Clear()
    {
        foreach (var line in lines)
        {
            line.GetComponent<IObjectPoolItem>().Release();
        }
        lines.Clear();
    }
}
