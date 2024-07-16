using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;

    public float moveSpeed;

    public float health = 100;
    public HealthBarController healthBar;

    private List<Gun> guns = new List<Gun>();

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Cursor.lockState = CursorLockMode.Confined;

        guns.AddRange(GetComponentsInChildren<Gun>(true));
        SwapGun(0);

        healthBar.SetHealth(health);
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

        Gun activeGun = guns.Find(gun => gun.isActiveAndEnabled);
        if (activeGun.isAutomatic)
        {
            if (Input.GetMouseButton(0))
                activeGun.Shoot();
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
                activeGun.Shoot();
        }

        //Swap Guns
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SwapGun(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            SwapGun(1);

        //Kicking
        if (Input.GetKeyDown(KeyCode.G))
        {
            //Kick Door here
        }
    }
    public void SwapGun(int index)
    {
        foreach (Gun gun in guns)
            gun.gameObject.SetActive(false);
        guns[index].gameObject.SetActive(true);
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
        healthBar.SetHealth(health);
    }
}
