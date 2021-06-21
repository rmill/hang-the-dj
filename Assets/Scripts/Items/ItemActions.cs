using UnityEngine;
using System.Collections;

public class ItemActions : MonoBehaviour  {

	[HideInInspector]
	public GameObject target;
	public Item item;

	public void GiveHealthToPlayer(){
		HealthSystem ph = target.GetComponent<HealthSystem>();
		if(ph != null && item != null) ph.AddHealth(item.data);
	}

	public void GiveWeaponToPlayer(){
		PlayerCombat pc = target.GetComponent<PlayerCombat>();
		if(pc != null) pc.EquipWeapon(item);
	}

	public void DestroyDrumBarrel(){

		//show hit effect
		GameObject.Instantiate(Resources.Load("HitEffect"), transform.position + Vector3.up * 1.2f, Quaternion.identity);

		//spawn broken crate drum barrel destroy animation
		GameObject g = GameObject.Instantiate(Resources.Load("DrumbarrelDestroyed"), transform.position, Quaternion.identity) as GameObject;

		//turn object towards the direction of the target
		if(target.transform.position.x > transform.position.x) g.transform.localScale = new Vector3(-1,1,1);

		//spawn an item
		if(item.SpawnObjectOnDestroy != null) SpawnItem();
	}

	public void DestroyWoodenCrate(){

		//show hit effect
		GameObject.Instantiate(Resources.Load("HitEffect"), transform.position + Vector3.up * 1.2f, Quaternion.identity);

		//spawn crate destroy animation
		GameObject g = GameObject.Instantiate(Resources.Load("WoodenCrateDestroyed"), transform.position, Quaternion.identity) as GameObject;

		//turn object towards the direction of the target
		if(target.transform.position.x > transform.position.x) g.transform.localScale = new Vector3(-1,1,1);

		//flip the animation into direction of the target
		if(target.transform.position.x > transform.position.x) g.transform.localScale = new Vector3(-1,1,1);

		//spawn an item
		if(item.SpawnObjectOnDestroy != null) SpawnItem();
	}

	//spawn an item
	public void SpawnItem(){
		GameObject.Instantiate(item.SpawnObjectOnDestroy, transform.position, Quaternion.identity);
	}
}
