using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private Rigidbody2D rb;

    private const float angle = .95f;
    private float rotateSpeed = 0;
    private bool side;

    public float noise;
    public GameObject noisePrefab;

    public AudioClip kickSFX;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.centerOfMass = new Vector2(-0.45f, 0);
    }

    private void FixedUpdate()
    {
        if (transform.rotation.z > angle || transform.rotation.z < -angle)
            rotateSpeed = 0;

        if (side)
            rb.MoveRotation(rb.rotation - rotateSpeed * Time.fixedDeltaTime);
        else
            rb.MoveRotation(rb.rotation + rotateSpeed * Time.fixedDeltaTime);
    }

    public void Kick()
    {
        Plane plane = new Plane(transform.right, transform.position);
        Transform player = FindObjectOfType<PlayerController>().GetComponent<Transform>();
        side = plane.GetSide(player.position);

        rotateSpeed = 750;
        NoiseObject _noise = Instantiate(noisePrefab, transform.position, noisePrefab.transform.rotation).GetComponent<NoiseObject>();
        _noise.maxSize = noise;
        AudioManager.instance.PlaySFX(kickSFX, 1);
        StartCoroutine(SlowTime());
    }

    public IEnumerator SlowTime()
    {
        Time.timeScale = 0.5f;
        yield return new WaitForSeconds(1.5f);
        Time.timeScale = 1;
    }
}
