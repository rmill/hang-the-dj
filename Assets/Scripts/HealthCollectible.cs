using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    public GameObject healingEffect;

    void OnTriggerEnter2D(Collider2D collider)
    {
        PlayerController player = collider.GetComponent<PlayerController>();
        if (player != null)
        {
            if (player.health < player.maxHealth)
            {
                Instantiate(healingEffect, gameObject.transform.position, Quaternion.Euler(-90, 0, 0));
                player.ChangeHealth(1);
                Destroy(gameObject);
            }
        }
    }
}
