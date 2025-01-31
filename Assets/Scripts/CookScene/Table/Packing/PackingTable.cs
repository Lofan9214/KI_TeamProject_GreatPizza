using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackingTable : MonoBehaviour
{
    public PizzaBox prefab;
    public Transform boxPos;

    public void SetPizzaBox(int count)
    {
        for (int i = 0; i < count; ++i)
        {
            Instantiate(prefab, boxPos.position + Vector3.up * i, Quaternion.identity, transform);
        }
    }
}
