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

    public Transform currentTarget;
    public float nextWaypointDistance = 3;
    private Path path;
    private int currentWaypoint;
    private bool reachedEndOfPath;

    private Seeker seeker;
    private Rigidbody2D rb;

    private int state;
    public Transform[] patrolCheckpoints;
    private int currentPatrolCheckpoint;

    private Gun gun;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        gun = GetComponentInChildren<Gun>();

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
        if (currentTarget == null)
        {
            Debug.LogError("Current target is not set!");
            return;
        }

        if (angerLevel >= 2.5f)
            state = 2;
        else
        {
            if (patrolCheckpoints.Length == 0)
                state = 0;
            else
                state = 1;
        }
        angerLevel = Mathf.Clamp(angerLevel, 0, 10);

        if (state == 0)
            currentTarget = transform;
        else if (state == 1)
        {
            currentSpeed = 100;
            currentTarget = patrolCheckpoints[currentPatrolCheckpoint];

            if (path != null && currentWaypoint < path.vectorPath.Count)
            {
                float AngleRad = Mathf.Atan2(path.vectorPath[currentWaypoint].y - transform.position.y,
                    path.vectorPath[currentWaypoint].x - transform.position.x);
                float AngleDeg = Mathf.Rad2Deg * AngleRad;
                transform.rotation = Quaternion.Euler(0, 0, AngleDeg);
            }

            float distance = Vector2.Distance(rb.position, currentTarget.position);
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
            currentTarget = FindObjectOfType<PlayerController>().transform;

            float AngleRad = Mathf.Atan2(currentTarget.position.y - transform.position.y, currentTarget.position.x - transform.position.x);
            float AngleDeg = Mathf.Rad2Deg * AngleRad;
            transform.rotation = Quaternion.Euler(0, 0, AngleDeg);

            if (Vector2.Distance(transform.position, currentTarget.position) <= 10)
            {
                gun.Shoot();
                rb.velocity = Vector2.zero;
            }
        }
        else if (state == 3)
        {
            rb.velocity = Vector2.zero;
            currentTarget = FindObjectOfType<PlayerController>().transform;
            float AngleRad = Mathf.Atan2(currentTarget.position.y - transform.position.y, currentTarget.position.x - transform.position.x);
            float AngleDeg = Mathf.Rad2Deg * AngleRad;
            transform.rotation = Quaternion.Euler(0, 0, AngleDeg);
        }
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

    public void TakeDamage(float damage)
    {
        health -= damage;
        angerLevel = 10;
        if (health <= 0)
            Destroy(gameObject);
    }
}
