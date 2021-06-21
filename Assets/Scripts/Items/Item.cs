using UnityEngine;
using System.Collections;

[System.Serializable]
public class Item {

	public string itemName;	//the name of this item
	public string sfx = ""; //sfx to play on interaction
	public string callMethod = ""; //method to call when this item has been pickup up
	public int data = 0; //extra data to send with this item (e.g. health recovery amount or damage amount)
	public bool isPickup; // set to true if this item is a pickup
	public GameObject SpawnObjectOnDestroy; //spawns another gameobject on destroy
}