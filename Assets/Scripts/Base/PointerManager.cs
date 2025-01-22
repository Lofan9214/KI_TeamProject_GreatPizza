using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerManager : MonoBehaviour
{
    public LayerMask layerMask;
    public Transform virtualCam;

    private string selectedId;

    private bool hitPointable;
    private Pizza pizza;
    private Transform target;

    private void Start()
    {
        selectedId = string.Empty;
    }

    private void Update()
    {
#if UNITY_EDITOR

#endif
#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            RaycastHit2D hit;
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touch.position), Vector2.zero, float.PositiveInfinity, layerMask);
                    hitPointable = hit;
                    pizza = null;
                    target = null;
                    if (hit)
                    {
                        IPointable pointable = hit.collider.GetComponent<IPointable>();
                        pointable?.OnPressObject(hit.point);
                        pizza = hit.collider.GetComponent<Pizza>();
                        if (pizza != null)
                        {
                            pizza.DragPizza(hit.point, selectedId, true);
                        }
                        if (hit.collider.CompareTag("Tub"))
                        {
                            selectedId = hit.collider.gameObject.name;
                            Debug.Log(selectedId);
                        }
                    }
                    break;
                case TouchPhase.Moved:
                    Vector3 deltaWorldPos = Camera.main.ScreenToWorldPoint(touch.position) - Camera.main.ScreenToWorldPoint(touch.position - touch.deltaPosition);
                    if (hitPointable)
                    {
                        hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touch.position), Vector2.zero, float.PositiveInfinity, layerMask);
                        if (hit)
                        {
                            pizza?.DragPizza(hit.point, selectedId);
                            if (hit.collider.CompareTag("PizzaBoard"))
                            {
                                hit.collider.transform.parent.transform.position += deltaWorldPos;
                            }
                        }
                        break;
                    }
                    Vector3 cameraPos = Camera.main.transform.position;
                    cameraPos -= deltaWorldPos;
                    virtualCam.position = cameraPos;
                    break;
                case TouchPhase.Stationary:
                    break;
                case TouchPhase.Ended:
                    break;
                case TouchPhase.Canceled:
                    break;
            }
        }
#endif
    }
}
