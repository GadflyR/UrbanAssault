using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
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
    private bool isReloading;

    public float spread;

    public bool isAutomatic;

    public bool isShotgun;
    public int shotsFired;

    public float noise;

    private bool canShoot = true;

    public List<Attachment> attachments = new List<Attachment>();

    private AudioManager audioM;
    public AudioClip shootSFX;
    public AudioClip reloadSFX;

    private void Start()
    { 
        //Get Attachments
        attachments.AddRange(GetComponentsInChildren<Attachment>());
        //Apply attachment modifiers
        foreach (Attachment attachment in attachments)
        {
            bulletSpeed += attachment.bulletSpeedModifier;
            damage += attachment.damageModifier;
            firerate += attachment.firerateModifier;
            magAmmo += attachment.ammoModifier;
            reloadTime += attachment.reloadTimeModifier;
            spread += attachment.spreadModifier;
            noise += attachment.noiseModifier;
        }
        //Clamp values so game doesn't SHIT ITSELF
        bulletSpeed = Mathf.Clamp(bulletSpeed, 1, Mathf.Infinity);
        damage = Mathf.Clamp(damage, 1, Mathf.Infinity);
        firerate = Mathf.Clamp(firerate, Mathf.Epsilon, Mathf.Infinity);
        magAmmo = (int)Mathf.Clamp(magAmmo, 1, Mathf.Infinity);
        reloadTime = Mathf.Clamp(reloadTime, Mathf.Epsilon, Mathf.Infinity);
        spread = Mathf.Clamp(spread, 0, Mathf.Infinity);
        noise = Mathf.Clamp(noise, 0, Mathf.Infinity);

        //Initialize cooldown & ammo
        cooldown = firerate;
        ammo = magAmmo;
        remainingAmmo = magAmmo * 5;

        audioM = AudioManager.instance;
    }
    private void Update()
    {
        //Reloading
        if (((Input.GetKeyDown(KeyCode.R) && ammo < magAmmo) || ammo <= 0) && !isReloading && remainingAmmo > 0)
            StartCoroutine(Reload());
        cooldown -= Time.deltaTime;

        //Update ammo UI
        if (isReloading && !isShotgun)
            UIManager.instance.ammoText.text = $"X | {remainingAmmo}";
        else
            UIManager.instance.ammoText.text = $"{ammo} | {remainingAmmo}";
    }
    public void Shoot()
    {
        //Cancel Reload if you're using a shotgun
        if (isShotgun && ammo >= 0)
            CancelReload();
        //Check if you can shoot
        if (cooldown <= 0 && !isReloading && ammo > 0 && canShoot)
        {
            for (int i = 0; i < shotsFired; i++)
            {
                Bullet bullet = Instantiate(bulletPrefab, barrelOutPoint.position, barrelOutPoint.rotation).GetComponent<Bullet>();
                bullet.damage = damage;
                Rigidbody2D bulletRB = bullet.GetComponent<Rigidbody2D>();
                bulletRB.velocity = (Vector2)barrelOutPoint.right * bulletSpeed + GenerateRandomSpread();
            }
            audioM.PlaySFX(shootSFX, noise / 25);
            cooldown = firerate;
            ammo--;
        }
    }
    public Vector2 GenerateRandomSpread()
    {
        Vector2 vector = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        return ((vector.magnitude > 1) ? vector.normalized : vector) * spread;
    }
    public IEnumerator Reload()
    {
        isReloading = true;
        audioM.PlaySFX(reloadSFX, 1);
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
            if (remainingAmmo >= magAmmo)
            {
                remainingAmmo -= (magAmmo - ammo);
                ammo = magAmmo; 
            }
            else
            {
                ammo = remainingAmmo;
                remainingAmmo = 0;
            }
        }
    }
    public void CancelReload()
    {
        StopAllCoroutines();
        isReloading = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Walls"))
            canShoot = false;
        //gameObject.SetActive(false);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Walls"))
            canShoot = true;
        //gameObject.SetActive(false);
    }
}
