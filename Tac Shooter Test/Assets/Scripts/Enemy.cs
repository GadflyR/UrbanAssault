using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Enemy : MonoBehaviour
{
    public float maxSpeed = 400;
    public float currentSpeed;

    public float health;

    private float angerLevel;
    public float angerThreshold = 1.5f;

    //AI Handling
    public Transform currentTarget;
    public float nextWaypointDistance = 0.5f;
    private Path path;
    private int currentWaypoint;
    private bool reachedEndOfPath;

    private Seeker seeker;
    private Rigidbody2D rb;

    //Field of View stuff
    public GameObject fovPrefab;
    private FieldOfView fov;
    public float viewAngle;
    public float viewDistance;

    //States
    public int state;
    public Transform[] patrolCheckpoints;
    private int currentPatrolCheckpoint;

    //Starting stuff for idle enemies
    private Transform startTransform;
    public GameObject transformHolder;

    private bool isLookingAround;

    private Gun gun;

    private PlayerController player;

    public AudioClip[] footstepSFXs;
    private float footstepInterval;
    private float footstepCooldown;

    public GameObject[] bloods;
    public GameObject enemyDeath;
    private Animator anim;

    public bool isMoving = false;
    private AnimatorClipInfo[] clipInfo;
    bool otherAnim = false;

    private AudioSource source;
    public AudioClip[] angryVoiceLines;
    public AudioClip[] confusedVoiceLines;
    public AudioClip deathSound;
    public AudioClip[] hurtSounds;
    public AudioClip[] heardNoiseVoiceLines;
    public AudioClip[] lostSightVoiceLines;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        gun = GetComponentInChildren<Gun>();
        anim = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
        player = FindObjectOfType<PlayerController>();

        fov = Instantiate(fovPrefab).GetComponent<FieldOfView>();
        fov.fov = viewAngle;
        fov.viewDistance = viewDistance;

        startTransform = Instantiate(transformHolder, transform.position, transform.rotation).transform;
        currentTarget = startTransform;

        if (seeker == null)
        {
            Debug.LogError("Seeker component not found!");
        }
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component not found!");
        }
        if (gun == null)
        {
            Debug.LogError("Gun component not found!");
        }

        InvokeRepeating("UpdatePath", 0, 0.5f);
    }

    private void Update()
    {
        fov.SetOrigin(transform.position);
        fov.SetAimDirection(transform.right);

        if (currentTarget == null)
        {
            Debug.LogError("Current target is not set!");
            return;
        }

        if (CanSeePlayer())
            angerLevel += Time.deltaTime;
        else
            angerLevel -= Time.deltaTime;
        angerLevel = Mathf.Clamp(angerLevel, 0, 10);

        if (angerLevel > 0 && angerLevel < angerThreshold)
        {
            if (state != 3)
                source.PlayOneShot(confusedVoiceLines[Random.Range(0, confusedVoiceLines.Length)], 1);
            state = 3;
        }
        else if (angerLevel >= angerThreshold)
        {
            if (state != 2)
            {
                source.PlayOneShot(angryVoiceLines[Random.Range(0, angryVoiceLines.Length)], 1);
                angerLevel = 5;
            }
            state = 2;
        }
        else if (state != 4 && state != 5)
        {
            if (state != 0 && state != 1)
                source.PlayOneShot(lostSightVoiceLines[Random.Range(0, lostSightVoiceLines.Length)], 1);
            if (patrolCheckpoints.Length == 0)
                state = 0;
            else
                state = 1;
        }
        angerLevel = Mathf.Clamp(angerLevel, 0, 10);

        //Footsteps
        if (currentSpeed != 0)
            footstepInterval = 1 / (currentSpeed / 100);
        footstepCooldown -= Time.deltaTime;
        if (footstepCooldown <= 0 && currentSpeed != 0)
        {
            source.PlayOneShot(footstepSFXs[Random.Range(0, footstepSFXs.Length)], 1);
            footstepCooldown = footstepInterval;
        }

        //State Handling
        if (state == 0)
        {
            currentSpeed = 0;
            currentTarget = startTransform;
            if (Vector2.Distance(transform.position, currentTarget.position) < nextWaypointDistance)
                transform.rotation = startTransform.rotation;
        }
        else if (state == 1)
        {
            currentSpeed = 100;
            currentTarget = patrolCheckpoints[currentPatrolCheckpoint];

            if (path != null && currentWaypoint < path.vectorPath.Count)
            {
                LookAtTarget();
            }

            float distance = Vector2.Distance(transform.position, currentTarget.position);
            if (distance <= nextWaypointDistance)
            {
                currentPatrolCheckpoint++;
                if (currentPatrolCheckpoint >= patrolCheckpoints.Length)
                    currentPatrolCheckpoint = 0;
            }
        }
        else if (state == 2)
        {
            currentSpeed = maxSpeed;
            currentTarget = player.transform;

            LookAtTarget();

            if (CanSeePlayer())
            {
                RaycastHit2D hit = Physics2D.Linecast(transform.position, player.transform.position);
                if (hit.transform != null)
                {
                    if (hit.transform.TryGetComponent(out PlayerController player))
                    {
                        if (!gun.isReloading)
                        {
                            otherAnim = true;
                            anim.Play("EnemyShoot");
                            gun.Shoot();
                            currentSpeed = 0;
                        }
                    }
                }
            }
        }
        else if (state == 3)
        {
            currentSpeed = 0;
            currentTarget = player.transform;
            if (CanSeePlayer())
                LookAtTarget();
        }
        else if (state == 4)
        {
            currentSpeed = 250;
            LookAtTarget();
            if (Vector2.Distance(transform.position, currentTarget.position) < nextWaypointDistance)
            {
                state = 5;
                StartCoroutine(LookAround());
            }
        }
        else if (state == 5)
        {
            if (isLookingAround)
            {
                float rotation = Mathf.SmoothStep(-90, 90, Mathf.PingPong(Time.time / 2, 1));
                transform.rotation = Quaternion.Euler(0, 0, rotation);
            }
            else
            {
                if (patrolCheckpoints.Length == 0)
                    state = 0;
                else
                    state = 1;
            }
        }

        //Animations
        clipInfo = anim.GetCurrentAnimatorClipInfo(0);

        if (gun.isReloading)
        {
            otherAnim = true;
            if (gun.isShotgun)
            {
                anim.speed = 1f / 3f * 0.5f / gun.reloadTime;
                anim.Play("EnemyReloadShotgun");
            }
            else
            {
                anim.speed = 0.5f / gun.reloadTime;
                anim.Play("EnemyReload");
            }
        }

        if (clipInfo[0].clip.name != "EnemyMove" || clipInfo[0].clip.name != "EnemyIdle")
        {
            if (anim.GetCurrentAnimatorStateInfo(0).length < anim.GetCurrentAnimatorStateInfo(0).normalizedTime)
            {
                otherAnim = false;
            }
        }

        if (rb.velocity != Vector2.zero && otherAnim == false)
        {
            anim.Play("EnemyMove");
        }
        else if ((rb.velocity == Vector2.zero) && otherAnim == false)
        {
            anim.Play("EnemyIdle");
        }

        print(clipInfo[0].clip.name);
    }

    private void FixedUpdate()
    {
        if (path == null)
            return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * currentSpeed * Time.fixedDeltaTime;
        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
            currentWaypoint++;
    }

    public void UpdatePath()
    {
        if (seeker != null && seeker.IsDone() && currentTarget != null)
            seeker.StartPath(rb.position, currentTarget.position, OnPathComplete);
    }

    public void OnPathComplete(Path _path)
    {
        if (!_path.error)
        {
            path = _path;
            currentWaypoint = 0;
        }
    }

    public bool CanSeePlayer()
    {
        if (Vector2.Distance(transform.position, player.transform.position) <= viewDistance)
        {
            Vector2 direction = (player.transform.position - transform.position).normalized;
            if (Vector2.Angle(transform.right, direction) < viewAngle / 2)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, viewDistance);
                if (hit.transform != null)
                {
                    if (hit.transform.TryGetComponent(out PlayerController player))
                        return true;
                }
            }
        }
        return false;
    }

    public void LookAtTarget()
    {
        float AngleRad = Mathf.Atan2(currentTarget.position.y - transform.position.y, currentTarget.position.x - transform.position.x);
        float AngleDeg = Mathf.Rad2Deg * AngleRad;
        transform.rotation = Quaternion.Euler(0, 0, AngleDeg);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        angerLevel = 10;
        Instantiate(bloods[Random.Range(0, bloods.Length)], transform.position, Quaternion.Euler(0, 0, Random.Range(-180f, 180f)));
        rb.AddForce(-transform.right.normalized * damage / 2, ForceMode2D.Impulse);
        source.PlayOneShot(hurtSounds[Random.Range(0, hurtSounds.Length)], 1);
        if (health <= 0)
        {
            Instantiate(enemyDeath, transform.position, transform.rotation);
            source.PlayOneShot(deathSound);
            Destroy(gameObject);
        }
    }

    public void HearNoise(Transform noiseTransform)
    {
        source.PlayOneShot(heardNoiseVoiceLines[Random.Range(0, heardNoiseVoiceLines.Length)], 1);
        state = 4;
        currentTarget = noiseTransform;
    }

    public IEnumerator LookAround()
    {
        isLookingAround = true;
        yield return new WaitForSeconds(5);
        isLookingAround = false;
    }

    private void OnDestroy()
    {
        Destroy(fov.gameObject);
    }
}
