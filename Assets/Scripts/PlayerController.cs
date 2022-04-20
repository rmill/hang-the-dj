using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float hitDuration = 1.0f;
    public float speedX = 3.0f;
    public float speedY = 2.0f;
    public int maxHealth = 5;
    public int health { get { return currentHealth; }}

    Animator animator;
    Rigidbody2D body;

    Vector2 lookDirection = new Vector2(1,0);

    bool isHit;
    bool pushed;
    float directionX;
    float directionY;
    float hitTimer;
    int currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {

        if (isHit)
        {
            hitTimer -= Time.deltaTime;
            if (hitTimer < 0)
            {
                isHit = false;
                pushed = false;
                animator.ResetTrigger("Hit");
            }
        } else
        {
            // Disable user input while player is hit
            directionX = Input.GetAxisRaw("Horizontal");
            directionY = Input.GetAxisRaw("Vertical");

            Vector2 move = new Vector2(directionX, directionY);
            if (!Mathf.Approximately(move.x, 0.0f))
            {
                lookDirection.Set(move.x, move.y);
                lookDirection.Normalize();
            }

            animator.SetFloat("LookHorizontal", lookDirection.x);
            animator.SetFloat("Speed", move.magnitude);
        }
    }

    // Use this for physics
    private void FixedUpdate()
    {
        if (isHit)
        {
            if (!pushed)
            {
                Vector2 pushDirection = new Vector2(directionX * -1.0f, 0.0f);
                body.AddForce(pushDirection, ForceMode2D.Impulse);
                pushed = true;
            }
        } else
        {
            Vector2 position = transform.position;
            position.x += speedX * directionX * Time.deltaTime;
            position.y += speedY * directionY * Time.deltaTime;
            body.MovePosition(position);
        }
    }

    void HitPlayer()
    {
        if (isHit)
            return;

        isHit = true;
        hitTimer = hitDuration;
        animator.SetTrigger("Hit");
    }

    public void ChangeHealth(int amount)
    {
        if (amount > 0)
        {
            // handle healing animation
        } else
        {
            HitPlayer();
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        if (currentHealth <= 0)
        {
            // handle dealth
        }
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }
}
