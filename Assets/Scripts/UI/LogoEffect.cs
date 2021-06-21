using UnityEngine;
using System.Collections;

public class LogoEffect : MonoBehaviour {

	public Material logoMaterial;
	private float texOffset = 0;
	public float speed;

	void Update () {
		texOffset += Time.deltaTime * speed; 
		logoMaterial.SetTextureOffset("_Overlay", new Vector2(-texOffset, texOffset));
	}
}
