using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RefContainer : MonoBehaviour 
{
	[Header("MAIN MENU --------------------------------------------------------------------------------------------------------------------------")]
	public Image leftAButton = null;
	public Image rightAButton = null;
	public Text leftReadyQuestion = null, rightReadyQuestion = null, leftReady = null, rightReady = null;

	[Header("GAME MENU --------------------------------------------------------------------------------------------------------------------------")]
	public GameObject mainCamera = null;
	public Image countdownDigits = null;
	public Image countdownFight = null;
	public Sprite countdown1 = null, countdown2 = null, countdown3 = null;

}

