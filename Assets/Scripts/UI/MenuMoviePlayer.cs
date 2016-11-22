using UnityEngine;
using System.Collections;

public class MenuMoviePlayer : MonoBehaviour
{
    [SerializeField]
    private bool loops = false;
    [SerializeField]
    private bool playsOnStart = false;

    // Use this for initialization
    private void Start()
    {
        if (playsOnStart)
            Play();

        ((MovieTexture)GetComponent<Renderer>().material.mainTexture).loop = loops;
    }

    public void Play()
    {
        ((MovieTexture)GetComponent<Renderer>().material.mainTexture).Play();
    }

}
