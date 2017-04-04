using UnityEngine;
using System.Collections;

public class GoZoneBehaviour : MonoBehaviour 
{

    [SerializeField] private string player = "";

    private bool _triggered = false;

	public bool triggered { get { return _triggered; } set { _triggered = value;} }

    void OnTriggerEnter2D (Collider2D col)
    {
        if (col.gameObject.tag == player)
        {
            _triggered = true;
        }
    }
}
