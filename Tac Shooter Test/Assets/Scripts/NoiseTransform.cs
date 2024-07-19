using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseTransform : MonoBehaviour
{
    private float timeAlive;
    private Enemy[] enemies;

    private void Start()
    {
        enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
    }
    private void Update()
    {
        bool isNotTargeted = Array.TrueForAll(enemies, enemy => enemy.currentTarget != transform);
        timeAlive += Time.deltaTime;
        if (isNotTargeted && timeAlive >= 1.5f)
            Destroy(gameObject);
    }
}
