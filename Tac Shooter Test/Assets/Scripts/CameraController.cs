using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private const float zOffset = -10;
    public float maxDistance = 10;
    private PlayerController player; // 缓存 PlayerController 对象

    private void Start()
    {
        player = FindObjectOfType<PlayerController>(); // 在 Start 方法中获取 PlayerController
    }

    private void FixedUpdate()
    {
        if (player == null)
        {
            // 尝试重新获取 PlayerController 对象
            player = FindObjectOfType<PlayerController>();

            // 如果仍然为 null，则不执行后续代码
            if (player == null)
                return;
        }

        Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        transform.position = ((Vector2)player.transform.position + cursorPos) / 2;
        transform.position = new Vector3(transform.position.x, transform.position.y, zOffset);
    }
}
