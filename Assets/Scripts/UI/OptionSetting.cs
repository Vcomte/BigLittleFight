using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OptionSetting : MonoBehaviour 
{
	[SerializeField] private Enums.OptionSetting optionSetting = Enums.OptionSetting.OS_NONE;
	[SerializeField] private Sprite optionBackgroundSelected  = null, optionbackgroundUnselected = null;
	[SerializeField] private Image optionBackground = null, barBackground = null, barFill = null;
	[SerializeField] private Color barBackgroundSelected = new Color (), barBackgroundUnselected = new Color ();
	[SerializeField] private Color barFillSelected = new Color (), barFillUnselected = new Color ();
	[SerializeField] private GameObject fillBar = null;
	[SerializeField] private float xPosMin = 0.0f, xPosMax = 0.0f;
	[SerializeField] private float minValue = 0.0f, maxValue = 0.0f;
	[SerializeField] private Text percentageValueText = null;
	[SerializeField] private Color emptyColor = new Color (), filledColor = new Color ();
	private bool isBarSelected = false;
	private float currentValue = 1.0f;

	private void Start()
	{
		Manager_Menu.event_onBarValueChanged += AdjustBarValue;
		optionBackground.sprite = optionbackgroundUnselected;
		barBackground.CrossFadeColor (barBackgroundUnselected, 0.0f, true, true);
		barFill.CrossFadeColor (barFillUnselected, 0.0f, true, true);
	}

	public void Selected(bool isSelected)
	{
		isBarSelected = isSelected;
		optionBackground.sprite = isSelected ? optionBackgroundSelected : optionbackgroundUnselected;
		barBackground.CrossFadeColor (isSelected ? barBackgroundSelected : barBackgroundUnselected, 0.15f, true, true);
		barFill.CrossFadeColor (isSelected ? barFillSelected : barFillUnselected, 0.15f, true, true);
	}

	public void AdjustBarValue(float increment)
	{
		if (!isBarSelected)
			return;

		currentValue += increment;
		if (currentValue > maxValue)
			currentValue = maxValue;
		else if (currentValue < minValue)
			currentValue = minValue;

		int percent = Mathf.RoundToInt (currentValue * 100 / maxValue);
		percentageValueText.text = percent.ToString () + "%";
		if (percent < 48)
			percentageValueText.color = emptyColor;
		else
			percentageValueText.color = filledColor;

		float xPos = xPosMin + (Mathf.Abs (xPosMin - xPosMax) * (currentValue / maxValue));
		fillBar.transform.localPosition = new Vector3 (xPos, 0.0f, 0.0f);

		switch (optionSetting)
		{
			case Enums.OptionSetting.OS_VOL_SFX:
			{
					Manager_Menu.Instance.AdjustVolumeSFX (currentValue);
				break;
			}
			case Enums.OptionSetting.OS_VOL_MUSIC:
			{
				Manager_Menu.Instance.AdjustVolumeMusic (currentValue);
				break;
			}
			case Enums.OptionSetting.OS_GRA_BRIGHTNESS:
			{
				Manager_Menu.Instance.AdjustGraphicsBrightness (currentValue);
				break;
			}
			case Enums.OptionSetting.OS_GRA_CONTRAST:
			{
				Manager_Menu.Instance.AdjustGraphicsContrast (currentValue);
				break;
			}
			
			default:
				break;
		}
	}

}
