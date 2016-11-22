using UnityEngine;
using System.Collections;

public class QTECollider : MonoBehaviour 
{
	[SerializeField] private bool startQTE = false;

	public void OnTriggerEnter2D(Collider2D c)
	{
		if (startQTE)
		{
			if (c.CompareTag ("QTECollider"))
			{
				Manager_Game.Instance.StartQTE ();
			}
		}
	}

}
