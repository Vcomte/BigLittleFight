using UnityEngine;
using System.Collections;

public class GoZoneBehaviour : MonoBehaviour {

    [SerializeField] private string player = "";

    private bool _triggered = false;

	public bool triggered { get { return _triggered; } set { _triggered = value;} }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

    void OnTriggerEnter2D (Collider2D col)
    {
        if (col.gameObject.tag == player)
        {
            _triggered = true;
			this.gameObject.GetComponent<Collider2D> ().enabled = false;
        }
    }
}
