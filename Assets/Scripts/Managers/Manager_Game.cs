using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;


public class Manager_Game : MonoBehaviour 
{
	#region Singleton
	public static Manager_Game Instance 
	{
		get 
		{
			if(instance == null)
				instance = FindObjectOfType(typeof(Manager_Game)) as Manager_Game;

			return instance;
		}
		set 
		{
			instance = value;
		}
	}
	private static Manager_Game instance;
	#endregion

	#region Serialized Vars

	[SerializeField] private Image p1PowerBar = null, p2PowerBar = null;
	[SerializeField] private float _PowerBarEmptyX = -133.0f, _PowerBarFillX = -2.0f;
	[SerializeField] private Image p1Go = null, p2Go = null, p1Victory = null, p2Victory = null;

	#endregion

	#region private vars

	private PlayerScript p1Ref = null, p2Ref = null;
	private bool isPaused = false;
	private bool gameHasStarted = false;

	#endregion

	#region public vars
	public Action<bool> onPause = null;

	#endregion

	#region UI Handling

	private void Start()
	{
		p1Go.CrossFadeAlpha (0.0f, 0.0f, true);
		p2Go.CrossFadeAlpha (0.0f, 0.0f, true);
        p1Victory.CrossFadeAlpha(0.0f, 0.0f, true);
        p2Victory.CrossFadeAlpha(0.0f, 0.0f, true);

        Invoke ("GetPlayerRefs", 0.1f);
	}

	private void GetPlayerRefs()
	{
		GameObject p1 = GameObject.FindGameObjectWithTag ("Player1");
		if(p1)
			p1Ref = p1.transform.parent.GetComponent<PlayerScript> ();

		GameObject p2 = GameObject.FindGameObjectWithTag ("Player2");
		if(p2)
			p2Ref = p2.transform.parent.GetComponent<PlayerScript> ();
	}

	private void Update()
	{
		if (p1Ref == null || p2Ref == null)
			GetPlayerRefs ();

		if (!gameHasStarted)
			return;

		if (Input.GetButtonDown ("P1StartButton") || Input.GetButtonDown ("P2StartButton"))
		{
			if (isPaused)
				UnpauseGame ();
			else
				PauseGame ();
		}
	}

	public void UpdatePowerBars(int playerNum, float currentValue, float maxValue)
	{
		if (playerNum == 1) 
		{
			float xPos = _PowerBarEmptyX + (Mathf.Abs (_PowerBarEmptyX - _PowerBarFillX) * (currentValue / maxValue));
			p1PowerBar.transform.localPosition = new Vector3 (xPos, 0.0f, 0.0f);
		}
		else 
		{
			float xPos = _PowerBarEmptyX + (Mathf.Abs (_PowerBarEmptyX - _PowerBarFillX) * (currentValue / maxValue));
			p2PowerBar.transform.localPosition = new Vector3 (xPos * -1.0f, 0.0f, 0.0f);	
		}
	}

	public void UpdateGoAdvantage(int playerNum)
	{
		if (playerNum == 1)
		{
			p1Go.CrossFadeAlpha (1.0f, 0.05f, true);
			p2Go.CrossFadeAlpha (0.0f, 0.05f, true);
		} else if (playerNum == 2)
		{
			p1Go.CrossFadeAlpha (0.0f, 0.05f, true);
			p2Go.CrossFadeAlpha (1.0f, 0.05f, true);
		} else
		{
			p1Go.CrossFadeAlpha (0.0f, 0.05f, true);
			p2Go.CrossFadeAlpha (0.0f, 0.05f, true);
		}
	}

    public void DisplayVictory(int playerNum)
    {
        if (playerNum == 1)
            p1Victory.CrossFadeAlpha(1.0f, 0.05f, true);
        else if (playerNum == 2)
            p2Victory.CrossFadeAlpha(1.0f, 0.05F, true);
        else
        {
            p1Victory.CrossFadeAlpha(0.0f, 0.05f, true);
            p2Victory.CrossFadeAlpha(0.0f, 0.05f, true);
        }
    }

	#endregion

	#region Game Handling
	public void PauseGame()
	{
		if (onPause != null)
		{
			isPaused = true;
			onPause (true);
		}
	}

	public void UnpauseGame()
	{
		if (onPause != null)
		{
			isPaused = false;
			onPause (false);
		}
	}

	public void StartGame()
	{
		gameHasStarted = true;
	}

	#endregion
}
