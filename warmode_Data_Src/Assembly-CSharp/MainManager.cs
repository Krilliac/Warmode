using System;
using UnityEngine;

public class MainManager : MonoBehaviour
{
	private GameObject goGUI;

	private void Awake()
	{
		RenderSettings.fog = false;
		switch (0)
		{
		case 0:
			GameData.gSteam = true;
			GameData.gVK = false;
			break;
		case 1:
			GameData.gSteam = false;
			GameData.gSocial = true;
			GameData.gVK = true;
			break;
		case 2:
			GameData.gSteam = false;
			GameData.gSocial = true;
			GameData.gFB = true;
			break;
		}
		MonoBehaviour.print("MainManager::Awake");
		Application.runInBackground = true;
		TEX.Init();
		GUIM.Init();
		BaseData.Init();
		Lang.Init();
		ContentLoader_.Init();
		MenuShop.Init();
		if (GameData.gVK)
		{
		}
		base.gameObject.GetComponent<UDPClient>().PostAwake();
		base.gameObject.GetComponent<WebHandler>().PostAwake();
		base.gameObject.GetComponent<BaseData>().PostAwake();
		this.goGUI = GameObject.Find("GUI");
		UIManager.PostAwake();
		UIManager.SetCanvasActive(true);
		UIManager.SetLoadingActive(true);
		this.goGUI.GetComponent<Main>().PostAwake();
		this.goGUI.GetComponent<TopBar>().PostAwake();
		this.goGUI.GetComponent<BottomBar>().PostAwake();
		this.goGUI.GetComponent<Profile>().PostAwake();
		this.goGUI.GetComponent<MenuShop>().PostAwake();
		this.goGUI.GetComponent<MenuPlayer>().PostAwake();
		this.goGUI.GetComponent<MenuGold>().PostAwake();
		this.goGUI.GetComponent<MenuServers>().PostAwake();
		this.goGUI.GetComponent<MenuOptions>().PostAwake();
		this.goGUI.GetComponent<MenuInventory>().PostAwake();
		this.goGUI.GetComponent<MainMenuConsole>().PostAwake();
		GameObject.Find("UI").GetComponent<UIManager>().DebugText.text = string.Empty;
		Debug.Log("StartAuth");
		bool flag = false;
		if (GameData.gSocial && !flag)
		{
			BaseData.StartAuth();
		}
		if (GameData.gVK && flag)
		{
			BaseData.uid = "7320897";
			BaseData.key = "3eeaed79cbe63bb1cb6d977753b1c048";
			BaseData.session = "_";
			WebHandler.get_profile("&version=" + Client.version);
		}
	}

	public void LoadEnd()
	{
		UIManager.SetLoadingActive(false);
	}
}
