using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	private GameObject goGUI;

	private GameObject goPlayer;

	private GameObject goCore;

	private void Awake()
	{
		Application.runInBackground = true;
		BlackScreen.SetActive(false);
		Zombie.SetInfectedScreen(false);
		GUI2.Init();
		TEX.Init();
		SND.Init();
		PlayerControll.Init();
		WeaponData.Init();
		BasePlayer.Init();
		EntControll.Init();
		this.goGUI = GameObject.Find("GUI");
		this.goGUI.GetComponent<global::Console>().PostAwake();
		this.goGUI.GetComponent<Crosshair>().PostAwake();
		this.goGUI.GetComponent<HUD>().PostAwake();
		this.goGUI.GetComponent<ChooseTeam>().PostAwake();
		this.goGUI.GetComponent<ScoreBoard>().PostAwake();
		this.goGUI.GetComponent<Chat>().PostAwake();
		this.goGUI.GetComponent<ScoreTop>().PostAwake();
		this.goGUI.GetComponent<Award>().PostAwake();
		this.goGUI.GetComponent<PlayerNames>().PostAwake();
		this.goGUI.GetComponent<Message>().PostAwake();
		this.goGUI.GetComponent<Indicator>().PostAwake();
		this.goGUI.GetComponent<BuyMenu>().PostAwake();
		this.goGUI.GetComponent<EscapeMenu>().PostAwake();
		this.goGUI.GetComponent<Vote>().PostAwake();
		this.goGUI.GetComponent<C4>().PostAwake();
		this.goGUI.GetComponent<MenuBanList>().PostAwake();
		this.goGUI.GetComponent<BlackScreen>().PostAwake();
		this.goGUI.GetComponent<Zombie>().PostAwake();
		this.goPlayer = GameObject.Find("LocalPlayer");
		this.goPlayer.GetComponent<HitSound>().PostAwake();
		this.goPlayer.GetComponent<HitEffect>().PostAwake();
		this.goCore = GameObject.Find("Core");
		this.goCore.GetComponent<UDPClient>().PostAwake();
		this.goCore.GetComponent<Client>().PostAwake();
		base.SendMessage("PostAwake");
		base.StartCoroutine("autostart");
		RenderSettings.fog = false;
	}

	[DebuggerHidden]
	private IEnumerator autostart()
	{
		return new GameManager.<autostart>c__IteratorC();
	}
}
