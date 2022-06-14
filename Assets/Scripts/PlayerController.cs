using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    public GameObject cherryBombPrefab;

    public float hitDuration = 1.0f;
    public float shootCooldown = 1.0f;
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
    float shootTimer;
    int currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        currentHealth = maxHealth;
        shootTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (shootTimer >= 0)
        {
            shootTimer -= Time.deltaTime;
        }

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
        // Disable user input while player is hit
        {
            directionX = Input.GetAxisRaw("Horizontal");
            directionY = Input.GetAxisRaw("Vertical");

            Vector2 move = new Vector2(directionX, directionY);
            if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
            {
                lookDirection.Set(move.x, move.y);
                lookDirection.Normalize();
            }

            animator.SetFloat("LookHorizontal", lookDirection.x);
            animator.SetFloat("Speed", move.magnitude);

            if (shootTimer < 0)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Launch();
                }
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                RaycastHit2D hit = Physics2D.Raycast(body.position + Vector2.up * 0.6f, lookDirection, 1.0f, LayerMask.GetMask("Interactables"));
                if (hit.collider != null)
                {
                    InteractableController interactable = hit.collider.GetComponent<InteractableController>();
                    if (interactable != null)
                    {
                        interactable.DisplayDialog();
                    }
                }
            }
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
            // XXX: handle healing animation
        } else
        {
            HitPlayer();
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        if (currentHealth <= 0)
        {
            // XXX: handle dealth
        }
        if (UIHealthBar.instance != null)
        {
            UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
        }
    }

    void Launch()
    {
        GameObject instance = Instantiate(cherryBombPrefab, body.position + Vector2.up * 0.7f, Quaternion.identity);
        CherryBomb cherryBomb = instance.GetComponent<CherryBomb>();
        cherryBomb.Launch(lookDirection, 300);

        shootTimer = shootCooldown;

        // XXX: create throwing animation and setup state machines
        animator.SetTrigger("Throw");
    }
}
