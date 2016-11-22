using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NewFightButton : MonoBehaviour 
{
	[SerializeField] private Image onHover = null, regular = null;

	public void OnSelect()
	{
		onHover.CrossFadeAlpha (1.0f, 0.1f, true);
		regular.CrossFadeAlpha (0.0f, 0.1f, true);
	}

	public void OnDeselect()
	{
		regular.CrossFadeAlpha (1.0f, 0.1f, true);
		onHover.CrossFadeAlpha (0.0f, 0.1f, true);
	}
}
