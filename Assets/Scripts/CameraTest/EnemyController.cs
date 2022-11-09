using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 1;
    public int direction = 1;
    private bool isActivated = false;
    

    public void activate()
    {
        isActivated = true;
        
        // Make any invisible enemies solid and visible
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.enabled = true;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = true;
    }
    
    void FixedUpdate()
    {
        if (!isActivated) return;
        
        Rigidbody2D body = GetComponent<Rigidbody2D>();
        Vector2 position = body.position;
        position.x += Time.deltaTime * speed * direction;
        body.MovePosition(position);
    }
}
