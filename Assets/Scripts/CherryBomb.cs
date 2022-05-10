using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CherryBomb : MonoBehaviour
{
    Rigidbody2D body;

    float fuseTimer = 0.4f;

    // Start is called before the first frame update
    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        fuseTimer -= Time.deltaTime;
        if (fuseTimer < 0)
        {
            Destroy(gameObject);
        }
    }

    public void Launch(Vector2 direction, float force)
    {
        // XXX: Player walking direction is affecting this (I think).
        // We need to fix this so the bomb is always launched with the
        // same trajectory.
        body.AddForce(direction * force);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        ZombieController zombie = collision.collider.GetComponent<ZombieController>();
        if (zombie != null)
        {
            zombie.Purify();
        }
        Destroy(gameObject);
    }
}
