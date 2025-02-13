using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public Transform background;
    public Transform casher;

    private void Start()
    {
        var scale = background.transform.localScale;
        scale.x = (9f / 19f) * Screen.width / Screen.height;
        background.transform.localScale = scale;
        var pos = casher.position;
        pos.x = Camera.main.ViewportToWorldPoint(Vector3.zero).x;
        casher.position = pos;
    }
}
