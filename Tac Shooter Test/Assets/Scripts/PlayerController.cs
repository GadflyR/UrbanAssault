using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;

    public float moveSpeed;

    public float health = 100;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Cursor.lockState = CursorLockMode.Confined;
    }
    private void Update()
    {
        //Movement
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        rb.velocity = new Vector2(horizontalInput, verticalInput) * moveSpeed;

        //Walking / Running
        if (Input.GetKeyDown(KeyCode.LeftShift))
            moveSpeed /= 2;
        if (Input.GetKeyUp(KeyCode.LeftShift))
            moveSpeed *= 2;

        //Shooting
        Gun activeGun = GetComponentInChildren<Gun>();
        if (activeGun.isAutomatic)
        {
            if (Input.GetMouseButton(0))
                activeGun?.Shoot();
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
                activeGun?.Shoot();
        }

        //Kicking
        if (Input.GetKeyDown(KeyCode.G))
        {
            print("G Pressed");
            if (Physics.Raycast(transform.position, transform.right, 1, LayerMask.NameToLayer("Doors")))
            {
                print("Hit Door");
            }
        }
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
    }
}
