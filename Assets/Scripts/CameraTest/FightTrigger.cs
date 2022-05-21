using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FightTrigger : MonoBehaviour
{
    public PolygonCollider2D fightRingCollider;
    public CinemachineVirtualCamera virtualCamera;
    public GameObject[] enemies;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Set the current view as Virtual Camera confiner
        CinemachineConfiner confiner = virtualCamera.gameObject.GetComponent<CinemachineConfiner>();
        confiner.m_BoundingShape2D = fightRingCollider;
        
        // Set Enemies to attack
        foreach (var enemy in enemies)
        {
            enemy.GetComponent<EnemyController>().activate();
        }
    }
}
