using UnityEngine;
using System.Collections;

public class QTESprite : MonoBehaviour {

    [SerializeField] private float timeToLive  = 1f;
    private float currentTime = 0f;


	// Use this for initialization
	void Start () {
        float rand = Random.Range(1.0f, 3.0f);
        this.transform.localScale = new Vector3(rand, rand, 1); 
    }
	
	// Update is called once per frame
	void Update () {
        Vector4 color = this.GetComponent<SpriteRenderer>().color;
        color.w = (1 - currentTime) / timeToLive;
        this.GetComponent<SpriteRenderer>().color = color;
        currentTime += Time.unscaledDeltaTime;
         if (currentTime > timeToLive)
            Destroy(this.gameObject);
    }
}
