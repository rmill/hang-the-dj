using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveX = 3.0f;
    public float moveY = 2f;

    Rigidbody2D rigidbody2d;
    Animator animator;

    Vector2 lookDirection = new Vector2(1,0);

    float horizontal;
    float vertical;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);
        if (!Mathf.Approximately(move.x, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("LookHorizontal", lookDirection.x);
        animator.SetFloat("Speed", move.magnitude);
    }

    // Use this for physics
    private void FixedUpdate()
    {
        Vector2 position = transform.position;
        position.x += moveX * horizontal * Time.deltaTime;
        position.y += moveY * vertical * Time.deltaTime;
        rigidbody2d.MovePosition(position);
    }
}
