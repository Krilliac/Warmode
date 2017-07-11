using BestHTTP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class WebHandler : MonoBehaviour
{
	public class WebData
	{
		public string url;

		public int cmd;

		public string sCmd;

		public int status;

		public WWW www;

		public WebData(int cmd, string param = "", bool auth = false)
		{
			string text = WebHandler.handlerSteam;
			if (GameData.gVK)
			{
				text = WebHandler.handlerVK;
			}
			if (GameData.gFB)
			{
				text = WebHandler.handlerFB;
			}
			this.cmd = cmd;
			this.sCmd = cmd.ToString();
			if (auth)
			{
				this.url = string.Concat(new string[]
				{
					"http://",
					text,
					"&ucmd=",
					cmd.ToString(),
					"&uid=",
					BaseData.uid,
					"&key=",
					BaseData.key,
					"&time=",
					Time.time.ToString(),
					param
				});
			}
			else
			{
				this.url = string.Concat(new string[]
				{
					"http://",
					text,
					"&ucmd=",
					cmd.ToString(),
					"&uid=",
					BaseData.uid,
					"&wid=",
					BaseData.warid,
					"&session=",
					BaseData.warsession,
					"&time=",
					Time.time.ToString(),
					param
				});
			}
			this.status = 0;
			if (GameData.gSteam)
			{
				if (auth)
				{
					this.url = string.Concat(new string[]
					{
						"http://",
						text,
						"&ucmd=",
						cmd.ToString(),
						"&uid=",
						BaseData.uid,
						"&time=",
						Time.time.ToString(),
						param
					});
				}
				else
				{
					this.url = string.Concat(new string[]
					{
						"http://",
						text,
						"&ucmd=",
						cmd.ToString(),
						"&uid=",
						BaseData.uid,
						"&key=",
						BaseData.key,
						"&time=",
						Time.time.ToString(),
						param
					});
				}
			}
			MonoBehaviour.print(this.url);
			WebHandler.Lock = true;
		}

		~WebData()
		{
		}
	}

	public static WebHandler cs = null;

	public static bool BestHTTP = true;

	public static string handlerSteam = "95.213.132.75/core/handler.php?platform=steam";

	public static string handlerVK = "95.213.132.75/core/handler.php?platform=vk";

	public static string handlerFB = "95.213.132.75/core/handler.php?platform=fb";

	public static List<WebHandler.WebData> request = new List<WebHandler.WebData>();

	public static bool Lock = false;

	public void PostAwake()
	{
		WebHandler.cs = this;
		if (WebHandler.BestHTTP)
		{
			HTTPManager.MaxConnectionPerServer = 1;
			HTTPManager.ConnectTimeout = TimeSpan.FromSeconds(10.0);
			HTTPManager.RequestTimeout = TimeSpan.FromSeconds(20.0);
		}
	}

	private void Update()
	{
		if (WebHandler.BestHTTP)
		{
			if (WebHandler.request.Count == 0)
			{
				return;
			}
			for (int i = 0; i < WebHandler.request.Count; i++)
			{
				if (WebHandler.request[i].status == 0)
				{
					WebHandler.request[i].status = 1;
					new HTTPRequest(new Uri(WebHandler.request[i].url), new OnRequestFinishedDelegate(this.cbesthttp))
					{
						Tag = WebHandler.request[i]
					}.Send();
				}
			}
			return;
		}
		else
		{
			if (WebHandler.request.Count == 0)
			{
				return;
			}
			for (int j = 0; j < WebHandler.request.Count; j++)
			{
				if (WebHandler.request[j].status == 0)
				{
					base.StartCoroutine(this.chttp(WebHandler.request[j]));
				}
			}
			return;
		}
	}

	public static void get_profile(string p)
	{
		WebHandler.request.Add(new WebHandler.WebData(0, p, true));
	}

	public static void set_profile()
	{
		WebHandler.request.Add(new WebHandler.WebData(1, "&name=" + BaseData.Name, false));
	}

	public static void set_buy(string p)
	{
		WebHandler.request.Add(new WebHandler.WebData(2, p, false));
	}

	public static void get_top()
	{
		WebHandler.request.Add(new WebHandler.WebData(3, string.Empty, false));
	}

	public static void get_weaponcost()
	{
		WebHandler.request.Add(new WebHandler.WebData(5, string.Empty, false));
	}

	public static void get_options()
	{
		WebHandler.request.Add(new WebHandler.WebData(6, string.Empty, false));
	}

	public static void set_options(string p)
	{
		WebHandler.request.Add(new WebHandler.WebData(7, p, false));
	}

	public static void set_buy2(string p)
	{
		WebHandler.request.Add(new WebHandler.WebData(8, p, false));
	}

	public static void get_clanadmindata(string p)
	{
		WebHandler.request.Add(new WebHandler.WebData(9, p, false));
	}

	public static void get_clancreate(string p)
	{
		WebHandler.request.Add(new WebHandler.WebData(10, p, false));
	}

	public static void get_clandata()
	{
		WebHandler.request.Add(new WebHandler.WebData(11, string.Empty, false));
	}

	public static void get_clans(string p)
	{
		WebHandler.request.Add(new WebHandler.WebData(12, p, false));
	}

	public static void set_claninvite(string p)
	{
		WebHandler.request.Add(new WebHandler.WebData(13, p, false));
	}

	public static void get_clanrestore()
	{
		WebHandler.request.Add(new WebHandler.WebData(14, string.Empty, false));
	}

	public static void set_clanleave()
	{
		WebHandler.request.Add(new WebHandler.WebData(15, string.Empty, false));
	}

	public static void set_badge(string p)
	{
		WebHandler.request.Add(new WebHandler.WebData(16, p, false));
	}

	public static void get_warkey(string p)
	{
		WebHandler.request.Add(new WebHandler.WebData(100, p, true));
	}

	public static void get_buy(string p)
	{
		WebHandler.request.Add(new WebHandler.WebData(101, p, false));
	}

	public static void get_buyfin(string p)
	{
		WebHandler.request.Add(new WebHandler.WebData(102, p, false));
	}

	public static void get_inv()
	{
		WebHandler.request.Add(new WebHandler.WebData(103, string.Empty, false));
	}

	public static void get_facebook_key()
	{
		WebHandler.request.Add(new WebHandler.WebData(110, string.Empty, true));
	}

	public static void get_auth(string p)
	{
		WebHandler.request.Add(new WebHandler.WebData(200, p, true));
	}

	public static void get_servers()
	{
		WebHandler.request.Add(new WebHandler.WebData(201, string.Empty, false));
	}

	[DebuggerHidden]
	private IEnumerator chttp(WebHandler.WebData wb)
	{
		WebHandler.<chttp>c__Iterator15 <chttp>c__Iterator = new WebHandler.<chttp>c__Iterator15();
		<chttp>c__Iterator.wb = wb;
		<chttp>c__Iterator.<$>wb = wb;
		return <chttp>c__Iterator;
	}

	public void cbesthttp(HTTPRequest req, HTTPResponse resp)
	{
		WebHandler.WebData webData = req.Tag as WebHandler.WebData;
		switch (req.State)
		{
		case HTTPRequestStates.Finished:
			if (resp.IsSuccess)
			{
				string[] array = resp.DataAsText.Split(new char[]
				{
					'|'
				});
				int num;
				if (!int.TryParse(array[0], out num))
				{
					UnityEngine.Debug.LogWarning("WebHandler error: bad parse command " + array[0]);
					webData.status = 5;
				}
				else if (num != webData.cmd)
				{
					UnityEngine.Debug.LogWarning("WebHandler error: bad parse " + num.ToString());
					webData.status = 5;
				}
				else
				{
					WebHandler.parsdata(webData.cmd, array);
					webData.status = 2;
				}
			}
			else
			{
				UnityEngine.Debug.LogWarning(string.Format("Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}", resp.StatusCode, resp.Message, resp.DataAsText));
				webData.status = 3;
			}
			break;
		case HTTPRequestStates.Error:
			UnityEngine.Debug.LogError("Request Finished with Error! " + ((req.Exception == null) ? "No Exception" : (req.Exception.Message + "\n" + req.Exception.StackTrace)));
			webData.status = 3;
			break;
		case HTTPRequestStates.Aborted:
			UnityEngine.Debug.LogWarning("Request Aborted!");
			webData.status = 3;
			break;
		case HTTPRequestStates.ConnectionTimedOut:
			UnityEngine.Debug.LogError("Connection Timed Out!");
			webData.status = 3;
			break;
		case HTTPRequestStates.TimedOut:
			UnityEngine.Debug.LogError("Processing the request Timed Out!");
			webData.status = 3;
			break;
		}
	}

	private static void parsdata(int cmd, string[] param)
	{
		WebHandler.Lock = false;
		if (cmd == 0)
		{
			WebHandler.pars_get_profile(param);
		}
		else if (cmd == 1)
		{
			WebHandler.pars_set_profile(param);
		}
		else if (cmd == 2)
		{
			WebHandler.pars_set_buy(param);
		}
		else if (cmd == 3)
		{
			WebHandler.pars_get_top(param);
		}
		else if (cmd == 5)
		{
			WebHandler.pars_get_weaponcost(param);
		}
		else if (cmd == 6)
		{
			WebHandler.pars_get_options(param);
		}
		else if (cmd == 8)
		{
			WebHandler.pars_set_buy(param);
		}
		else if (cmd == 9)
		{
			WebHandler.pars_get_clanadmindata(param);
		}
		else if (cmd == 10)
		{
			WebHandler.pars_get_clancreate(param);
		}
		else if (cmd == 11)
		{
			WebHandler.pars_get_clandata(param);
		}
		else if (cmd == 12)
		{
			WebHandler.pars_get_clans(param);
		}
		else if (cmd == 13)
		{
			WebHandler.pars_set_claninvite(param);
		}
		else if (cmd == 14)
		{
			WebHandler.pars_get_clanrestore(param);
		}
		else if (cmd == 15)
		{
			WebHandler.pars_set_clanleave(param);
		}
		else if (cmd == 16)
		{
			WebHandler.pars_set_badge(param);
		}
		else if (cmd == 100)
		{
			WebHandler.pars_get_warkey(param);
		}
		else if (cmd == 102)
		{
			WebHandler.pars_get_buyfin(param);
		}
		else if (cmd == 103)
		{
			WebHandler.pars_get_inv(param);
		}
		else if (cmd == 110)
		{
			WebHandler.pars_get_facebook_key(param);
		}
		else if (cmd == 200)
		{
			WebHandler.pars_get_auth(param);
		}
		else if (cmd == 201)
		{
			WebHandler.pars_get_servers(param);
		}
	}

	private static void pars_get_profile(string[] param)
	{
		string str = "params: ";
		for (int i = 0; i < param.Length; i++)
		{
			str = str + param[i] + "|";
		}
		Main.inbuySocial = false;
		if (param.Length < 2)
		{
			return;
		}
		if (param[1] != "0")
		{
			return;
		}
		if (param[2] == "0")
		{
			BaseData.warid = param[3];
			BaseData.Name = param[4];
			BaseData.Gold = param[5];
			BaseData.GP = param[6];
			BaseData.EXP = param[7];
			BaseData.warsession = param[8];
			BaseData.ClanName = param[9];
			int.TryParse(BaseData.warid, out BaseData.iWarid);
			BaseData.Profile = true;
			int.TryParse(BaseData.Gold, out BaseData.iGold);
			int.TryParse(BaseData.GP, out BaseData.iGP);
			int.TryParse(BaseData.EXP, out BaseData.iEXP);
			BaseData.FullCalcLevel();
			if (!MenuShop.inbuy)
			{
				Main.MainInventory();
			}
			else
			{
				MenuShop.inbuy = false;
				MainMenuConsole.Command("reload_inv");
				Main.MainInventory();
				Main.HideAll();
				MenuInventory.SetActive(true);
			}
			WebHandler.get_servers();
			BaseData.Auth = true;
			MenuPlayer.SetActive(true);
			BaseData.FullCalcLevel();
			PlayerPrefs.SetInt(BaseData.uid + "_exp", BaseData.iEXP);
		}
		else if (!(param[2] == "1"))
		{
			if (param[2] == "2")
			{
				UnityEngine.Debug.Log("Client old version");
				Client.actualVersion = false;
			}
			else if (param[2] == "3")
			{
				UnityEngine.Debug.Log("Close application");
				BaseData.Auth = false;
			}
			else
			{
				UnityEngine.Debug.Log("Unban cost: " + param[2]);
				BaseData.Update = true;
				BaseData.banCost = param[2];
			}
		}
	}

	private static void pars_set_profile(string[] param)
	{
		int num = 0;
		int.TryParse(param[1], out num);
		BaseData.Name = param[2];
	}

	private static void pars_set_buy(string[] param)
	{
		int num = 0;
		int.TryParse(param[1], out num);
		if (num > 0)
		{
			MenuShop.inbuy = false;
			return;
		}
		WebHandler.get_profile("&version=" + Client.version);
	}

	private static void pars_get_top(string[] param)
	{
	}

	private static void pars_get_options(string[] param)
	{
	}

	private static void pars_get_weaponcost(string[] param)
	{
	}

	private static void pars_get_clancreate(string[] param)
	{
	}

	private static void pars_get_clanadmindata(string[] param)
	{
	}

	private static void pars_get_clandata(string[] param)
	{
	}

	private static void pars_get_clans(string[] param)
	{
	}

	private static void pars_set_claninvite(string[] param)
	{
	}

	private static void pars_get_clanrestore(string[] param)
	{
	}

	private static void pars_set_clanleave(string[] param)
	{
	}

	private static void pars_set_badge(string[] param)
	{
	}

	private static void pars_get_warkey(string[] param)
	{
		if (param.Length < 5)
		{
			return;
		}
		if (param[1] != "0")
		{
			return;
		}
		BaseData.key = param[2];
		if (!int.TryParse(param[3], out BaseData.iWarid))
		{
			return;
		}
		if (!int.TryParse(param[4], out BaseData.iEXP))
		{
			return;
		}
		BaseData.Auth = true;
		MenuPlayer.SetActive(true);
		PlayerPrefs.SetString(BaseData.uid + "_key", BaseData.key);
		PlayerPrefs.SetInt(BaseData.uid + "_wid", BaseData.iWarid);
		PlayerPrefs.SetInt(BaseData.uid + "_exp", BaseData.iEXP);
		BaseData.FullCalcLevel();
	}

	private static void pars_get_buyfin(string[] param)
	{
		if (param.Length < 2)
		{
			return;
		}
		if (param[1] != "0")
		{
			return;
		}
		if (param[2] == "1")
		{
			Main.MainAuthSteam();
		}
		else
		{
			MainMenuConsole.Command("reload_inv");
			Main.MainInventory();
			Main.HideAll();
			MenuInventory.SetActive(true);
		}
	}

	private static void pars_get_inv(string[] param)
	{
		if (param.Length < 4)
		{
			return;
		}
		BaseData.invsig = param[2];
		int num = 0;
		if (!int.TryParse(param[3], out num))
		{
			return;
		}
		if (num != param.Length - 5)
		{
			return;
		}
		for (int i = 4; i < param.Length - 1; i++)
		{
			int num2 = 0;
			if (!int.TryParse(param[i], out num2))
			{
				return;
			}
			BaseData.item[num2] = 1;
			PlayerPrefs.SetInt(BaseData.uid + "_item_" + num2.ToString(), 1);
		}
		PlayerPrefs.SetString(BaseData.uid + "_invsig", BaseData.invsig);
	}

	private static void pars_get_facebook_key(string[] param)
	{
		MonoBehaviour.print("pars_get_facebook_key");
		if (param.Length < 2)
		{
			return;
		}
		if (param[1] != "0")
		{
			return;
		}
		BaseData.key = param[2];
		UnityEngine.Debug.Log("key" + BaseData.key);
		BaseData.Profile = false;
		WebHandler.get_profile("&version=" + Client.version);
	}

	private static void pars_get_auth(string[] param)
	{
		if (param.Length < 2)
		{
			return;
		}
		if (param[1] != "0")
		{
			return;
		}
		if (param[2] == "0")
		{
			if (!int.TryParse(param[3], out BaseData.iEXP))
			{
				return;
			}
			BaseData.Auth = true;
			MenuPlayer.SetActive(true);
			BaseData.FullCalcLevel();
			PlayerPrefs.SetInt(BaseData.uid + "_exp", BaseData.iEXP);
		}
		else if (param[2] == "1")
		{
			string ticket = Steam.GetTicket();
			WebHandler.get_warkey("&ticket=" + ticket);
		}
		else if (param[2] == "2")
		{
			UnityEngine.Debug.Log("Client old version");
			Client.actualVersion = false;
		}
		else if (param[2] == "3")
		{
			UnityEngine.Debug.Log("Close application");
			Application.Quit();
		}
		else
		{
			BaseData.Update = true;
			BaseData.banCost = param[2];
		}
	}

	private static void pars_get_servers(string[] param)
	{
		if (param.Length < 1)
		{
			return;
		}
		MenuServers.server.Clear();
		for (int i = 1; i < param.Length; i++)
		{
			string[] array = param[i].Split(new char[]
			{
				':'
			});
			if (array.Length >= 3)
			{
				string ip = array[0];
				int port = 0;
				int channel = 0;
				if (int.TryParse(array[1], out port))
				{
					if (int.TryParse(array[2], out channel))
					{
						MenuServers.server.Add(new MenuServers.CServerData(ip, port, channel, 0, string.Empty));
					}
				}
			}
		}
	}
}
