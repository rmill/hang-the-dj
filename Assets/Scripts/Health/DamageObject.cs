using UnityEngine;
using System.Collections;

[System.Serializable]
public class DamageObject {

	public int damage;
	public float range;
	public AttackType attackType;
	public GameObject inflictor;
	public float comboResetTime = .5f;

	public DamageObject(int _damage, GameObject _inflictor){
		damage =  _damage;
		inflictor = _inflictor;
	}

	public DamageObject(int _damage, AttackType _attackType, GameObject _inflictor){
		damage =  _damage;
		attackType = _attackType;
		inflictor = _inflictor;
	}
}

public enum AttackType {
	Default = 0,
	SoftPunch = 10,
	MediumPunch = 20,
	KnockDown = 30,
	SoftKick = 40,
	HardKick = 50,
	SpecialMove = 60,
	DeathBlow = 70,
};
