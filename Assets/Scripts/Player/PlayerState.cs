using UnityEngine;
using System.Collections;

//this script decides the order of imporance of playerstates. For Example you can't attack if you are being hit, therefore an attack state while your are hit is ignored
public class PlayerState : MonoBehaviour {

	public PLAYERSTATE currentState = PLAYERSTATE.IDLE;

	public void SetState(PLAYERSTATE state){
		currentState = state;
	}
}

public enum PLAYERSTATE
{
	IDLE,
	MOVING,
	JUMPING,
	JUMPKICK,
	PUNCH,
	KICK,
	DEFENDING,
	HIT,
	DEATH,
	THROWKNIFE,
	PICKUPITEM,
	KNOCKDOWN,
};