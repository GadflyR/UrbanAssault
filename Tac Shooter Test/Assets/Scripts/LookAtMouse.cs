using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtMouse : MonoBehaviour
{
    public FieldOfView fieldOfView;

    private void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float AngleRad = Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x);
        float AngleDeg = Mathf.Rad2Deg * AngleRad;
        transform.rotation = Quaternion.Euler(0, 0, AngleDeg);



        // DONT TOUCH (FOV script)
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        Vector3 direction = new Vector2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y);
        fieldOfView.SetOrigin(transform.position);
        fieldOfView.SetAimDirection(direction);
    }
}
