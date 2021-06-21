using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class EnemyManager {

	public static List<GameObject> enemyList = new List<GameObject>();
	public static List<GameObject> enemiesAttackingPlayer = new List<GameObject>();
	public static List<GameObject> activeEnemies = new List<GameObject>();
	static GameSettings settings; 


	static GameSettings GetSettings(){
		if(settings == null) settings = Resources.Load("GameSettings", typeof(GameSettings)) as GameSettings;
		return settings;
	}

	public static void RemoveEnemyFromList( GameObject g ){
		enemyList.Remove(g);
		SetEnemyTactics();
	}


	//sets the tactics for each enemy in the currently active wave
	public static void SetEnemyTactics(){
		getActiveEnemies();

		if(activeEnemies.Count > 0){
			for(int i=0; i<activeEnemies.Count; i++){
				if(i < MaxEnemyAttacking()){
					activeEnemies[i].GetComponent<EnemyAI>().SetEnemyTactic(ENEMYTACTIC.ENGAGE);
				} else {
					activeEnemies[i].GetComponent<EnemyAI>().SetEnemyTactic(ENEMYTACTIC.KEEPMEDIUMDISTANCE);
				}
			}
		}
	}

	//forces all enemies to use a certain tactic
	public static void ForceEnemyTactic(ENEMYTACTIC tactic){
		getActiveEnemies();
		if(activeEnemies.Count > 0){
			for(int i=0; i<activeEnemies.Count; i++){
				activeEnemies[i].GetComponent<EnemyAI>().SetEnemyTactic(tactic);
			}
		}
	}

	//returns a list of enemies that are currently active
	public static void getActiveEnemies(){
		activeEnemies.Clear();
		foreach(GameObject enemy in enemyList){
			if(enemy != null && enemy.activeSelf)activeEnemies.Add(enemy);
		}
	}

	//player has died, stop all enemy attacks
	public static void PlayerHasDied(){
		ForceEnemyTactic(ENEMYTACTIC.KEEPMEDIUMDISTANCE);
		enemyList.Clear();
	}

	//returns the maximum number of enemies that can attack the player at once (Tools/Game Settings)
	static int MaxEnemyAttacking(){
		return GetSettings().MaxAttackers;
	}
}
