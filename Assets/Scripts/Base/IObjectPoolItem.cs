using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public interface IObjectPoolItem
{
    public IObjectPool<GameObject> ObjPool { get; set; }

    public void Release();
}
