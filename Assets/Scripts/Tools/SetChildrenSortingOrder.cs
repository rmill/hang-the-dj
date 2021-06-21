using UnityEngine;
using System.Collections;

public class SetChildrenSortingOrder : MonoBehaviour {

	private Renderer[] renderers;
	private SpriteRenderer[] spriteRenderers;

	void Start(){
		SetSorting();
	}

	//sets the sorting of renderers and spriteRenderers to the y position
	void SetSorting() {
		renderers = GetComponentsInChildren<Renderer>();
		spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

		//apply sorting to renderers
		foreach(Renderer r in renderers){
			if(r != null){
				r.sortingOrder = Mathf.RoundToInt(transform.position.y*-10f);
			}
		}

		//apply sorting to sprite renderers
		foreach(SpriteRenderer r in spriteRenderers){
			if(r != null){
				r.sortingOrder = Mathf.RoundToInt(transform.position.y*-10f);
			}
		}

	}
}
