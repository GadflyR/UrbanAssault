using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;

    public float maxSpeed;
    private float moveSpeed;

    private float footstepCooldown = 1;

    public GameObject noisePrefab;

    public float health = 100;
    private HealthBarController healthBar;

    private List<Gun> guns = new List<Gun>();

    public LayerMask doors;

    public Transform kickPoint;

    public AudioClip[] footstepSFXs;
    public GameObject bloodyFootstep;
    public GameObject playerDeath;
    public GameObject[] bloods;

    private Vignette vignette;

    private Animator anim;

    public bool isMoving = false;
    private AnimatorClipInfo[] clipInfo;
    bool otherAnim = false;

    public AudioClip deathSFX;

    private Coroutine regenHealthCoroutine;
    public AudioClip heartbeatSFX;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        healthBar = FindObjectOfType<HealthBarController>();
        Cursor.lockState = CursorLockMode.Confined;

        SwapGun(0);

        healthBar.SetHealth(health);

        Volume volume = FindObjectOfType<Volume>();
        if (volume != null && volume.profile.TryGet(out Vignette v))
        {
            vignette = v;
        }

        // 初始化Vignette颜色
        if (vignette != null)
        {
            vignette.color.value = Color.black; // 设置为默认颜色（黑色）
        }
    }

    private void Update()
    {
        // WASD 控制移动
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        rb.velocity = new Vector2(horizontalInput, verticalInput) * moveSpeed;

        if (IsInTutorialScene() && TutorialManager.instance.isInCutscene)
        {
            // 在 Tutorial 场景且在 cutscene 中时，不执行后续代码
            rb.velocity = Vector2.zero;
            return;
        }

        //Walking / Running
        if (Input.GetKey(KeyCode.LeftShift))
            moveSpeed = maxSpeed / 2;
        else
            moveSpeed = maxSpeed;

        footstepCooldown -= Time.deltaTime;
        if (footstepCooldown <= 0 && rb.velocity != Vector2.zero)
        {
            NoiseObject noise = Instantiate(noisePrefab, transform.position, noisePrefab.transform.rotation).GetComponent<NoiseObject>();
            noise.maxSize = moveSpeed;
            if (health <= 30)
                Instantiate(bloodyFootstep, transform.position, Quaternion.Euler(0, 0, Random.Range(-180f, 180f)));
            if (Input.GetKey(KeyCode.LeftShift))
            {
                AudioManager.instance.PlaySFX(footstepSFXs[Random.Range(0, footstepSFXs.Length)], 0.5f);
                footstepCooldown = 0.5f;
            }
            else
            {
                AudioManager.instance.PlaySFX(footstepSFXs[Random.Range(0, footstepSFXs.Length)], 1);
                footstepCooldown = 0.35f;
            }
        }

        //Add Guns
        if (guns.Count == 0)
        {
            // 确保在实例化武器后调用 AddRange
            guns.AddRange(GetComponentsInChildren<Gun>(true));
        }

        Gun activeGun = guns.Find(gun => gun.isActiveAndEnabled);
        if (activeGun != null)
        {
            if (activeGun.isAutomatic)
            {
                if (Input.GetMouseButton(0))
                {
                    otherAnim = true;
                    anim.Play("Shoot");
                    activeGun.Shoot();
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    otherAnim = true;
                    anim.Play("Shoot");
                    activeGun.Shoot();
                }
            }

            // 检查 activeGun 是否为 null
            if (activeGun != null && activeGun.isReloading)
            {
                otherAnim = true;
                if (activeGun.isShotgun)
                {
                    anim.speed = 1f / 3f * 0.5f / activeGun.reloadTime;
                    anim.Play("ReloadShotgun");
                }
                else
                {
                    anim.speed = 0.5f / activeGun.reloadTime;
                    anim.Play("Reload");
                }
            }
        }

        //Swap Guns
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SwapGun(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            SwapGun(1);

        //Kicking
        if (Input.GetKeyDown(KeyCode.G))
        {
            anim.speed = 1f;
            otherAnim = true;
            anim.Play("Kick");
            RaycastHit2D hit = Physics2D.Raycast(kickPoint.position, kickPoint.right, 1.25f, doors);
            if (hit.collider != null)
            {
                if (hit.collider.TryGetComponent(out Door door))
                    door.Kick();
            }
        }

        clipInfo = anim.GetCurrentAnimatorClipInfo(0);

        //Stops walk or idle animation until another animation has finished
        if (clipInfo[0].clip.name != "Move" || clipInfo[0].clip.name != "Idle")
        {
            if (anim.GetCurrentAnimatorStateInfo(0).length < anim.GetCurrentAnimatorStateInfo(0).normalizedTime)
                otherAnim = false;
        }

        if (rb.velocity != Vector2.zero && otherAnim == false)
            anim.Play("Move");
        else if ((rb.velocity == Vector2.zero) && otherAnim == false)
            anim.Play("Idle");

        if (vignette != null)
        {
            if (health <= 30)
                vignette.color.value = Color.Lerp(vignette.color.value, Color.red, Mathf.PingPong(Time.time, 1)); // 血量低于30时设置为红色
            else
                vignette.color.value = Color.Lerp(vignette.color.value, Color.black, Mathf.PingPong(Time.time, 1)); // 正常状态设置为黑色
        }

        if (health < 50 && regenHealthCoroutine == null)
            regenHealthCoroutine = StartCoroutine(RegenHealth());
    }

    public void SwapGun(int index)
    {
        if (index >= 0 && index < guns.Count)
        {
            foreach (Gun gun in guns)
                gun.gameObject.SetActive(false);
            guns[index].gameObject.SetActive(true);
            AudioManager.instance.PlaySFX(guns[index].switchSFX, 1);
        }
        else
            Debug.LogError("Gun index out of range");
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        healthBar.SetHealth(health);
        Instantiate(bloods[Random.Range(0, bloods.Length)], transform.position, Quaternion.Euler(0, 0, Random.Range(-180f, 180f)));
        rb.AddForce(-transform.right.normalized * damage, ForceMode2D.Impulse);

        if (health <= 0)
        {
            Instantiate(playerDeath, transform.position, transform.rotation);
            StopAllAudio(); // 停止所有音频
            NotifyEnemiesOfDeath(); // 通知敌人玩家已死亡
            UIManager.instance.SwitchUIs(1); // 显示游戏结束UI
            Debug.Log("GameManager.Instance: " + (GameManager.Instance != null ? "Exists" : "Does not exist"));
            AudioManager.instance.PlaySFX(deathSFX, 1);
            Destroy(gameObject);
        }
    }

    // 新增：停止所有音频的方法
    private void StopAllAudio()
    {
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audioSource in allAudioSources)
            audioSource.Stop();
    }

    // 新增：通知所有敌人玩家已死亡
    private void NotifyEnemiesOfDeath()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
            enemy.PlayerDied();
    }

    private bool IsInTutorialScene()
    {
        return SceneManager.GetActiveScene().name == "Tutorial";
    }

    private IEnumerator RegenHealth()
    {
        while (health < 50)
        {
            health += 1;
            healthBar.SetHealth(health);
            AudioManager.instance.PlaySFX(heartbeatSFX, 1);
            yield return new WaitForSeconds(0.5f); // 每0.5秒恢复1点血量
        }
        regenHealthCoroutine = null; // 协程结束，重置为null
    }
}
