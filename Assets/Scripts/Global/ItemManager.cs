using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ItemManager {

	public static List<GameObject> itemList = new List<GameObject>();

	public static void AddItemToList( GameObject g ){
		itemList.Add(g);
	}

	public static void RemoveItemFromList( GameObject g ){
		itemList.Remove(g);
	}

	public static void clearItemList(){
		itemList.Clear();
	}
}
