using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieController : MonoBehaviour
{
    public float changeTime = 3.0f;
    public float speed;

    Animator animator;
    Rigidbody2D body;

    float timer;
    int direction = 1;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        timer = changeTime;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }

        animator.SetFloat("LookHorizontal", direction);
    }

    void FixedUpdate()
    {
        Vector2 position = body.position;
        position.x += Time.deltaTime * speed * direction;
        body.MovePosition(position);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            player.ChangeHealth(-2);
        }
    }
}
