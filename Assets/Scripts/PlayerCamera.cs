using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private const string Horizontal = "Horizontal";
    private const string Vertical = "Vertical";

    public float sensivity = 0.01f;

    private void Update()
    {
        Vector3 cameraPos;
#if UNITY_EDITOR
        cameraPos = Camera.main.transform.position;
        cameraPos.x += Input.GetAxis(Horizontal) * Time.deltaTime * 10f;
        cameraPos.y += Input.GetAxis(Vertical) * Time.deltaTime * 10f;
#endif
#if UNITY_ANDROID || UNITY_IOS
        cameraPos = Camera.main.transform.position;
        if (Input.touchCount == 1
            && Input.touches[0].phase == TouchPhase.Moved)
        {
            Vector2 deltapos = Input.touches[0].deltaPosition * sensivity;
            cameraPos.x -= deltapos.x;
            cameraPos.y -= deltapos.y;
        }
#endif
        gameObject.transform.position = cameraPos;
    }
}
