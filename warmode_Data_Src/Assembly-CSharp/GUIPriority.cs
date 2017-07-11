using System;
using UnityEngine;

public class GUIPriority : MonoBehaviour
{
	private void OnGUI()
	{
		BlackScreen.Draw();
		Zombie.Draw();
		ChooseTeam.Draw();
		MenuBanList.Draw();
		Award.Draw();
		ScoreBoard.Draw();
		Message.Draw();
	}
}
