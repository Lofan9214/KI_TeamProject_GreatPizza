using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    private static readonly object lockObject = new object();
    private static bool applicationIsQuitting = false;

    public static T Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                Debug.LogWarning($"[Singleton] Instance of {typeof(T)} is null because application is quitting.");
                return null;
            }

            lock (lockObject)
            {
                if (instance == null)
                    instance = FindObjectOfType<T>();

                if (FindObjectsOfType<T>().Length > 1)
                {
                    Debug.LogError($"[Singleton] Multiple instances of {typeof(T)} found!");
                    return instance;
                }

                if (instance == null)
                {
                    GameObject singleton = new GameObject($"(singleton) {typeof(T)}");

                    instance = singleton.AddComponent<T>();

                    DontDestroyOnLoad(singleton);

                    Debug.Log($"[Singleton] An instance of {typeof(T)} was created with DontDestroyOnLoad.");
                }
                else
                {
                    Debug.Log($"[Singleton] Using existing instance: {instance.gameObject.name}");
                }
            }

            return instance;
        }
    }
}
