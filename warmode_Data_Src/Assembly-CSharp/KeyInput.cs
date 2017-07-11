using System;
using UnityEngine;

public class KeyInput : MonoBehaviour
{
	private Vector3 mouseclick;

	private float lastinput;

	private void Update()
	{
		if (Crosshair.forceLockCursor && !BuyMenu.show && !ChooseTeam.show && !EscapeMenu.show && !MenuBanList.show && !Vote.show)
		{
			Cursor.lockState = CursorLockMode.Locked;
		}
		if (Time.time < this.lastinput)
		{
			return;
		}
		if (Input.GetKeyUp(KeyCode.F10) || Input.GetKeyUp(KeyCode.BackQuote))
		{
			GameObject gameObject = GameObject.Find("GUI");
			gameObject.GetComponent<global::Console>().ToggleActive();
		}
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			if (BuyMenu.isActive())
			{
				BuyMenu.SetActive(false);
			}
			else
			{
				EscapeMenu.Toggle();
			}
		}
		if (Input.GetKeyUp(vp_FPInput.control[16]) && ScoreBoard.gamemode != 3 && (BasePlayer.deadflag == 0 || (BasePlayer.deadflag == 1 && BasePlayer.team == 255)))
		{
			ChooseTeam.Toggle();
		}
		if (Input.GetKey(vp_FPInput.control[17]))
		{
			ScoreBoard.SetActive(true);
		}
		else if (Input.GetKeyUp(vp_FPInput.control[17]))
		{
			ScoreBoard.SetActive(false);
		}
		if (Input.GetKeyUp(KeyCode.Return))
		{
			Chat.Toggle();
		}
		if (Input.GetKeyUp(vp_FPInput.control[18]))
		{
			Chat.ToggleTeam();
		}
		if (Input.GetKeyUp(vp_FPInput.control[19]))
		{
			if (BuyMenu.canbuy && BuyMenu.inbuyzone)
			{
				BuyMenu.Toggle();
			}
			else
			{
				BuyMenu.SetActive(false);
			}
		}
		if (Input.GetKeyUp(vp_FPInput.control[21]))
		{
			if (BuyMenu.canbuy && BuyMenu.inbuyzone)
			{
				BuyMenu.ToggleAmmunition();
			}
			else
			{
				BuyMenu.SetActive(false);
			}
		}
	}
}
