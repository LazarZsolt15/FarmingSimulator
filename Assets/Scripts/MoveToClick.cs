using UnityEngine;

public class MoveToClick : MonoBehaviour
{
    public float speed = 5f;  // Mozgás sebessége
    private Vector3 targetPosition;  // Cél pozíció
    private bool isMoving = false;   // Mozgás állapota

    void FixedUpdate()
    {
        // Ellenőrzi, hogy a bal egérgombot lenyomták-e
        if (Input.GetMouseButtonDown(0))
        {
            // Létrehoz egy sugár (ray) az egér pozíciójából a 3D térbe
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Ellenőrzi, hogy a sugár talált-e felületet
            if (Physics.Raycast(ray, out hit))
            {
                // Beállítja a cél pozíciót a találat helyére, de az Y koordináta az objektum aktuális Y pozícióján marad
                targetPosition = hit.point;
                targetPosition.y = transform.position.y;
                isMoving = true;
            }
        }

        // Ha mozogni kell és még nem ért el a cél pozícióhoz
        if (isMoving && Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            // Mozgatja az objektumot a cél felé, anélkül, hogy az Y tengelyen mozgatná
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            transform.position += moveDirection * speed * Time.deltaTime;
        }
        else
        {
            isMoving = false;
        }
    }
}
