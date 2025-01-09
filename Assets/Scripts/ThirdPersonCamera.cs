using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;  // A követendő objektum Transform komponense
    public Vector3 offset = new Vector3(0, 10, -10);  // Eltolás a céltól
    public float rotationOffset = 45f;  // Forgatási offset fokban

    private bool hasInitialized = false;

    public void Initialize()
    {
        hasInitialized = true;
    }

    void Update()
    {
        if (target != null && hasInitialized)
        {
            // A kamera pozíciójának beállítása a célobjektum pozíciójához az eltolás hozzáadásával
            transform.position = target.position + offset;

            // Nézzen a target irányába, majd forgassa el a kamerát lefelé a megadott szöggel
            transform.LookAt(target);
            transform.RotateAround(target.position, transform.right, -rotationOffset);
        }
    }
}
