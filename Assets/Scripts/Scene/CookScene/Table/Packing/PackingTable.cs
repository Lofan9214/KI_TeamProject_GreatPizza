using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackingTable : MonoBehaviour
{
    public PizzaBox prefab;
    public Transform boxPos;

    public PizzaBox box { get; private set; }

    public void SetPizzaBox()
    {
        if (box == null)
        {
            box = Instantiate(prefab, boxPos.position, Quaternion.identity, transform);
        }
        else
        {
            box.gameObject.SetActive(true);
            box.ResetState();
            box.transform.parent = transform;
            box.transform.position = boxPos.position;
        }
    }
}
