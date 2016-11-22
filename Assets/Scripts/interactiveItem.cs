using UnityEngine;
using System.Collections;

public class interactiveItem : MonoBehaviour {

	public bool thrown = false;
	public bool available = false;
	private PlayerScript thrower;
	private bool canDamage = true;
	private bool isPaused = false;

    [SerializeField] private Sprite throwingVersion = null;
    private Vector4 initialColor = Vector4.zero;

    private Coroutine flashCoroutine = null;

	// Use this for initialization
	void Awake () 
	{
		Manager_Game.Instance.onPause += Handler_OnPause;
        initialColor = this.GetComponent<SpriteRenderer>().color;
	}

	// Update is called once per frame

	private void Handler_OnPause(bool pause)
	{
		isPaused = pause;
	}

	void Update () {
		//Remplacer l'input en saisie par l'input du joueur ayant réalisé la 1ere collision
		if (!thrower)
			return;

		if (isPaused)
			return;


		if (available && Input.GetButtonDown(thrower.playerID + "XButton"))
		{
			thrower.toThrow = this.gameObject;
			//thrower.arrow.GetComponentInChildren<SpriteRenderer>().enabled = true;

			available = false;
            if (throwingVersion != null)
            {
                this.GetComponent<SpriteRenderer>().sprite = throwingVersion;
                this.GetComponent<BoxCollider2D>().size = this.GetComponent<SpriteRenderer>().sprite.bounds.size;
                this.GetComponent<BoxCollider2D>().offset = new Vector2((this.GetComponent<SpriteRenderer>().sprite.bounds.size.x / 2), 0);
            }

            this.gameObject.transform.parent = thrower.gameObject.transform;
			int faceLeft = thrower.faceLeft ? -1 : 1;
			Vector3 newPos = thrower.gameObject.transform.position + faceLeft * new Vector3(this.GetComponent<Collider2D>().bounds.size.x / 2f + 
                thrower.gameObject.transform.GetChild(0).gameObject.GetComponent<Collider2D>().bounds.size.x / 2f + 0.1f, 0, 0);
			this.gameObject.transform.position = newPos;

			thrower.throwing = true;
		}
		if(!canDamage) //Means the object has already been thrown and is waiting for being destroyed by killSelf
		{
			StartCoroutine(flash());
		}
	}

	//Triggers are used before the object is thrown
	void OnTriggerEnter2D(Collider2D col){
		if (!thrown)
		{
			available = true;
            flashCoroutine = StartCoroutine(flash());
			thrower = col.gameObject.transform.parent.GetComponent<PlayerScript> ();
		}
	}

	void OnTriggerExit2D(Collider2D col){
		available = false;
        StopCoroutine(flashCoroutine);
        this.GetComponent<SpriteRenderer>().color = initialColor;
    }

    //Collisions are used when the object is thrown to determine damage
    void OnCollisionEnter2D(Collision2D col)
	{
		if (canDamage)
		{
			if (thrower.playerID.Equals("P1"))
			{
                if (col.gameObject.transform.GetChild(0).CompareTag("Player2"))
				{
                    col.gameObject.GetComponent<PlayerScript>().TakeDamage(20);
				}
			}
			else if (thrower.playerID.Equals("P2"))
			{
				if (col.gameObject.transform.GetChild(0).CompareTag("Player1"))
				{
					col.gameObject.GetComponent<PlayerScript>().TakeDamage(20);
				}
			}
			canDamage = false;
			StartCoroutine(killSelf());
		}
	}

    IEnumerator flash()
    {
        float alpha = this.GetComponent<SpriteRenderer>().color.a;
        Vector4 initialColor = this.GetComponent<SpriteRenderer>().color;
        int coeff = -1;
        bool getRed = true;
        while (true)
        {
            Vector4 color = this.GetComponent<SpriteRenderer>().color;
            if (!available)
            {
                alpha = alpha + Time.deltaTime * 3f * coeff;
                this.GetComponent<SpriteRenderer>().color = new Vector4(color.x, color.y, color.z, alpha);
                if (alpha >= 0.99f)
                    coeff = -1;
                else if (alpha <= 0.01f)
                    coeff = 1;
            }

            else
            {
                if ( !getRed)
                {
                    this.GetComponent<SpriteRenderer>().color = Vector4.Lerp(color, initialColor, 0.1f);
                    if (1 - color.y < 0.1f)
                        getRed = true;
                }
                else
                {
                    this.GetComponent<SpriteRenderer>().color = Vector4.Lerp(color, Color.red, 0.02f);
                    if (color.y < 0.6f)
                        getRed = false;
                }
            }
            yield return null;
        }
    }

	IEnumerator killSelf()
	{
		Destroy(this.gameObject, 2);
		yield return null;
	}
}

/*
IEnumerator flash()
	{
		float alpha = this.GetComponent<SpriteRenderer>().color.a;
        Vector4 initialColor = this.GetComponent<SpriteRenderer>().color;
        int coeff = -1;
		while (true)
		{
			alpha = alpha + Time.deltaTime * 3f * coeff;
			Vector4 color = this.GetComponent<SpriteRenderer>().color;
			this.GetComponent<SpriteRenderer>().color = new Vector4(color.x, color.y, color.z, alpha);
			if (alpha >= 0.99f)
				coeff = -1;
			else if (alpha <= 0.01f)
				coeff = 1;
			yield return null;
		}
	}
*/
