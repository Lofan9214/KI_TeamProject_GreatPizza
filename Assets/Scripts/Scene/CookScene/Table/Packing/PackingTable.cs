using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackingTable : MonoBehaviour
{
    public PizzaBox prefab;
    public Transform boxPos;

    public PizzaBox box { get; private set; }

    public void SetPizzaBox(int count)
    {
        for (int i = 0; i < count; ++i)
        {
            box = Instantiate(prefab, boxPos.position, Quaternion.identity, transform);
        }
    }

    public void DestroyPizzaBox()
    {
        if (box != null)
        {
            Destroy(box.gameObject);
            box = null;
        }
    }
}
