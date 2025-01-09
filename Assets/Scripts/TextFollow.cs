using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TextFollow : MonoBehaviour
{

    public Transform target;
    public Vector3 offset;
    private Camera cam;
    private RectTransform rectTransform;
    private bool hasinicialized = false;

    // Start is called before the first frame update
    public void Initialize()
    {
        this.cam = Camera.main;
        rectTransform = GetComponent<RectTransform>();
        hasinicialized = true;
    }

    void Update()
    {
        if (target != null && hasinicialized)
        {
            Vector3 worldPosition = target.position + offset;
            Vector3 viewportPoint = cam.WorldToViewportPoint(worldPosition);

            //Csak akkor l�tsz�djon, ha a kamera el�tt van a karakter
            if (viewportPoint.z > 0)
            {
                Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, worldPosition);
                rectTransform.position = screenPoint;
                rectTransform.gameObject.SetActive(true);
            }
        }
    }
}
