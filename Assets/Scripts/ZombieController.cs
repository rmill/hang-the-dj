using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieController : MonoBehaviour
{
    public float changeTime = 3.0f;
    public float speed;

    Animator animator;
    Rigidbody2D body;

    bool attacking;
    bool infected = true;
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
        if (infected)
        {
            if (!attacking)
            {
                timer -= Time.deltaTime;
                if (timer < 0)
                {
                    direction = -direction;
                    timer = changeTime;
                }

                animator.SetFloat("LookHorizontal", direction);
            }
        }
    }

    void FixedUpdate()
    {
        if (infected)
        {
            if (!attacking)
            {
                Vector2 position = body.position;
                position.x += Time.deltaTime * speed * direction;
                body.MovePosition(position);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            animator.SetTrigger("Attacking");
            attacking = true;
            player.ChangeHealth(-2);
        }
    }

    // The editor will say this method isn't in use, but it gets called
    // by Animation Events in the Zombie Attack animations
    void FinishAttack()
    {
        animator.ResetTrigger("Attacking");
        attacking = false;
    }

    public void Purify()
    {
        infected = false;
        body.simulated = false;
        animator.SetTrigger("Purified");
    }
}
