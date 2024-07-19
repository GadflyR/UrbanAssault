using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum WeaponType
    {
        Primary,
        Secondary
    }

    public WeaponType weaponType;
    public string description;
    public string gunName;
    public Sprite weaponSprite;
    public GameObject bulletPrefab;
    public Transform barrelOutPoint;

    public float bulletSpeed;
    public float damage;

    private float cooldown;
    public float firerate;

    private int ammo;
    public int magAmmo;
    public int remainingAmmo;

    public float reloadTime;
    public bool isReloading;

    public float spread;

    public bool isAutomatic;
    public bool isShotgun;
    public int shotsFired;

    public float noise;
    private bool isSilenced;
    public GameObject noisePrefab;

    private bool isHeldByPlayer;

    public List<Attachment> attachments = new List<Attachment>();

    public AudioClip shootSFX;
    public AudioClip silencedShootSFX;
    public AudioClip reloadSFX;
    public AudioClip switchSFX;
    public AudioClip outOfAmmoSFX;

    public Animator anim;
    private void Start()
    {
        if (UIManager.instance == null)
        {
            Debug.LogError("UIManager.instance is not set");
        }

        if (AudioManager.instance == null)
        {
            Debug.LogError("AudioManager.instance is not set");
        }

        attachments.AddRange(GetComponentsInChildren<Attachment>());
        foreach (Attachment attachment in attachments)
        {
            if (attachment.noiseModifier < 0)
                isSilenced = true;

            bulletSpeed += attachment.bulletSpeedModifier;
            damage += attachment.damageModifier;
            firerate += attachment.firerateModifier;
            magAmmo += attachment.ammoModifier;
            reloadTime += attachment.reloadTimeModifier;
            spread += attachment.spreadModifier;
            noise += attachment.noiseModifier;
        }

        bulletSpeed = Mathf.Clamp(bulletSpeed, 1, Mathf.Infinity);
        damage = Mathf.Clamp(damage, 1, Mathf.Infinity);
        firerate = Mathf.Clamp(firerate, Mathf.Epsilon, Mathf.Infinity);
        magAmmo = (int)Mathf.Clamp(magAmmo, 1, Mathf.Infinity);
        reloadTime = Mathf.Clamp(reloadTime, Mathf.Epsilon, Mathf.Infinity);
        spread = Mathf.Clamp(spread, 0, Mathf.Infinity);
        noise = Mathf.Clamp(noise, 0, Mathf.Infinity);

        cooldown = firerate;
        ammo = magAmmo;
        remainingAmmo = magAmmo * 5;

        isHeldByPlayer = transform.parent.TryGetComponent(out PlayerController player);

        anim = GetComponentInParent<Animator>();
    }

    private void Update()
    {
        //Reloading
        if (((Input.GetKeyDown(KeyCode.R) && ammo < magAmmo) || ammo <= 0) && !isReloading && remainingAmmo > 0)
            StartCoroutine(Reload());
        cooldown -= Time.deltaTime;

        //Update ammo UI
        if (UIManager.instance != null)
        {
            if (isReloading && !isShotgun)
                UIManager.instance.ammoText.text = $"X | {remainingAmmo}";
            else
                UIManager.instance.ammoText.text = $"{ammo} | {remainingAmmo}";
        }
    }
    public void Shoot()
    {
        //Cancel Reload if you're using a shotgun
        if (isShotgun && ammo >= 0 && isReloading)
            CancelReload();
        //Check if you can shoot
        if (cooldown <= 0 && !isReloading && ammo > 0)
        {
            for (int i = 0; i < shotsFired; i++)
            {
                Bullet bullet = Instantiate(bulletPrefab, barrelOutPoint.position, barrelOutPoint.rotation).GetComponent<Bullet>();
                bullet.damage = damage;
                Rigidbody2D bulletRB = bullet.GetComponent<Rigidbody2D>();
                bulletRB.velocity = (Vector2)barrelOutPoint.right * bulletSpeed + GenerateRandomSpread();
            }
            if (isHeldByPlayer)
            {
                VFXManager.instance.ShakeCam(0.1f, noise / 10);
                NoiseObject noiseObj = Instantiate(noisePrefab, transform.position, noisePrefab.transform.rotation).GetComponent<NoiseObject>();
                noiseObj.maxSize = noise;
            }
            AudioManager.instance.PlaySFX(isSilenced ? silencedShootSFX : shootSFX, 1);
            cooldown = firerate;
            ammo--;
        }
        else if (ammo <= 0)
            AudioManager.instance.PlaySFX(outOfAmmoSFX, 1);
    }
    public Vector2 GenerateRandomSpread()
    {
        Vector2 vector = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        return ((vector.magnitude > 1) ? vector.normalized : vector) * spread;
    }
    public IEnumerator Reload()
    {
        isReloading = true;

        AudioManager.instance.PlaySFX(reloadSFX, 1);
        if (isShotgun)
        {
            yield return new WaitForSeconds(reloadTime);
            ammo++;
            remainingAmmo--;
            if (ammo < magAmmo && remainingAmmo > 0)
                StartCoroutine(Reload());
            else
                isReloading = false;
        }
        else
        {
            yield return new WaitForSeconds(reloadTime);
            isReloading = false;
            if (remainingAmmo >= magAmmo || ammo + remainingAmmo > magAmmo)
            {
                remainingAmmo -= (magAmmo - ammo);
                ammo = magAmmo;
            }
            else
            {
                ammo += remainingAmmo;
                remainingAmmo = 0;
            }
        }
    }
    public void CancelReload()
    {
        StopAllCoroutines();
        AudioManager.instance.StopPlaying();
        isReloading = false;
    }
    private void OnEnable()
    {
        StartCoroutine(WaitForAudioManager());
    }

    private IEnumerator WaitForAudioManager()
    {
        while (AudioManager.instance == null)
        {
            yield return null; // Wait until the AudioManager instance is ready
        }
        AudioManager.instance.PlaySFX(switchSFX, 1);
    }
    private void OnDisable()
    {
        if (transform.parent.TryGetComponent(out PlayerController player))
            CancelReload();
    }
    public string GetDescription()
    {
        return description;
    }
}
