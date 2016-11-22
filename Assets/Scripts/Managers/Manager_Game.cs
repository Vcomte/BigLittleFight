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

	[SerializeField] private Image p1HealthBar = null, p2HealthBar = null, p1PowerBar = null, p2PowerBar = null;
	[SerializeField] private float _PowerBarEmptyX = -133.0f, _PowerBarFillX = -2.0f;
	[SerializeField] private Image p1Go = null, p2Go = null, p1Victory = null, p2Victory = null;

	[SerializeField] private Text qTEPressText = null;
	[SerializeField] private Image qTEHolder = null, qTEBtn = null;
	[SerializeField] private Image p1Qtebar = null, p1QTEBtn1 = null, p1QTEBtn2 = null, p1QTEBtn3 = null;
	[SerializeField] private Image p2Qtebar = null, p2QTEBtn1 = null, p2QTEBtn2 = null, p2QTEBtn3 = null;
	[SerializeField] private List<Sprite> qTEBtnSprites = new List<Sprite>();

	[SerializeField] private float qTEReactionInterval = 0.25f;

	#endregion

	#region private vars

	private Enums.QTEBtn currentQTEBtn = Enums.QTEBtn.BTN_NONE;
	private Coroutine qTECoroutine = null;
	private PlayerScript p1Ref = null, p2Ref = null;
	private bool isQTEAvailableForP1 = false;
	private bool isQTEAvailableForP2 = false;
	private bool isPaused = false;
	private bool gameHasStarted = false;

	#endregion

	#region public vars
	public bool isQTEOngoing = false;
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

		if (isQTEOngoing)
		{
			string[] list = Enums.GetQTEInputString (currentQTEBtn);

			if (isQTEAvailableForP1)
			{
				if (Input.GetButtonDown ("P1" + list [0]))
				{
					++p1Ref.qTECurrentPoints;
					if (p1Ref.qTECurrentPoints == 1)
					{
						p1QTEBtn1.sprite = qTEBtnSprites [(int)(currentQTEBtn)];
						p1QTEBtn1.enabled = true;
					} else if (p1Ref.qTECurrentPoints == 2)
					{
						p1QTEBtn2.sprite = qTEBtnSprites [(int)(currentQTEBtn)];
						p1QTEBtn2.enabled = true;
					} else if (p1Ref.qTECurrentPoints == 3)
					{
						p1QTEBtn3.sprite = qTEBtnSprites [(int)(currentQTEBtn)];
						p1QTEBtn3.enabled = true;
					}

					isQTEAvailableForP1 = false;
					isQTEAvailableForP2 = false;
				} else if (Input.GetButtonDown ("P1" + list [1]) ||
				        Input.GetButtonDown ("P1" + list [2]) ||
				        Input.GetButtonDown ("P1" + list [3]))
				{
					isQTEAvailableForP1 = false;
				}
			}

			if (isQTEAvailableForP2)
			{
				if (Input.GetButtonDown ("P2" + list [0]))
				{
					++p2Ref.qTECurrentPoints;
					if (p1Ref.qTECurrentPoints == 1)
					{
						p2QTEBtn1.sprite = qTEBtnSprites [(int)(currentQTEBtn)];
						p2QTEBtn1.enabled = true;
					} else if (p1Ref.qTECurrentPoints == 2)
					{
						p2QTEBtn2.sprite = qTEBtnSprites [(int)(currentQTEBtn)];
						p2QTEBtn2.enabled = true;
					} else if (p1Ref.qTECurrentPoints == 3)
					{
						p2QTEBtn3.sprite = qTEBtnSprites [(int)(currentQTEBtn)];
						p2QTEBtn3.enabled = true;
					}

					isQTEAvailableForP1 = false;
					isQTEAvailableForP2 = false;
				} else if (Input.GetButtonDown ("P2" + list [1]) ||
				        Input.GetButtonDown ("P2" + list [2]) ||
				        Input.GetButtonDown ("P2" + list [3]))
				{

					isQTEAvailableForP2 = false;
				}
			}
		}
	}

	public void UpdateHealthBars(int playerNum, float currentValue, float maxValue)
	{
		Image bar = (playerNum == 1 ? p1HealthBar : p2HealthBar);
		bar.fillAmount = currentValue / maxValue;
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

	public void StartQTE()
	{
		//TODO: Camera Zoom, visual effects and stuff
		isQTEOngoing = true;

		qTEHolder.enabled = true;
		p1Qtebar.enabled = true;
		p2Qtebar.enabled = true;
		qTEPressText.enabled = true;

		qTECoroutine = StartCoroutine (QTECoroutine ());

		qTEPressText.gameObject.GetComponent<SizeAnim> ().StartSizeCoroutine();
		PauseGame ();
	}

	public void EndQTE()
	{
		//TODO: Camera Dezoom, call player KnockBack

		isQTEOngoing = false;
		qTEPressText.enabled = false;
		qTEHolder.enabled = false;
		qTEBtn.enabled = false;
		p1Qtebar.enabled = false;
		p2Qtebar.enabled = false;

		p1QTEBtn1.enabled = false;
		p1QTEBtn2.enabled = false;
		p1QTEBtn3.enabled = false;
		p2QTEBtn1.enabled = false;
		p2QTEBtn2.enabled = false;
		p2QTEBtn3.enabled = false;

		if (p1Ref.qTECurrentPoints == 3)
		{
			p2Ref.TakeDamage (p2Ref.hpMax);
		}
		else if (p1Ref.qTECurrentPoints == 3)
		{
			p1Ref.TakeDamage (p1Ref.hpMax);
		}
		p1Ref.qTECurrentPoints = 0;
		p2Ref.qTECurrentPoints = 0;
		qTEPressText.gameObject.GetComponent<SizeAnim> ().StopSizeCoroutine();
		UnpauseGame ();
	}

	private IEnumerator QTECoroutine()
	{
		yield return StartCoroutine(Utils.WaitForRealSeconds(0.25f));

		qTEBtn.enabled = true;

		while(p1Ref.qTECurrentPoints < 3 && p2Ref.qTECurrentPoints < 3)		
		{
			//Make sure we never get the same button twice
			Enums.QTEBtn oldBtn = currentQTEBtn;
			do
			{
				currentQTEBtn = (Enums.QTEBtn)(UnityEngine.Random.Range (0, 4));
				yield return null;
			}
			while(currentQTEBtn.Equals (oldBtn));


			qTEBtn.sprite = qTEBtnSprites [(int)(currentQTEBtn)];

			isQTEAvailableForP1 = true;
			isQTEAvailableForP2 = true;

			yield return StartCoroutine(Utils.WaitForRealSeconds(qTEReactionInterval));
		}

		yield return StartCoroutine(Utils.WaitForRealSeconds(0.1f));

		EndQTE ();

		yield return null;
	}

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
