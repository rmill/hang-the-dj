using UnityEngine;
using System.Collections;

public class ThrowingKnife : MonoBehaviour
{

	public float speed;
	public int Damage = 10;
	public string sfx = "PunchHit";

	public void ThrowKnife(int direction)
	{
		transform.localScale = new Vector3(direction, 1, 1);
		StartCoroutine (startTravel(direction));
	}

	IEnumerator startTravel(int direction)
	{
		while (true) {
			transform.position += (Vector3.right * direction * speed) * Time.deltaTime;
			yield return null;
		}
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag ("Enemy")) {
			DamageObject d = new DamageObject(Damage, gameObject);
			d.attackType = AttackType.KnockDown;
			col.GetComponent<EnemyAI>().Hit(d);
			col.GetComponent<HealthSystem>().SubstractHealth(d.damage);
			GlobalAudioPlayer.PlaySFX(sfx);
			ShowHitEffect();
			Destroy(gameObject);
		}
	}

	void ShowHitEffect()
	{
		GameObject.Instantiate(Resources.Load("HitEffect"), transform.position, Quaternion.identity);
	}
}
