using UnityEngine;
using System.Collections;

public class SetSortingOrder : MonoBehaviour {

	public Renderer renderder;
	public int SortingOrder = 0;
	public bool BaseSortingOnYPos;

	void Start(){
		SetSorting();
	}

	void SetSorting() {
		if(renderder != null){
			if(BaseSortingOnYPos) 
				renderder.sortingOrder = Mathf.RoundToInt(transform.position.y*-10f);
			else
				renderder.sortingOrder = SortingOrder;
		}
	}
}
