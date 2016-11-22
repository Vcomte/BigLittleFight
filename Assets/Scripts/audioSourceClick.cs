using UnityEngine;
using System.Collections;

public class audioSourceClick : MonoBehaviour {

	// Use this for initialization
	void Start () {
        this.GetComponent<AudioSource>().mute = true;
        StartCoroutine(WaitForDemute());
	}
	
	private IEnumerator WaitForDemute()
    {
        yield return new WaitForSecondsRealtime(2);
        this.GetComponent<AudioSource>().mute = false;
    }
}
