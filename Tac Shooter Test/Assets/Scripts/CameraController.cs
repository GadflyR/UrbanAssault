using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private const float zOffset = -10;
    public float maxDistance = 10;

    private void FixedUpdate()
    {
        GameObject player = FindObjectOfType<PlayerController>().gameObject;
        Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        transform.position = ((Vector2)player.transform.position + cursorPos) / 2;
        transform.position = new Vector3(transform.position.x, transform.position.y, zOffset);
    }
}
