using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Enemy : MonoBehaviour
{
    public float maxSpeed = 400;
    public float currentSpeed;

    public float health;

    //AI Variables
    public Transform currentTarget;
    public float nextWaypointDistance = 3;
    private Path path;
    private int currentWaypoint;
    private bool reachedEndOfPath = false;

    private Seeker seeker;
    private Rigidbody2D rb;

    //State the enemy is in. 0 = Idle, 1 = Patrolling, 2 = Chasing the Player
    private int state;
    public Transform[] patrolCheckpoints;
    private int currentPatrolCheckpoint;

    //Gun
    private Gun gun;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        gun = GetComponentInChildren<Gun>();

        if (patrolCheckpoints.Length == 0)
            state = 0;
        else
            state = 1;

        InvokeRepeating("UpdatePath", 0, 0.5f);
    }
    private void Update()
    {
        //Looking at the target
        float AngleRad = Mathf.Atan2(currentTarget.position.y - transform.position.y, currentTarget.position.x - transform.position.x);
        float AngleDeg = Mathf.Rad2Deg * AngleRad;
        transform.rotation = Quaternion.Euler(0, 0, AngleDeg);

        //State Handling
        if (state == 0)
            currentTarget = transform;
        else if (state == 1)
        {
            currentSpeed = 100;
            currentTarget = patrolCheckpoints[currentPatrolCheckpoint];
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
            if (Vector2.Distance(transform.position, currentTarget.position) <= 10)
            {
                gun.Shoot();
                rb.velocity = Vector2.zero;
            }
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
        if (seeker.IsDone())
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
        state = 2;
        if (health <= 0)
            Destroy(gameObject);
    }
}
