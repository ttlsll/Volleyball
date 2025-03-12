using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSizeManager : MonoBehaviour
{
    public Camera mainCamera;
    public Collider2D[] colliders;
    public float buffer;
    
    // Update is called once per frame
    void Update()
    {
        var bounds = new Bounds();

        foreach (var col in colliders) bounds.Encapsulate(col.bounds);

        bounds.Expand(buffer);

        var vertical = bounds.size.y;

        var horizontal = bounds.size.x * mainCamera.pixelHeight / mainCamera.pixelWidth;
        
        var size = Mathf.Max(horizontal, vertical) * 0.5f;
        var center = bounds.center + new Vector3(0, 0, -10);

        size = Mathf.Clamp(size, 5f, 100);
        center.y = Mathf.Clamp(center.y, 0, 100);

        mainCamera.orthographicSize = size;
        mainCamera.transform.position = center;
    }
}
