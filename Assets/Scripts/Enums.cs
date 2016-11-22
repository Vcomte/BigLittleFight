using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class Enums
{
	public enum AttackType
	{
		ATTACK_HIGH = 0,
		ATTACK_LOW,
		ATTACK_NONE
	}

	public enum CharSize
	{
		SIZE_BIG = 0,
		SIZE_SMALL,
		SIZE_NONE
	}

	public enum MenuState
	{
		MENU_MAIN = 0,
		MENU_CONNECTION,
		MENU_GAME,
		MENU_NONE
	}

	public enum OptionSetting
	{
		OS_VOL_SFX = 0,
		OS_VOL_MUSIC,
		OS_GRA_BRIGHTNESS,
		OS_GRA_CONTRAST,
		OS_NONE
	}

	public enum QTEBtn
	{
		BTN_A = 0,
		BTN_B,
		BTN_Y,
		BTN_X,
		BTN_NONE
	}

	public static string[] GetQTEInputString(QTEBtn btn)
	{
		//List<string> list = new List<string> ();
		switch (btn)
		{
			case QTEBtn.BTN_A:
			{
				return new string[4] { "AButton", "BButton", "YButton", "XButton"};
			}
			case QTEBtn.BTN_B:
			{
				return new string[4] { "BButton", "AButton", "YButton", "XButton"};
			}
			case QTEBtn.BTN_Y:
			{
				return new string[4] { "YButton", "BButton", "AButton", "XButton"};
			}
			case QTEBtn.BTN_X:
			{
				return new string[4] { "XButton", "BButton", "YButton", "AButton"};
			}
			
			default:
				return null;
		}
	}
}
