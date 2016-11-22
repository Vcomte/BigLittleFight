using UnityEngine;
using System.Collections;

public class HitTrigger : MonoBehaviour
{
	private PlayerScript opponentPlayerScript = null;
	private Collider2D myCollider = null;

	[SerializeField] private string opponentTag = "";
	[SerializeField] private int attackDamage = 10;
	[SerializeField] private Enums.AttackType attackType = Enums.AttackType.ATTACK_NONE;

	private void Start()
	{
		GameObject opponent = GameObject.FindGameObjectWithTag (opponentTag);
		if(opponent)
			opponentPlayerScript = opponent.transform.parent.GetComponent<PlayerScript> ();

		myCollider = transform.GetComponent<Collider2D> ();
	}

	public void Set(string newTag)
	{
		opponentTag = newTag;
	}

	public void Deactivate()
	{
		Destroy (gameObject);
	}

	public void OnTriggerEnter2D(Collider2D c)
	{
		if (c.CompareTag (opponentTag)) 
		{
			opponentPlayerScript.TakeHit (attackType, attackDamage);
		}
		if (c.CompareTag (opponentTag + "DashCollider"))
		{
			opponentPlayerScript.TakeHit ();
		}
	}
}
