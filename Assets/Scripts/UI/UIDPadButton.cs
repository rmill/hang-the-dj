using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
  
public class UIDPadButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	public Vector2 dir;
	
	public void OnPointerDown(PointerEventData eventData){
		InputManager.InputEvent(dir);
	}

	public void OnPointerUp(PointerEventData eventData){
		InputManager.InputEvent(Vector2.zero);
	}
}
