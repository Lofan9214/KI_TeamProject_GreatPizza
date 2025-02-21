using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class IngameObjectPoolManager : MonoBehaviour
{
    [SerializeField]
    private GameObject pizzaPrefab;
    [SerializeField]
    private GameObject cutPrefab;
    [SerializeField]
    private GameObject toppingPrefab;

    public IObjectPool<GameObject> cutPool { get; private set; }
    public IObjectPool<GameObject> toppingPool { get; private set; }
    public IObjectPool<GameObject> pizzaPool { get; private set; }

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        cutPool = new ObjectPool<GameObject>(CreatePooledCut, OnTakeFromPool, OnReturnedToPool, OnDestroyOnObject, true);
        toppingPool = new ObjectPool<GameObject>(CreatePooledTopping, OnTakeFromPool, OnReturnedToPool, OnDestroyOnObject, true);
        pizzaPool = new ObjectPool<GameObject>(CreatePooledPizza, OnTakeFromPool, OnReturnedToPool, OnDestroyOnObject, true);
    }

    private GameObject CreatePooledCut()
    {
        GameObject cut = Instantiate(cutPrefab);
        cut.GetComponent<IObjectPoolItem>().ObjPool = cutPool;
        return cut;
    }

    private GameObject CreatePooledTopping()
    {
        GameObject topping = Instantiate(toppingPrefab);
        topping.GetComponent<IObjectPoolItem>().ObjPool = toppingPool;
        return topping;
    }

    private GameObject CreatePooledPizza()
    {
        GameObject pizza = Instantiate(pizzaPrefab);
        pizza.GetComponent<IObjectPoolItem>().ObjPool = pizzaPool;
        return pizza;
    }

    private void OnTakeFromPool(GameObject go)
    {
        go.SetActive(true);
    }

    private void OnReturnedToPool(GameObject go)
    {
        go.transform.SetParent(transform);
        go.SetActive(false);
    }

    private void OnDestroyOnObject(GameObject go)
    {
        Destroy(go);
    }
}
