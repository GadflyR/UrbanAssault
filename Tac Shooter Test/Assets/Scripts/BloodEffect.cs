using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodEffect : MonoBehaviour
{
    public float lifetime;
    private float timer;
    private SpriteRenderer sprite;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        timer = lifetime;
    }
    private void Update()
    {
        timer -= Time.deltaTime;
        sprite.color = new Color(255, 255, 255, timer / lifetime);
        if (timer <= 0)
            Destroy(gameObject);
    }
}
