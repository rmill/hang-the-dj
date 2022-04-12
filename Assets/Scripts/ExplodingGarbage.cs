using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingGarbage : MonoBehaviour
{
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();    
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        animator.SetTrigger("Exploding");

        PlayerController player = collider.GetComponent<PlayerController>();
        if (player != null)
        {
            player.ChangeHealth(-1);
        }
    }
}
