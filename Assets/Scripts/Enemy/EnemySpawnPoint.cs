using UnityEngine;
using System.Collections;

public class EnemySpawnPoint : MonoBehaviour {

	void Start () {
		GameObject enemy = GameObject.Instantiate(Resources.Load("Enemy"), transform.position, Quaternion.identity) as GameObject;
		enemy.name = "Enemy";
	}
}
