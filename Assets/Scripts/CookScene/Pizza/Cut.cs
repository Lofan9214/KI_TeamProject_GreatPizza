using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

public class Cut : MonoBehaviour
{
    public GameObject prefab;

    public void AddCut(quaternion rotation)
    {
        Instantiate(prefab, transform.position, rotation, transform);
    }
}
