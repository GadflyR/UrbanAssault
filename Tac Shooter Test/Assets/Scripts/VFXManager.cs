using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager instance;

    public bool VFXOn = true;

    private Transform cameraTransform; //Make sure to set this to the camera object in the inspector
    // How long the object should shake for. Set by outside function
    public float shakeDuration = 0f;
    // Amplitude of the shake. A larger value shakes the camera harder.
    public float shakeAmount = 0.7f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void OnLevelWasLoaded(int level)
    {
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        //Camera Shake
        if (shakeDuration > 0)
        {
            //lerp the position to a random position within a "sphere" to shake the camera
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, Random.insideUnitSphere * shakeAmount, .2f);
            //decrement the duration timer
            shakeDuration -= Time.deltaTime;
        }
        else
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, Vector2.zero, .2f);
    }

    public void ShakeCam(float duration = .3f, float amt = .7f)
    {
        if (VFXOn)
        {
            shakeDuration = duration;
            shakeAmount = amt;
        }
    }
}
