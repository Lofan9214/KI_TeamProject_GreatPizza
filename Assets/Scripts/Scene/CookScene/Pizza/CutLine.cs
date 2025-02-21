using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class CutLine : MonoBehaviour, IObjectPoolItem
{
    public IObjectPool<GameObject> ObjPool { get; set; }

    public void Release()
    {
        ObjPool.Release(gameObject);
    }
}
