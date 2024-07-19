using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseObject : MonoBehaviour
{
    public float maxSize;

    public GameObject noisePos;
    private Transform noiseTransform;

    private void Start()
    {
        noiseTransform = Instantiate(noisePos, transform.position, Quaternion.identity).transform;
        StartCoroutine(ScaleOverTime(0.05f, maxSize));
    }
    private void Update()
    {
        if (transform.localScale.x <= 0.01f)
            Destroy(gameObject);
    }
    private IEnumerator ScaleOverTime(float duration, float scale)
    {
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.one * scale;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            var t = timeElapsed / duration;
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = endScale;
        StartCoroutine(ScaleOverTime(0.5f, 0));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Enemy enemy))
            enemy.HearNoise(noiseTransform);
    }
}
