using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class Client : MonoBehaviour
{
	public class RecvData
	{
		public byte[] Buffer;

		public int Len;

		public RecvData(byte[] _buffer, int _len)
		{
			this.Buffer = new byte[_len];
			for (int i = 0; i < _len; i++)
			{
				this.Buffer[i] = _buffer[i];
			}
			this.Len = _len;
		}

		~RecvData()
		{
			this.Buffer = null;
		}
	}

	public static Client cs;

	public static int ID = -1;

	public static bool Auth;

	public static int Rate = 30000;

	public static string Map = string.Empty;

	private static float COORDMAX = 1024f;

	private static float COORDRES = 32f;

	public static string IP = "95.213.132.75";

	public static int PORT = 5555;

	public static string PASSWORD = string.Empty;

	public static bool Loaded;

	public static byte version = 12;

	public static byte subversion;

	public static bool actualVersion = true;

	private bool Active;

	private TcpClient client;

	public byte[] sendbuffer;

	private static bool forcedisconnect;

	private static float pingTime;

	private List<Client.RecvData> Tlist = new List<Client.RecvData>();

	private byte[] readBuffer = new byte[102400];

	private int SplitRead;

	private int BytesRead;

	private int writepos;

	private byte[] readbuffer;

	private int readlen;

	private int readpos;

	private bool readerror;

	public static void Init()
	{
		Client.ID = -1;
		Client.Auth = false;
		Client.forcedisconnect = false;
	}

	public void PostAwake()
	{
		Client.cs = this;
		this.sendbuffer = new byte[2050];
		Client.Init();
		this.Active = false;
		List<Client.RecvData> tlist = this.Tlist;
		lock (tlist)
		{
			this.Tlist.Clear();
		}
	}

	public void Connect()
	{
		try
		{
			this.client = new TcpClient(Client.IP, Client.PORT);
			this.client.NoDelay = true;
			this.client.GetStream().BeginRead(this.readBuffer, 0, Client.Rate, new AsyncCallback(this.DoRead), null);
			Debug.Log("connected");
			this.Active = true;
		}
		catch
		{
			Debug.Log("Server is not active.");
			this.Active = false;
		}
		if (!this.Active)
		{
			PlayerPrefs.SetInt("connect_error", 1);
			global::Console.cs.Command("disconnect");
			return;
		}
		if (GameData.gSteam)
		{
			uint a = BitConverter.ToUInt32(IPAddress.Parse(Client.IP).GetAddressBytes(), 0);
			Steam.s_a(a, (uint)Client.PORT);
		}
		if (Client.PASSWORD.Length > 0)
		{
			this.send_begin();
		}
		this.send_auth();
	}

	private void Update()
	{
		List<Client.RecvData> tlist = this.Tlist;
		lock (tlist)
		{
			for (int i = 0; i < this.Tlist.Count; i++)
			{
				this.ProcessData(this.Tlist[i].Buffer, this.Tlist[i].Len);
			}
			this.Tlist.Clear();
		}
		if (Client.forcedisconnect)
		{
			Client.forcedisconnect = false;
			PlayerPrefs.SetInt("connect_error", 1);
			global::Console.cs.Command("disconnect");
		}
		if (GameData.gSteam)
		{
			Steam.s_b();
		}
	}

	private void OnApplicationQuit()
	{
		if (this.client != null)
		{
			this.client.Close();
		}
		Steam.s_c();
	}

	public void CloseClient()
	{
		if (this.client != null)
		{
			this.client.Close();
		}
	}

	private void DoRead(IAsyncResult ar)
	{
		try
		{
			this.BytesRead = this.client.GetStream().EndRead(ar);
			if (this.BytesRead >= 1)
			{
				this.SplitRead += this.BytesRead;
				while (this.SplitRead >= 4)
				{
					int num = this.DecodeShort(this.readBuffer, 2);
					if (this.SplitRead < num)
					{
						IL_E1:
						this.client.GetStream().BeginRead(this.readBuffer, this.SplitRead, Client.Rate, new AsyncCallback(this.DoRead), null);
						return;
					}
					List<Client.RecvData> tlist = this.Tlist;
					lock (tlist)
					{
						this.Tlist.Add(new Client.RecvData(this.readBuffer, num));
					}
					int num2 = 0;
					for (int i = num; i < this.SplitRead; i++)
					{
						this.readBuffer[num2] = this.readBuffer[i];
						num2++;
					}
					this.SplitRead -= num;
				}
				goto IL_E1;
			}
			Client.forcedisconnect = true;
		}
		catch (Exception var_4_115)
		{
		}
	}

	public void AddMsg(byte[] buffer, int len)
	{
		List<Client.RecvData> tlist = this.Tlist;
		lock (tlist)
		{
			this.Tlist.Add(new Client.RecvData(buffer, len));
		}
	}

	private void ProcessData(byte[] buffer, int len)
	{
		if (len < 2)
		{
			return;
		}
		if (buffer[0] != 245)
		{
			return;
		}
		byte b = buffer[1];
		switch (b)
		{
		case 0:
			this.recv_auth(buffer, len);
			return;
		case 1:
			this.recv_pos(buffer, len);
			return;
		case 2:
			this.recv_spawn(buffer, len);
			return;
		case 3:
		case 7:
		case 12:
		case 24:
		case 26:
		case 31:
		case 33:
		case 48:
		case 50:
		case 54:
			IL_108:
			if (b != 255)
			{
				return;
			}
			this.recv_ping(buffer, len);
			return;
		case 4:
			this.recv_connect(buffer, len);
			return;
		case 5:
			this.recv_disconnect(buffer, len);
			return;
		case 6:
			this.recv_playerdata(buffer, len);
			return;
		case 8:
			this.recv_chat(buffer, len);
			return;
		case 9:
			this.recv_takedamage(buffer, len);
			return;
		case 10:
			this.recv_playerstatus(buffer, len);
			return;
		case 11:
			this.recv_deathmsg(buffer, len);
			return;
		case 13:
			this.recv_gamerules(buffer, len);
			return;
		case 14:
			this.recv_currweapon(buffer, len);
			return;
		case 15:
			this.recv_attack(buffer, len);
			return;
		case 16:
			this.recv_buy(buffer, len);
			return;
		case 17:
			this.recv_gameend(buffer, len);
			return;
		case 18:
			this.recv_reward(buffer, len);
			return;
		case 19:
			this.recv_money(buffer, len);
			return;
		case 20:
			this.recv_respawnbar(buffer, len);
			return;
		case 21:
			this.recv_roundstart(buffer, len);
			return;
		case 22:
			this.recv_roundend(buffer, len);
			return;
		case 23:
			this.recv_message(buffer, len);
			return;
		case 25:
			this.recv_custompoints(buffer, len);
			return;
		case 27:
			this.recv_fog(buffer, len);
			return;
		case 28:
			this.recv_ent(buffer, len);
			return;
		case 29:
			this.recv_entdestroy(buffer, len);
			return;
		case 30:
			this.recv_version(buffer, len);
			return;
		case 32:
			this.recv_ammo(buffer, len);
			return;
		case 34:
			this.recv_changelevel(buffer, len);
			return;
		case 35:
			this.recv_votestart(buffer, len);
			return;
		case 36:
			this.recv_bombholder(buffer, len);
			return;
		case 37:
			this.recv_bombdrop(buffer, len);
			return;
		case 38:
			this.recv_bombplanting(buffer, len);
			return;
		case 39:
			this.recv_bombplanted(buffer, len);
			return;
		case 40:
			this.recv_bombdiffusing(buffer, len);
			return;
		case 41:
			this.recv_bombdiffused(buffer, len);
			return;
		case 42:
			this.recv_bombdetonated(buffer, len);
			return;
		case 43:
			this.recv_freezepos(buffer, len);
			return;
		case 44:
			this.recv_weapondrop(buffer, len);
			return;
		case 45:
			this.recv_weaponpos(buffer, len);
			return;
		case 46:
			this.recv_weaponpickup(buffer, len);
			return;
		case 47:
			this.recv_mapweaponpos(buffer, len);
			return;
		case 49:
			this.recv_grenadeanim(buffer, len);
			return;
		case 51:
			this.recv_makezombie(buffer, len);
			return;
		case 52:
			this.recv_zombieinfection(buffer, len);
			return;
		case 53:
			this.recv_zombieirepel(buffer, len);
			return;
		case 55:
			this.recv_playerinfo(buffer, len);
			return;
		case 56:
			this.recv_votestart(buffer, len);
			return;
		case 57:
			this.recv_vote(buffer, len);
			return;
		}
		goto IL_108;
	}

	private void recv_auth(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		Client.ID = this.READ_BYTE();
		int gamemode = this.READ_BYTE();
		Client.Map = this.READ_STRING();
		Client.Auth = true;
		MonoBehaviour.print("[NET] AUTH ID = " + Client.ID.ToString() + " MAP " + Client.Map);
		MapLoader.cs.Load(Client.Map);
		ScoreBoard.SetData(gamemode, Client.Map);
	}

	private void recv_pos(byte[] buffer, int len)
	{
		if (!Client.Loaded)
		{
			return;
		}
		this.BEGIN_READ(buffer, len, 4);
		int id = this.READ_BYTE();
		float pX = this.READ_COORD();
		float num = this.READ_COORD();
		float pZ = this.READ_COORD();
		float rX = this.READ_ANGLE();
		float rY = this.READ_ANGLE();
		int state = this.READ_BYTE();
		int zoom = this.READ_BYTE();
		float animSpeed = this.READ_FLOAT();
		float animStrafe = this.READ_FLOAT();
		PlayerControll.UpdatePosition(id, pX, num - 0.08f, pZ, rX, rY, state, zoom, animSpeed, animStrafe);
	}

	private void recv_spawn(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		int num = this.READ_BYTE();
		float x = this.READ_FLOAT();
		float y = this.READ_FLOAT();
		float z = this.READ_FLOAT();
		float angle = this.READ_FLOAT();
		int freezeTime = this.READ_BYTE();
		if (ScoreBoard.gamemode == 3 && PlayerControll.Player[num] != null)
		{
			PlayerControll.Player[num].Team = 1;
		}
		if (Client.ID == num)
		{
			GameData.SetFreezeTime(freezeTime);
			if (ScoreBoard.gamemode == 3)
			{
				BasePlayer.team = 1;
			}
			BasePlayer.Spawn(x, y, z, angle);
		}
		else
		{
			PlayerControll.Spawn(num, x, y, z);
		}
	}

	private void recv_connect(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		int id = this.READ_BYTE();
		PlayerControll.CreatePlayer(id, "Player", string.Empty, 255, 1, 0f, -512f, 0f, false);
	}

	private void recv_disconnect(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		int num = this.READ_BYTE();
		if (PlayerControll.Player[num] != null)
		{
			Message.AddSystem(PlayerControll.Player[num].Name + " disconnected\n");
		}
		PlayerControll.DestroyPlayer(num);
		ScoreTop.UpdateData();
	}

	private void recv_playerdata(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		int num = this.READ_BYTE();
		string name = this.READ_STRING();
		string clanname = this.READ_STRING();
		int num2 = this.READ_BYTE();
		int deadflag = this.READ_BYTE();
		int frags = this.READ_LONG();
		int deaths = this.READ_SHORT();
		int points = this.READ_SHORT();
		int level = this.READ_SHORT();
		int badge_back = this.READ_SHORT();
		int badge_icon = this.READ_SHORT();
		int mask_merc = this.READ_SHORT();
		int mask_warcorp = this.READ_SHORT();
		MonoBehaviour.print("recv_playerdata, id: " + num);
		if (PlayerControll.Player[num] != null && PlayerControll.Player[num].Team == 255 && num2 != 255)
		{
			Message.AddSystem(PlayerControll.Player[num].Name + " connected\n");
		}
		bool flag = false;
		if (ScoreBoard.gamemode == 3 && num2 == 0)
		{
			flag = true;
		}
		PlayerControll.CreatePlayer(num, name, clanname, num2, deadflag, 0f, -512f, 0f, flag);
		PlayerControll.SetPlayerStats(num, frags, deaths, points, level);
		PlayerControll.SetPlayerBadge(num, badge_back, badge_icon);
		if (!flag)
		{
			PlayerControll.SetPlayerMask(num, mask_merc, mask_warcorp);
		}
		PlayerControll.ResetCurrent(num);
	}

	private void recv_chat(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		int num = this.READ_BYTE();
		int flag = this.READ_BYTE();
		string msg = this.READ_STRING();
		if (PlayerControll.Player[num] == null)
		{
			return;
		}
		int num2 = PlayerControll.Player[num].Team;
		if (num2 > 1)
		{
			num2 = 2;
		}
		Message.AddChat(flag, PlayerControll.Player[num].Name, msg, num2);
	}

	private void recv_takedamage(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		int num = this.READ_BYTE();
		int num2 = this.READ_BYTE();
		int hitzone = this.READ_BYTE();
		int aid = this.READ_BYTE();
		if (num2 == 1)
		{
			if (Client.ID == num)
			{
				DeadCam.SetActive(true);
				HitSound.SetDeath();
				for (int i = 0; i < 128; i++)
				{
					BaseData.currentWeapon[i] = BaseData.profileWeapon[i];
				}
				BasePlayer.defuse = 0;
				HUD.SetDiffuseState(0);
			}
			else
			{
				PlayerControll.CreatePlayerRD(num, hitzone, aid, false);
			}
			PlayerControll.ResetCurrent(num);
		}
		if (PlayerControll.Player[num] == null)
		{
			return;
		}
		PlayerControll.Player[num].DeadFlag = num2;
		if (PlayerControll.Player[num].go)
		{
			UnityEngine.Object.Destroy(PlayerControll.Player[num].go);
		}
	}

	private void recv_playerstatus(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		int num = this.READ_BYTE();
		int num2 = this.READ_LONG();
		int num3 = this.READ_BYTE();
		int health = BasePlayer.health;
		BasePlayer.health = num2;
		HUD.sHealth = num2.ToString();
		HUD.sArmor = num3.ToString();
		if (num2 != 100 && num2 < health)
		{
			HitSound.SetHit();
			HitEffect.SetActive();
			if (num != Client.ID && PlayerControll.Player[num] != null)
			{
				Indicator.SetIndicator(PlayerControll.Player[num].position.x, PlayerControll.Player[num].position.y, PlayerControll.Player[num].position.z);
			}
			vp_FPController.LastDamageTime = Time.time;
		}
	}

	private void recv_deathmsg(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		int num = this.READ_BYTE();
		int num2 = this.READ_BYTE();
		int wid = this.READ_BYTE();
		int num3 = this.READ_BYTE();
		int streak = this.READ_BYTE();
		if (PlayerControll.Player[num] == null)
		{
			return;
		}
		if (PlayerControll.Player[num2] == null)
		{
			return;
		}
		int team = PlayerControll.Player[num].Team;
		int num4 = PlayerControll.Player[num2].Team;
		int status = 0;
		if (num == Client.ID)
		{
			status = 1;
		}
		else if (num2 == Client.ID)
		{
			status = 2;
		}
		if (ScoreBoard.gamemode == 3 && team == 0)
		{
			num4 = 1;
			wid = 31;
			num3 = 2;
		}
		Message.AddDeath(num, num2, PlayerControll.Player[num].Name, PlayerControll.Player[num2].Name, wid, team, num4, num3, streak, status);
		if (num == num2)
		{
			PlayerControll.Player[num2].SetDeathsInc(1);
		}
		else if (ScoreTop.FriendlyFire == 1 && team == num4)
		{
			PlayerControll.Player[num].SetFragsDec(1);
		}
		else
		{
			PlayerControll.Player[num].SetFragsInc(1);
			PlayerControll.Player[num2].SetDeathsInc(1);
			PlayerControll.Player[num].SetPointsInc(100);
			if (ScoreTop.GameMode == 0)
			{
				if (team == 0)
				{
					ScoreTop.SetScore0Inc(1);
				}
				else
				{
					ScoreTop.SetScore1Inc(1);
				}
			}
			else if (ScoreTop.GameMode == 1)
			{
				if (team == 0)
				{
				}
			}
			else if (ScoreTop.GameMode != 2 || team == 0)
			{
			}
		}
		if (num == Client.ID && num != num2)
		{
			if (ScoreTop.FriendlyFire != 1 || team != num4)
			{
				Award.SetPoints(100);
				Award.SetMoney(WeaponData.GetData(wid).fragMoney);
				Award.SetDeath(wid, num3);
			}
		}
	}

	private void recv_gamerules(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		int num = this.READ_BYTE();
		int fraglimit = this.READ_SHORT();
		int num2 = this.READ_SHORT();
		int num3 = this.READ_SHORT();
		int timeleft = this.READ_SHORT();
		string mapname = this.READ_STRING();
		int score = this.READ_SHORT();
		int score2 = this.READ_SHORT();
		int freezeTime = this.READ_SHORT();
		int friendlyfire = this.READ_SHORT();
		int restartRoundMode = this.READ_BYTE();
		int forcechasecam = this.READ_BYTE();
		GameData.SetRestartRoundMode(restartRoundMode);
		if (num == 0)
		{
			ScoreTop.SetScore0(num2);
			ScoreTop.SetScore1(num3);
		}
		else if (num == 1)
		{
			ScoreTop.SetScore0(score);
			ScoreTop.SetScore1(score2);
			ScoreBoard.SetRound0(num2);
			ScoreBoard.SetRound1(num3);
		}
		else if (num == 2)
		{
			ScoreTop.SetScore0(score);
			ScoreTop.SetScore1(score2);
			ScoreBoard.SetRound0(num2);
			ScoreBoard.SetRound1(num3);
		}
		else if (num == 3)
		{
			ScoreTop.SetScore0(score);
			ScoreTop.SetScore1(score2);
			ScoreBoard.SetRound0(num2);
			ScoreBoard.SetRound1(num3);
		}
		ScoreTop.SetData(num, fraglimit, timeleft, friendlyfire);
		ScoreBoard.SetData(num, mapname);
		GameData.SetFreezeTime(freezeTime);
		SpecCam.SetForcechasecam(forcechasecam);
	}

	private void recv_currweapon(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		int num = this.READ_BYTE();
		int num2 = this.READ_BYTE();
		int num3 = this.READ_LONG();
		if (SpecCam.show)
		{
			SpecCam.CurrWeapon(num, num2);
		}
		if (PlayerControll.Player[num] == null)
		{
			return;
		}
		PlayerControll.Player[num].currweapon = num2;
		if (WeaponData.CheckCustomSkin(num2))
		{
			PlayerControll.Player[num].currentWeapon[num2] = num3;
		}
		if (num2 == 27 || num2 == 29 || num2 == 30)
		{
			PlayerControll.Player[num].fastGrenade = num3;
		}
		if (PlayerControll.Player[num].go == null)
		{
			return;
		}
		if (!PlayerControll.Player[num].go.activeSelf)
		{
			return;
		}
		if (PlayerControll.Player[num].animator == null)
		{
			return;
		}
		PlayerControll.UpdateWeapon(num);
		PlayerControll.UpdateWeaponModel(num);
	}

	private void recv_attack(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		int id = this.READ_BYTE();
		PlayerControll.PlayFire(id);
	}

	private void recv_buy(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		int num = this.READ_BYTE();
		int money = this.READ_SHORT();
		HUD.SetMoney(money);
		if (WeaponData.CheckCustomSkin(num))
		{
			BaseData.currentWeapon[num] = BaseData.profileWeapon[num];
		}
		if (num == 0)
		{
			BasePlayer.currweapon = null;
			vp_FPWeaponHandler.cs.m_CurrentWeaponID = 0;
			BasePlayer.weapon[0] = null;
			BasePlayer.weapon[1] = null;
		}
		else if (num >= 1 && num <= 5)
		{
			BasePlayer.currweapon = null;
			vp_FPWeaponHandler.cs.m_CurrentWeaponID = 0;
			BasePlayer.weapon[1] = new CWeapon(WeaponData.GetData(num));
			BasePlayer.CalcAmmo();
			vp_FPInput.cs.Player.SetWeaponByName.Try(BasePlayer.weapon[1].data.selectName);
		}
		else if (num == 27)
		{
			HUD.SetFG(1);
			BasePlayer.fg = 1;
			BasePlayer.weapon[3] = new CWeapon(WeaponData.GetData(num));
			BasePlayer.selectedGrenade = 0;
		}
		else if (num == 29)
		{
			HUD.SetFB(1);
			BasePlayer.fb = 1;
			BasePlayer.weapon[5] = new CWeapon(WeaponData.GetData(num));
			BasePlayer.selectedGrenade = 1;
		}
		else if (num == 30)
		{
			HUD.SetSG(1);
			BasePlayer.sg = 1;
			BasePlayer.weapon[6] = new CWeapon(WeaponData.GetData(num));
			BasePlayer.selectedGrenade = 2;
		}
		else if (num == 49)
		{
			BasePlayer.defuse = 1;
			HUD.SetDiffuseState(1);
		}
		else if (num == 120)
		{
			HUD.SetArmor(100);
		}
		else if (num == 121)
		{
			HUD.SetArmor(100);
		}
		else
		{
			BasePlayer.currweapon = null;
			vp_FPWeaponHandler.cs.m_CurrentWeaponID = 0;
			BasePlayer.weapon[0] = new CWeapon(WeaponData.GetData(num));
			BasePlayer.CalcAmmo();
			vp_FPInput.cs.Player.SetWeaponByName.Try(BasePlayer.weapon[0].data.selectName);
		}
	}

	private void recv_gameend(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		int num = this.READ_BYTE();
		int winteam = this.READ_BYTE();
		if (num == 0)
		{
			ScoreBoard.SetActiveForce(false);
			PlayerControll.ClearScore();
			ScoreTop.SetScore0(0);
			ScoreTop.SetScore1(0);
			ScoreBoard.SetRound0(0);
			ScoreBoard.SetRound1(0);
			HUD.PlayStop();
		}
		else
		{
			ScoreBoard.SetActiveForce(true);
			ScoreBoard.winteam = winteam;
			Message.ResetMessage();
			HUD.PlayChampion();
		}
	}

	private void recv_reward(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		int num = this.READ_LONG();
		int num2 = this.READ_LONG();
		ScoreBoard.wingp = num;
		ScoreBoard.winexp = num2;
		BaseData.iGP += num;
		BaseData.GP = BaseData.iGP.ToString();
		BaseData.iEXP += num2;
		PlayerPrefs.SetInt(BaseData.uid + "_exp", BaseData.iEXP);
		BaseData.FullCalcLevel();
	}

	private void recv_money(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		int money = this.READ_SHORT();
		HUD.SetMoney(money);
	}

	private void recv_respawnbar(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		int respawnBar = this.READ_BYTE();
		HUD.SetRespawnBar(respawnBar);
	}

	private void recv_roundstart(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		int roundTimeLimit = this.READ_SHORT();
		int timeLeft = this.READ_SHORT();
		int num = this.READ_SHORT();
		int num2 = this.READ_SHORT();
		int restartRoundMode = this.READ_BYTE();
		GameData.SetRoundTimeLimit(roundTimeLimit);
		GameData.SetRoundTimeStart(Time.time);
		ScoreTop.SetTimeLeft(timeLeft);
		ScoreTop.SetScore0(num);
		ScoreTop.SetScore1(num2);
		ScoreBoard.SetRound0(num);
		ScoreBoard.SetRound1(num2);
		GameData.SetRestartRoundMode(restartRoundMode);
		GameData.infected = false;
		ScoreTop.SetBombIndicator(false);
		Message.HideBombPlantedMsg();
		C4.SetBombPlanted(false);
		C4.SetBombDiffused(false);
		C4.SetBombDropped(false);
		C4.DestroyBomb();
		Message.DeathClear();
	}

	private void recv_roundend(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		int num = this.READ_BYTE();
		int num2 = this.READ_BYTE();
		ScoreBoard.SetRound0(num);
		ScoreBoard.SetRound1(num2);
		ScoreTop.SetScore0(num);
		ScoreTop.SetScore1(num2);
		if (ScoreBoard.gamemode == 2)
		{
		}
		ScoreTop.SetBombIndicator(false);
	}

	private void recv_message(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		int message = this.READ_BYTE();
		Message.SetMessage(message);
	}

	private void recv_version(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		if (this.READ_BYTE() == 0)
		{
			Client.actualVersion = false;
		}
	}

	private void recv_custompoints(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		int num = this.READ_BYTE();
		int num2 = this.READ_BYTE();
		int cpid = this.READ_BYTE();
		if (PlayerControll.Player[num] != null)
		{
			PlayerControll.Player[num].SetPointsInc(num2);
		}
		if (Client.ID == num)
		{
			Award.SetCustomPoints(num2, cpid);
		}
	}

	private void recv_fog(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		int num = this.READ_BYTE();
		int num2 = this.READ_BYTE();
		int num3 = this.READ_BYTE();
		int num4 = this.READ_BYTE();
		float fogDensity = this.READ_FLOAT();
		int num5 = this.READ_BYTE();
		int num6 = this.READ_BYTE();
		RenderSettings.fog = true;
		RenderSettings.fogColor = new Color((float)num2, (float)num3, (float)num4, 1f);
		RenderSettings.fogDensity = fogDensity;
		RenderSettings.fogStartDistance = (float)num5;
		RenderSettings.fogEndDistance = (float)num6;
	}

	private void recv_ent(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		int ownerid = this.READ_BYTE();
		int classid = this.READ_BYTE();
		int uid = this.READ_LONG();
		int type = this.READ_BYTE();
		float x = this.READ_FLOAT();
		float y = this.READ_FLOAT();
		float z = this.READ_FLOAT();
		float x2 = this.READ_FLOAT();
		float y2 = this.READ_FLOAT();
		float z2 = this.READ_FLOAT();
		float x3 = this.READ_FLOAT();
		float y3 = this.READ_FLOAT();
		float z3 = this.READ_FLOAT();
		float x4 = this.READ_FLOAT();
		float y4 = this.READ_FLOAT();
		float z4 = this.READ_FLOAT();
		EntControll.CreateEnt(ownerid, classid, uid, type, new Vector3(x, y, z), new Vector3(x2, y2, z2), new Vector3(x3, y3, z3), new Vector3(x4, y4, z4));
	}

	private void recv_entdestroy(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		int num = this.READ_BYTE();
		int num2 = this.READ_BYTE();
		float x = this.READ_FLOAT();
		float y = this.READ_FLOAT();
		float z = this.READ_FLOAT();
		if (num == 1)
		{
			vp_FPWeapon.CreateGrenadeFlash(new Vector3(x, y, z));
		}
		else if (num == 2)
		{
			vp_FPWeapon.CreateGrenadeSmoke(new Vector3(x, y, z));
		}
	}

	private void recv_ammo(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		int num = this.READ_BYTE();
		int num2 = this.READ_LONG();
		int num3 = this.READ_LONG();
		int num4 = this.READ_LONG();
		int num5 = this.READ_LONG();
		HUD.SetFG(num2);
		HUD.SetFB(num3);
		HUD.SetSG(num4);
		HUD.SetDiffuseState(num5);
		BasePlayer.fg = num2;
		BasePlayer.fb = num3;
		BasePlayer.sg = num4;
		BasePlayer.defuse = num5;
	}

	private void recv_changelevel(byte[] buffer, int len)
	{
		PlayerPrefs.SetInt("reconnect", 1);
		Client.forcedisconnect = true;
	}

	private void recv_votestart(byte[] buffer, int len)
	{
		if (BasePlayer.team == 255)
		{
			return;
		}
		this.BEGIN_READ(buffer, len, 4);
		int num = this.READ_BYTE();
		int num2 = this.READ_BYTE();
		if (num2 == 255)
		{
			Vote_Dialog.cs.EndVote();
			return;
		}
		if (PlayerControll.Player[num] == null || PlayerControll.Player[num2] == null)
		{
			return;
		}
		Vote_Dialog.cs.StartVote(num, num2);
	}

	private void recv_vote(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		int answer = this.READ_BYTE();
		Vote_Dialog.cs.IncAnswer(answer);
	}

	private void recv_ping(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		float num = this.READ_FLOAT();
		float pingText = Time.time - num;
		global::Ping.cs.SetPingText(pingText);
	}

	private void recv_bombholder(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		int num = this.READ_BYTE();
		C4.DestroyBomb();
		C4.bombdropped = false;
		if (PlayerControll.Player[num] != null)
		{
			PlayerControll.Player[num].bomb = true;
			if (num != Client.ID)
			{
				PlayerControll.SetRootBombVisible(num, true);
			}
		}
		if (Client.ID == num)
		{
			BasePlayer.weapon[4] = new CWeapon(WeaponData.GetData(50));
			BasePlayer.bomb = true;
		}
		else
		{
			BasePlayer.weapon[4] = null;
			BasePlayer.bomb = false;
		}
	}

	private void recv_bombdrop(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		int id = this.READ_BYTE();
		C4.bombpos[0] = this.READ_FLOAT();
		C4.bombpos[1] = this.READ_FLOAT();
		C4.bombpos[2] = this.READ_FLOAT();
		C4.bombangle = this.READ_ANGLE();
		C4.OnRecvDropBomb(id);
	}

	private void recv_bombplanting(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		int num = this.READ_BYTE();
		int num2 = this.READ_BYTE();
		if (num2 == 0)
		{
			if (Client.ID == num || (SpecCam.show && SpecCam.FID == num && SpecCam.mode == 1))
			{
				vp_FPWeapon.SetBombAnimationState(1);
			}
			else
			{
				PlayerControll.SetBombSound(num, 0);
				PlayerControll.SetBombAnimation(num, 5);
			}
		}
		if (num2 == 1)
		{
			C4.canplant = true;
			C4.plantstarttime = Time.time;
			if (Client.ID == num || (SpecCam.show && SpecCam.FID == num && SpecCam.mode == 1))
			{
				vp_FPWeapon.SetBombAnimationState(3);
			}
			else
			{
				PlayerControll.SetBombSound(num, 1);
				PlayerControll.SetBombAnimation(num, 50);
			}
		}
	}

	private void recv_bombplanted(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		int num = this.READ_BYTE();
		C4.bombpos[0] = this.READ_FLOAT();
		C4.bombpos[1] = this.READ_FLOAT();
		C4.bombpos[2] = this.READ_FLOAT();
		C4.bombangle = this.READ_ANGLE();
		int timeLeft = this.READ_BYTE();
		if (Client.ID == num)
		{
			HUD.SetBombState(0);
			BasePlayer.currweapon = null;
			BasePlayer.weapon[4] = null;
			base.StartCoroutine(C4.KeyboardUnlockDelay());
		}
		else
		{
			PlayerControll.SetBombSound(num, 2);
			PlayerControll.SetBombAnimation(num, 5);
			PlayerControll.Player[num].bomb = false;
			PlayerControll.SetRootBombVisible(num, false);
		}
		C4.SetBombPlanted(true);
		ScoreTop.SetTimeLeft(timeLeft);
		ScoreTop.SetBombIndicator(true);
		Message.ShowBombPlantedMsg();
		C4.CreateBomb(new Vector3(C4.bombpos[0], C4.bombpos[1], C4.bombpos[2]), C4.bombangle, true);
		C4Place.PlayPlantSound(1);
		if (Client.ID == num)
		{
			vp_FPWeaponHandler.cs.m_CurrentWeaponID = 0;
			if (BasePlayer.weapon[0] != null)
			{
				vp_FPInput.cs.Player.SetWeaponByName.Try(BasePlayer.weapon[0].data.selectName);
			}
			else if (BasePlayer.weapon[1] != null)
			{
				vp_FPInput.cs.Player.SetWeaponByName.Try(BasePlayer.weapon[1].data.selectName);
			}
			else if (BasePlayer.weapon[2] != null)
			{
				vp_FPInput.cs.Player.SetWeaponByName.Try(BasePlayer.weapon[2].data.selectName);
			}
		}
	}

	private void recv_bombdiffusing(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		int num = this.READ_BYTE();
		int num2 = this.READ_BYTE();
		if (num2 == 0)
		{
			C4.SetShowDiffuseBar(false);
			if (Client.ID == num)
			{
				vp_FPInput.lockKeyboard = false;
			}
		}
		if (num2 == 1)
		{
			C4.diffusestarttime = Time.time;
			C4.SetDiffuserId(num);
			C4.SetShowDiffuseBar(true);
			if (Client.ID == num)
			{
				vp_FPInput.lockKeyboard = true;
				C4Place.PlayDiffuseSound(0);
			}
			else if (SpecCam.show && SpecCam.FID == num && SpecCam.mode == 1 && PlayerControll.Player[SpecCam.FID] != null)
			{
				C4Place.PlayDiffuseSound(0);
			}
		}
		if (num2 == 2)
		{
			if (Client.ID == num)
			{
				vp_FPInput.lockKeyboard = false;
				C4Place.PlayDiffuseSound(1);
			}
			else if (SpecCam.show && SpecCam.FID == num && SpecCam.mode == 1 && PlayerControll.Player[SpecCam.FID] != null)
			{
				C4Place.PlayDiffuseSound(1);
			}
			C4.SetBombDiffused(true);
			C4Place.Activate(false);
			C4.SetShowDiffuseBar(false);
		}
	}

	private void recv_bombdiffused(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		int num = this.READ_BYTE();
		vp_FPInput.lockKeyboard = false;
		C4.SetBombDiffused(true);
		C4Place.Activate(false);
	}

	private void recv_bombdetonated(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		int num = this.READ_BYTE();
		int num2 = this.READ_BYTE();
		if (num == 1)
		{
			C4.SetShowDiffuseBar(false);
			C4Place.Explosion();
		}
		if (num2 == Client.ID)
		{
			Award.SetPoints(300);
		}
		if (PlayerControll.Player[num2] != null)
		{
			PlayerControll.Player[num2].SetPointsInc(300);
		}
	}

	private void recv_freezepos(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		int num = this.READ_BYTE();
		int num2 = this.READ_BYTE();
		if (num != Client.ID)
		{
			return;
		}
		if (num2 == 0)
		{
			BasePlayer.FreezePosition(false);
		}
	}

	private void recv_weapondrop(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		int mode = this.READ_BYTE();
		int num = this.READ_BYTE();
		int num2 = this.READ_BYTE();
		int num3 = this.READ_BYTE();
		int customskin = this.READ_LONG();
		int num4 = this.READ_BYTE();
		float num5 = this.READ_FLOAT();
		float num6 = this.READ_FLOAT();
		float num7 = this.READ_FLOAT();
		float x = this.READ_FLOAT();
		float num8 = this.READ_FLOAT();
		float z = this.READ_FLOAT();
		float x2 = this.READ_FLOAT();
		float y = this.READ_FLOAT();
		float z2 = this.READ_FLOAT();
		float x3 = this.READ_FLOAT();
		float y2 = this.READ_FLOAT();
		float z3 = this.READ_FLOAT();
		if (Client.ID == num)
		{
			BasePlayer.lastdroppeduid = num2;
		}
		if (num3 == 50)
		{
			C4.bombpos[0] = num5;
			C4.bombpos[1] = num6;
			C4.bombpos[2] = num7;
			C4.bombangle = num8;
			if (PlayerControll.Player[num] != null)
			{
				PlayerControll.Player[num].bomb = false;
			}
			C4.bombdropped = true;
			if (num == Client.ID)
			{
				PlayerControll.Player[Client.ID].bomb = false;
				BasePlayer.bomb = false;
			}
			mode = 3;
		}
		vp_FPWeapon.CreateWeaponDrop(mode, num, num2, num3, customskin, (byte)num4, new Vector3(num5, num6, num7), new Vector3(x, num8, z), new Vector3(x2, y, z2), new Vector3(x3, y2, z3));
	}

	private void recv_weaponpos(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		int num = this.READ_BYTE();
		int num2 = this.READ_BYTE();
		float value = this.READ_FLOAT();
		float num3 = this.READ_FLOAT();
		float value2 = this.READ_FLOAT();
		float num4 = this.READ_FLOAT();
		float num5 = this.READ_FLOAT();
		float num6 = this.READ_FLOAT();
		if (vp_FPWeapon.WeaponOnMapArray[num2] == null)
		{
			return;
		}
		if (vp_FPWeapon.WeaponOnMapArray[num2].GetComponent<WeaponDrop>().wid == 50)
		{
			C4.bombpos[0] = value;
			C4.bombpos[1] = num3;
			C4.bombpos[2] = value2;
			C4.bombangle = num3;
		}
	}

	private void recv_weaponpickup(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		int num = this.READ_BYTE();
		int num2 = this.READ_BYTE();
		int num3 = this.READ_LONG();
		int clip = this.READ_BYTE();
		GameObject gameObject = vp_FPWeapon.WeaponOnMapArray[num2];
		if (gameObject == null)
		{
			return;
		}
		WeaponDrop component = gameObject.GetComponent<WeaponDrop>();
		int wid = component.wid;
		int slot = WeaponData.GetData(wid).slot;
		int customskin = component.customskin;
		if (wid != 49)
		{
			if (Client.ID == num)
			{
				if (WeaponData.CheckCustomSkin(wid))
				{
					BaseData.currentWeapon[wid] = customskin;
				}
				if (wid != 50)
				{
					BasePlayer.currweapon = null;
					vp_FPWeaponHandler.cs.m_CurrentWeaponID = 0;
				}
				BasePlayer.weapon[slot] = new CWeapon(WeaponData.GetData(wid));
				if (wid != 50)
				{
					int ammoType = BasePlayer.weapon[slot].data.ammoType;
					BasePlayer.ammo[ammoType] = num3;
					BasePlayer.sAmmo[ammoType] = BasePlayer.ammo[ammoType].ToString();
					BasePlayer.weapon[slot].clip = clip;
					BasePlayer.weapon[slot].sClip = clip.ToString();
				}
				if (wid != 50)
				{
					vp_FPCamera.lastWeapon = vp_FPCamera.currWeapon;
					vp_FPCamera.returnWeapon = null;
					vp_FPInput.cs.Player.SetWeaponByName.Try(BasePlayer.weapon[slot].data.selectName);
				}
			}
			else if (PlayerControll.Player[num] != null && WeaponData.CheckCustomSkin(wid))
			{
				PlayerControll.Player[num].currentWeapon[wid] = customskin;
			}
		}
		if (wid == 49 && Client.ID == num)
		{
			BasePlayer.defuse = 1;
			HUD.SetDiffuseState(1);
		}
		if (wid == 50)
		{
			C4.bombdropped = false;
			if (PlayerControll.Player[num] != null)
			{
				PlayerControll.Player[num].bomb = true;
				if (num != Client.ID)
				{
					PlayerControll.SetRootBombVisible(num, true);
				}
			}
			if (Client.ID == num)
			{
				BasePlayer.bomb = true;
			}
			else
			{
				BasePlayer.bomb = false;
			}
		}
		vp_FPWeapon.RemoveMapWeapon(num2);
	}

	private void recv_mapweaponpos(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		int id = this.READ_BYTE();
		int num = this.READ_BYTE();
		int wid = this.READ_BYTE();
		int customskin = this.READ_LONG();
		MonoBehaviour.print("recv_weaponpos " + num);
		float num2 = this.READ_FLOAT();
		float num3 = this.READ_FLOAT();
		float num4 = this.READ_FLOAT();
		float x = this.READ_FLOAT();
		float y = this.READ_FLOAT();
		float z = this.READ_FLOAT();
		if (vp_FPWeapon.WeaponOnMapArray[num] == null)
		{
			return;
		}
		if (vp_FPWeapon.WeaponOnMapArray[num].GetComponent<WeaponDrop>().wid == 50)
		{
			C4.bombpos[0] = num2;
			C4.bombpos[1] = num3;
			C4.bombpos[2] = num4;
			C4.bombangle = num3;
		}
		vp_FPWeapon.CreateMapWeaponDrop(id, num, wid, customskin, new Vector3(num2, num3, num4), new Vector3(x, y, z));
	}

	private void recv_grenadeanim(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		int num = this.READ_BYTE();
		int num2 = this.READ_BYTE();
		if (Client.ID != num)
		{
			if (num2 == 1)
			{
				PlayerControll.SetGrenadeAnimation(num, 9);
			}
			else if (num2 == 2)
			{
				PlayerControll.SetGrenadeAnimation(num, 10);
			}
		}
	}

	private void recv_makezombie(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		int num = this.READ_BYTE();
		int num2 = this.READ_BYTE();
		if (PlayerControll.Player[num] == null)
		{
			return;
		}
		PlayerControll.Player[num].Team = 0;
		PlayerControll.Player[num].zombieAlfa = num2;
		if (Client.ID == num)
		{
			BasePlayer.team = 0;
			BasePlayer.currweapon = null;
			vp_FPWeaponHandler.cs.m_CurrentWeaponID = 0;
			BasePlayer.weapon[0] = null;
			BasePlayer.weapon[1] = null;
			BasePlayer.weapon[2] = null;
			BasePlayer.weapon[3] = null;
			BasePlayer.weapon[4] = null;
			BasePlayer.weapon[5] = null;
			BasePlayer.weapon[6] = null;
			vp_FPInput.cs.Player.SetWeaponByName.Try(BasePlayer.weapon[7].data.selectName);
			BasePlayer.fg = 0;
			BasePlayer.fb = 0;
			BasePlayer.sg = 0;
			BasePlayer.defuse = 0;
			Zombie.SetInfectedScreen(true);
			HUD.cs.OnResize();
			HUD.SetMoney(0);
			if (num2 == 1)
			{
				BasePlayer.health = 9000;
				HUD.sHealth = "9000";
			}
			else
			{
				BasePlayer.health = 5000;
				HUD.sHealth = "5000";
			}
			HUD.sArmor = "0";
		}
		else
		{
			PlayerControll.MakeZombie(num);
		}
		ScoreTop.UpdateData();
	}

	private void recv_zombieinfection(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		int num = this.READ_BYTE();
		GameData.infected = true;
	}

	private void recv_zombieirepel(byte[] buffer, int len)
	{
		this.BEGIN_READ(buffer, len, 4);
		float x = this.READ_COORD();
		float y = this.READ_COORD();
		float z = this.READ_COORD();
		float d = this.READ_FLOAT();
		vp_FPController arg_46_0 = vp_FPController.cs;
		Vector3 vector = new Vector3(x, y, z);
		arg_46_0.AddSoftForce(vector.normalized * d, 1f);
	}

	private void recv_playerinfo(byte[] buffer, int len)
	{
	}

	public void send_auth()
	{
		if (this.client == null)
		{
			MonoBehaviour.print("client not connected");
			return;
		}
		NetworkStream stream = this.client.GetStream();
		this.BEGIN_WRITE();
		this.WRITE_BYTE(245);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(Client.version);
		this.WRITE_BYTE(Client.subversion);
		this.WRITE_LONG(BaseData.iWarid);
		this.WRITE_STRING(BaseData.warsession);
		this.WRITE_STRING(BaseData.Name);
		this.WRITE_STRING(BaseData.ClanName);
		this.WRITE_STRING(BaseData.uid);
		this.WRITE_STRING(BaseData.key);
		this.WRITE_STRING(BaseData.invsig);
		this.WRITE_BYTE(0);
		this.WRITE_LONG(BaseData.iLevel);
		this.WRITE_LONG(BaseData.badge_back);
		this.WRITE_LONG(BaseData.badge_icon);
		this.WRITE_LONG(BaseData.mask_merc);
		this.WRITE_LONG(BaseData.mask_warcorp);
		this.WRITE_LONG(BaseData.profileWeapon[WeaponData.GetId("ak47")]);
		this.WRITE_LONG(BaseData.profileWeapon[WeaponData.GetId("aks74u")]);
		this.WRITE_LONG(BaseData.profileWeapon[WeaponData.GetId("asval")]);
		this.WRITE_LONG(BaseData.profileWeapon[WeaponData.GetId("aug")]);
		this.WRITE_LONG(BaseData.profileWeapon[WeaponData.GetId("awp")]);
		this.WRITE_LONG(BaseData.profileWeapon[WeaponData.GetId("beretta")]);
		this.WRITE_LONG(BaseData.profileWeapon[WeaponData.GetId("bm4")]);
		this.WRITE_LONG(BaseData.profileWeapon[WeaponData.GetId("colt")]);
		this.WRITE_LONG(BaseData.profileWeapon[WeaponData.GetId("deagle")]);
		this.WRITE_LONG(BaseData.profileWeapon[WeaponData.GetId("famas")]);
		this.WRITE_LONG(BaseData.profileWeapon[WeaponData.GetId("glock17")]);
		this.WRITE_LONG(BaseData.profileWeapon[WeaponData.GetId("m4a1")]);
		this.WRITE_LONG(BaseData.profileWeapon[WeaponData.GetId("m24")]);
		this.WRITE_LONG(BaseData.profileWeapon[WeaponData.GetId("m90")]);
		this.WRITE_LONG(BaseData.profileWeapon[WeaponData.GetId("m110")]);
		this.WRITE_LONG(BaseData.profileWeapon[WeaponData.GetId("m249")]);
		this.WRITE_LONG(BaseData.profileWeapon[WeaponData.GetId("mp5")]);
		this.WRITE_LONG(BaseData.profileWeapon[WeaponData.GetId("mp7")]);
		this.WRITE_LONG(BaseData.profileWeapon[WeaponData.GetId("p90")]);
		this.WRITE_LONG(BaseData.profileWeapon[WeaponData.GetId("pkp")]);
		this.WRITE_LONG(BaseData.profileWeapon[WeaponData.GetId("qbz95")]);
		this.WRITE_LONG(BaseData.profileWeapon[WeaponData.GetId("remington")]);
		this.WRITE_LONG(BaseData.profileWeapon[WeaponData.GetId("spas12")]);
		this.WRITE_LONG(BaseData.profileWeapon[WeaponData.GetId("svd")]);
		this.WRITE_LONG(BaseData.profileWeapon[WeaponData.GetId("ump45")]);
		for (int i = 0; i < 1024; i++)
		{
			this.WRITE_BYTE((byte)BaseData.item[i]);
		}
		this.END_WRITE();
		stream.Write(this.sendbuffer, 0, this.WRITE_LEN());
	}

	public void send_pos(float pos_x, float pos_y, float pos_z, float rot_x, float rot_y, int state, int zoom, float speed, float strafe)
	{
		if (this.client == null)
		{
			return;
		}
		if (!this.client.Connected)
		{
			return;
		}
		NetworkStream stream = this.client.GetStream();
		this.BEGIN_WRITE();
		this.WRITE_BYTE(245);
		this.WRITE_BYTE(1);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(Client.subversion);
		this.WRITE_FLOAT(pos_x);
		this.WRITE_FLOAT(pos_y);
		this.WRITE_FLOAT(pos_z);
		this.WRITE_ANGLE(rot_x);
		this.WRITE_ANGLE(rot_y);
		this.WRITE_BYTE((byte)state);
		this.WRITE_BYTE((byte)zoom);
		this.WRITE_FLOAT(speed);
		this.WRITE_FLOAT(strafe);
		this.END_WRITE();
		stream.Write(this.sendbuffer, 0, this.WRITE_LEN());
	}

	public void send_clientloaded()
	{
		if (this.client == null)
		{
			return;
		}
		if (!this.client.Connected)
		{
			return;
		}
		NetworkStream stream = this.client.GetStream();
		this.BEGIN_WRITE();
		this.WRITE_BYTE(245);
		this.WRITE_BYTE(3);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(0);
		this.END_WRITE();
		stream.Write(this.sendbuffer, 0, this.WRITE_LEN());
	}

	public void send_chooseteam(int team)
	{
		if (this.client == null)
		{
			return;
		}
		if (!this.client.Connected)
		{
			return;
		}
		NetworkStream stream = this.client.GetStream();
		this.BEGIN_WRITE();
		this.WRITE_BYTE(245);
		this.WRITE_BYTE(7);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE((byte)team);
		this.END_WRITE();
		stream.Write(this.sendbuffer, 0, this.WRITE_LEN());
	}

	public void send_chat(int flag, string message)
	{
		if (this.client == null)
		{
			return;
		}
		if (!this.client.Connected)
		{
			return;
		}
		NetworkStream stream = this.client.GetStream();
		this.BEGIN_WRITE();
		this.WRITE_BYTE(245);
		this.WRITE_BYTE(8);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE((byte)flag);
		this.WRITE_STRING(message);
		this.END_WRITE();
		stream.Write(this.sendbuffer, 0, this.WRITE_LEN());
	}

	public void send_takedamage(byte vid, byte hitzone, byte clip, float x0, float y0, float z0, float x1, float y1, float z1)
	{
		if (this.client == null)
		{
			return;
		}
		if (!this.client.Connected)
		{
			return;
		}
		NetworkStream stream = this.client.GetStream();
		this.BEGIN_WRITE();
		this.WRITE_BYTE(245);
		this.WRITE_BYTE(9);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(vid);
		this.WRITE_BYTE(hitzone);
		this.WRITE_BYTE(clip);
		this.WRITE_FLOAT(x0);
		this.WRITE_FLOAT(y0);
		this.WRITE_FLOAT(z0);
		this.WRITE_FLOAT(x1);
		this.WRITE_FLOAT(y1);
		this.WRITE_FLOAT(z1);
		this.END_WRITE();
		stream.Write(this.sendbuffer, 0, this.WRITE_LEN());
	}

	public void send_consolecmd(string cmd, string param)
	{
		if (this.client == null)
		{
			return;
		}
		if (!this.client.Connected)
		{
			return;
		}
		NetworkStream stream = this.client.GetStream();
		this.BEGIN_WRITE();
		this.WRITE_BYTE(245);
		this.WRITE_BYTE(12);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(0);
		this.WRITE_STRING(cmd);
		this.WRITE_STRING(param);
		this.END_WRITE();
		stream.Write(this.sendbuffer, 0, this.WRITE_LEN());
	}

	public void send_currweapon(int wid, int state)
	{
		if (this.client == null)
		{
			return;
		}
		if (!this.client.Connected)
		{
			return;
		}
		NetworkStream stream = this.client.GetStream();
		this.BEGIN_WRITE();
		this.WRITE_BYTE(245);
		this.WRITE_BYTE(14);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE((byte)wid);
		this.WRITE_BYTE((byte)state);
		this.END_WRITE();
		stream.Write(this.sendbuffer, 0, this.WRITE_LEN());
	}

	public void send_attack()
	{
		if (this.client == null)
		{
			return;
		}
		if (!this.client.Connected)
		{
			return;
		}
		NetworkStream stream = this.client.GetStream();
		this.BEGIN_WRITE();
		this.WRITE_BYTE(245);
		this.WRITE_BYTE(15);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(Client.subversion);
		this.END_WRITE();
		stream.Write(this.sendbuffer, 0, this.WRITE_LEN());
	}

	public void send_buy(int wid)
	{
		if (this.client == null)
		{
			return;
		}
		if (!this.client.Connected)
		{
			return;
		}
		NetworkStream stream = this.client.GetStream();
		this.BEGIN_WRITE();
		this.WRITE_BYTE(245);
		this.WRITE_BYTE(16);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE((byte)wid);
		this.END_WRITE();
		stream.Write(this.sendbuffer, 0, this.WRITE_LEN());
	}

	public void send_update()
	{
		if (this.client == null)
		{
			return;
		}
		if (!this.client.Connected)
		{
			return;
		}
		NetworkStream stream = this.client.GetStream();
		this.BEGIN_WRITE();
		this.WRITE_BYTE(245);
		this.WRITE_BYTE(24);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(200);
		this.END_WRITE();
		stream.Write(this.sendbuffer, 0, this.WRITE_LEN());
	}

	public void send_begin()
	{
		if (this.client == null)
		{
			return;
		}
		if (!this.client.Connected)
		{
			return;
		}
		NetworkStream stream = this.client.GetStream();
		this.BEGIN_WRITE();
		this.WRITE_BYTE(245);
		this.WRITE_BYTE(26);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(0);
		this.WRITE_STRING(Client.PASSWORD);
		this.END_WRITE();
		stream.Write(this.sendbuffer, 0, this.WRITE_LEN());
	}

	public void send_ent(int classid, int type, Vector3 pos, Vector3 rot, Vector3 force, Vector3 torque)
	{
		if (this.client == null)
		{
			return;
		}
		if (!this.client.Connected)
		{
			return;
		}
		NetworkStream stream = this.client.GetStream();
		this.BEGIN_WRITE();
		this.WRITE_BYTE(245);
		this.WRITE_BYTE(28);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE((byte)classid);
		this.WRITE_BYTE((byte)type);
		this.WRITE_FLOAT(pos.x);
		this.WRITE_FLOAT(pos.y);
		this.WRITE_FLOAT(pos.z);
		this.WRITE_FLOAT(rot.x);
		this.WRITE_FLOAT(rot.y);
		this.WRITE_FLOAT(rot.z);
		this.WRITE_FLOAT(force.x);
		this.WRITE_FLOAT(force.y);
		this.WRITE_FLOAT(force.z);
		this.WRITE_FLOAT(torque.x);
		this.WRITE_FLOAT(torque.y);
		this.WRITE_FLOAT(torque.z);
		this.END_WRITE();
		stream.Write(this.sendbuffer, 0, this.WRITE_LEN());
	}

	public void send_ent_destroy(int uid, int type, Vector3 pos)
	{
		if (this.client == null)
		{
			return;
		}
		if (!this.client.Connected)
		{
			return;
		}
		NetworkStream stream = this.client.GetStream();
		this.BEGIN_WRITE();
		this.WRITE_BYTE(245);
		this.WRITE_BYTE(29);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(0);
		this.WRITE_LONG(uid);
		this.WRITE_BYTE((byte)type);
		this.WRITE_FLOAT(pos.x);
		this.WRITE_FLOAT(pos.y);
		this.WRITE_FLOAT(pos.z);
		this.END_WRITE();
		stream.Write(this.sendbuffer, 0, this.WRITE_LEN());
	}

	public void send_fall(float flytime)
	{
		if (this.client == null)
		{
			return;
		}
		if (!this.client.Connected)
		{
			return;
		}
		NetworkStream stream = this.client.GetStream();
		this.BEGIN_WRITE();
		this.WRITE_BYTE(245);
		this.WRITE_BYTE(33);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(0);
		this.WRITE_FLOAT(flytime);
		this.END_WRITE();
		stream.Write(this.sendbuffer, 0, this.WRITE_LEN());
	}

	public void send_bomb_holder()
	{
		if (this.client == null)
		{
			return;
		}
		if (!this.client.Connected)
		{
			return;
		}
		NetworkStream stream = this.client.GetStream();
		this.BEGIN_WRITE();
		this.WRITE_BYTE(245);
		this.WRITE_BYTE(36);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(0);
		this.END_WRITE();
		stream.Write(this.sendbuffer, 0, this.WRITE_LEN());
	}

	public void send_bomb_drop(float pos_x, float pos_y, float pos_z, float rot_y)
	{
		if (this.client == null)
		{
			return;
		}
		if (!this.client.Connected)
		{
			return;
		}
		NetworkStream stream = this.client.GetStream();
		this.BEGIN_WRITE();
		this.WRITE_BYTE(245);
		this.WRITE_BYTE(37);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(1);
		this.END_WRITE();
		stream.Write(this.sendbuffer, 0, this.WRITE_LEN());
	}

	public void send_bomb_planting(float pos_x, float pos_y, float pos_z, float rot_y)
	{
		if (this.client == null)
		{
			return;
		}
		if (!this.client.Connected)
		{
			return;
		}
		NetworkStream stream = this.client.GetStream();
		this.BEGIN_WRITE();
		this.WRITE_BYTE(245);
		this.WRITE_BYTE(38);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(0);
		this.WRITE_FLOAT(pos_x);
		this.WRITE_FLOAT(pos_y);
		this.WRITE_FLOAT(pos_z);
		this.WRITE_ANGLE(rot_y);
		this.END_WRITE();
		stream.Write(this.sendbuffer, 0, this.WRITE_LEN());
	}

	public void send_bomb_planted(byte val)
	{
		if (this.client == null)
		{
			return;
		}
		if (!this.client.Connected)
		{
			return;
		}
		NetworkStream stream = this.client.GetStream();
		this.BEGIN_WRITE();
		this.WRITE_BYTE(245);
		this.WRITE_BYTE(39);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(val);
		this.END_WRITE();
		stream.Write(this.sendbuffer, 0, this.WRITE_LEN());
	}

	public void send_bomb_diffusing(float pos_x, float pos_y, float pos_z)
	{
		if (this.client == null)
		{
			return;
		}
		if (!this.client.Connected)
		{
			return;
		}
		NetworkStream stream = this.client.GetStream();
		this.BEGIN_WRITE();
		this.WRITE_BYTE(245);
		this.WRITE_BYTE(40);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(1);
		this.END_WRITE();
		stream.Write(this.sendbuffer, 0, this.WRITE_LEN());
	}

	public void send_bomb_diffused(byte val)
	{
		if (this.client == null)
		{
			return;
		}
		if (!this.client.Connected)
		{
			return;
		}
		NetworkStream stream = this.client.GetStream();
		this.BEGIN_WRITE();
		this.WRITE_BYTE(245);
		this.WRITE_BYTE(41);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(val);
		this.END_WRITE();
		stream.Write(this.sendbuffer, 0, this.WRITE_LEN());
	}

	public void send_weapon_drop(int mode, int wid, int clip, byte puuid, Vector3 pos, Vector3 rot, Vector3 force, Vector3 torque)
	{
		if (this.client == null)
		{
			return;
		}
		if (!this.client.Connected)
		{
			return;
		}
		NetworkStream stream = this.client.GetStream();
		this.BEGIN_WRITE();
		this.WRITE_BYTE(245);
		this.WRITE_BYTE(44);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE((byte)mode);
		this.WRITE_BYTE((byte)wid);
		this.WRITE_BYTE((byte)clip);
		this.WRITE_BYTE(puuid);
		this.WRITE_FLOAT(pos.x);
		this.WRITE_FLOAT(pos.y);
		this.WRITE_FLOAT(pos.z);
		this.WRITE_FLOAT(rot.x);
		this.WRITE_FLOAT(rot.y);
		this.WRITE_FLOAT(rot.z);
		this.WRITE_FLOAT(force.x);
		this.WRITE_FLOAT(force.y);
		this.WRITE_FLOAT(force.z);
		this.WRITE_FLOAT(torque.x);
		this.WRITE_FLOAT(torque.y);
		this.WRITE_FLOAT(torque.z);
		this.END_WRITE();
		stream.Write(this.sendbuffer, 0, this.WRITE_LEN());
	}

	public void send_weapon_pos(byte mode, int uid, Vector3 pos, Vector3 rot)
	{
		if (this.client == null)
		{
			return;
		}
		if (!this.client.Connected)
		{
			return;
		}
		NetworkStream stream = this.client.GetStream();
		this.BEGIN_WRITE();
		this.WRITE_BYTE(245);
		this.WRITE_BYTE(45);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(mode);
		this.WRITE_BYTE((byte)uid);
		this.WRITE_FLOAT(pos.x);
		this.WRITE_FLOAT(pos.y);
		this.WRITE_FLOAT(pos.z);
		this.WRITE_FLOAT(rot.x);
		this.WRITE_FLOAT(rot.y);
		this.WRITE_FLOAT(rot.z);
		this.END_WRITE();
		stream.Write(this.sendbuffer, 0, this.WRITE_LEN());
	}

	public void send_weapon_pickup(int uid)
	{
		if (this.client == null)
		{
			return;
		}
		if (!this.client.Connected)
		{
			return;
		}
		NetworkStream stream = this.client.GetStream();
		this.BEGIN_WRITE();
		this.WRITE_BYTE(245);
		this.WRITE_BYTE(46);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE((byte)uid);
		this.END_WRITE();
		stream.Write(this.sendbuffer, 0, this.WRITE_LEN());
	}

	public void send_mapweapon_pos()
	{
		if (this.client == null)
		{
			return;
		}
		if (!this.client.Connected)
		{
			return;
		}
		NetworkStream stream = this.client.GetStream();
		this.BEGIN_WRITE();
		this.WRITE_BYTE(245);
		this.WRITE_BYTE(47);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(0);
		this.END_WRITE();
		stream.Write(this.sendbuffer, 0, this.WRITE_LEN());
	}

	public void send_weapon_clip(byte clip)
	{
		if (this.client == null)
		{
			return;
		}
		if (!this.client.Connected)
		{
			return;
		}
		NetworkStream stream = this.client.GetStream();
		this.BEGIN_WRITE();
		this.WRITE_BYTE(245);
		this.WRITE_BYTE(48);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(clip);
		this.END_WRITE();
		stream.Write(this.sendbuffer, 0, this.WRITE_LEN());
	}

	public void send_grenade_anim(int animid)
	{
		if (this.client == null)
		{
			return;
		}
		if (!this.client.Connected)
		{
			return;
		}
		NetworkStream stream = this.client.GetStream();
		this.BEGIN_WRITE();
		this.WRITE_BYTE(245);
		this.WRITE_BYTE(49);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE((byte)animid);
		this.END_WRITE();
		stream.Write(this.sendbuffer, 0, this.WRITE_LEN());
	}

	public void send_votestart(byte kickId)
	{
		if (this.client == null)
		{
			return;
		}
		if (!this.client.Connected)
		{
			return;
		}
		NetworkStream stream = this.client.GetStream();
		this.BEGIN_WRITE();
		this.WRITE_BYTE(245);
		this.WRITE_BYTE(56);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(kickId);
		this.END_WRITE();
		stream.Write(this.sendbuffer, 0, this.WRITE_LEN());
	}

	public void send_vote(byte answer)
	{
		if (this.client == null)
		{
			return;
		}
		if (!this.client.Connected)
		{
			return;
		}
		NetworkStream stream = this.client.GetStream();
		this.BEGIN_WRITE();
		this.WRITE_BYTE(245);
		this.WRITE_BYTE(57);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(answer);
		this.END_WRITE();
		stream.Write(this.sendbuffer, 0, this.WRITE_LEN());
		MonoBehaviour.print("answer " + answer);
	}

	public void send_ping()
	{
		if (this.client == null)
		{
			return;
		}
		if (!this.client.Connected)
		{
			return;
		}
		NetworkStream stream = this.client.GetStream();
		this.BEGIN_WRITE();
		this.WRITE_BYTE(245);
		this.WRITE_BYTE(255);
		this.WRITE_BYTE(0);
		this.WRITE_BYTE(0);
		Client.pingTime = Time.time;
		this.WRITE_FLOAT(Client.pingTime);
		this.END_WRITE();
		stream.Write(this.sendbuffer, 0, this.WRITE_LEN());
	}

	public void BEGIN_WRITE()
	{
		this.writepos = 0;
	}

	public void WRITE_BYTE(byte bvalue)
	{
		this.sendbuffer[this.writepos] = bvalue;
		this.writepos++;
	}

	public void WRITE_ANGLE(float fvalue)
	{
		if (fvalue > 360f)
		{
			fvalue = 360f;
		}
		else if (fvalue < 0f)
		{
			fvalue = 0f;
		}
		this.sendbuffer[this.writepos] = (byte)(0.7111111f * fvalue);
		this.writepos++;
	}

	private void WRITE_SHORT(short svalue)
	{
		byte[] array = this.EncodeShort(svalue);
		this.sendbuffer[this.writepos] = array[0];
		this.sendbuffer[this.writepos + 1] = array[1];
		this.writepos += 2;
	}

	private void WRITE_FLOAT(float fvalue)
	{
		byte[] array = this.EncodeFloat(fvalue);
		this.sendbuffer[this.writepos] = array[0];
		this.sendbuffer[this.writepos + 1] = array[1];
		this.sendbuffer[this.writepos + 2] = array[2];
		this.sendbuffer[this.writepos + 3] = array[3];
		this.writepos += 4;
	}

	private void WRITE_LONG(int ivalue)
	{
		byte[] array = this.EncodeInteger(ivalue);
		this.sendbuffer[this.writepos] = array[0];
		this.sendbuffer[this.writepos + 1] = array[1];
		this.sendbuffer[this.writepos + 2] = array[2];
		this.sendbuffer[this.writepos + 3] = array[3];
		this.writepos += 4;
	}

	private void WRITE_STRING(string svalue)
	{
		UTF8Encoding uTF8Encoding = new UTF8Encoding();
		int byteCount = uTF8Encoding.GetByteCount(svalue);
		byte[] array = new byte[byteCount];
		Buffer.BlockCopy(uTF8Encoding.GetBytes(svalue), 0, array, 0, byteCount);
		for (int i = 0; i < byteCount; i++)
		{
			this.WRITE_BYTE(array[i]);
		}
		this.WRITE_BYTE(0);
	}

	private void WRITE_STRING_SIZE(string svalue)
	{
		UTF8Encoding uTF8Encoding = new UTF8Encoding();
		int byteCount = uTF8Encoding.GetByteCount(svalue);
		this.WRITE_LONG(byteCount);
		byte[] array = new byte[byteCount];
		Buffer.BlockCopy(uTF8Encoding.GetBytes(svalue), 0, array, 0, byteCount);
		for (int i = 0; i < byteCount; i++)
		{
			this.WRITE_BYTE(array[i]);
		}
	}

	public int WRITE_LEN()
	{
		return this.writepos;
	}

	public void END_WRITE()
	{
		short svalue = (short)this.writepos;
		this.writepos = 2;
		this.WRITE_SHORT(svalue);
		this.writepos = (int)svalue;
	}

	public byte[] EncodeShort(short inShort)
	{
		return BitConverter.GetBytes(inShort);
	}

	public byte[] EncodeInteger(int inInt)
	{
		return BitConverter.GetBytes(inInt);
	}

	public byte[] EncodeFloat(float inFloat)
	{
		return BitConverter.GetBytes(inFloat);
	}

	public byte[] EncodeStringUTF8(string inString)
	{
		UTF8Encoding uTF8Encoding = new UTF8Encoding();
		int byteCount = uTF8Encoding.GetByteCount(inString);
		byte[] array = new byte[byteCount];
		Buffer.BlockCopy(uTF8Encoding.GetBytes(inString), 0, array, 0, byteCount);
		return array;
	}

	public byte[] EncodeStringASCII(string inString)
	{
		ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
		int byteCount = aSCIIEncoding.GetByteCount(inString);
		byte[] array = new byte[byteCount];
		Buffer.BlockCopy(aSCIIEncoding.GetBytes(inString), 0, array, 0, byteCount);
		return array;
	}

	public byte[] EncodeVector2(Vector2 inObject)
	{
		byte[] array = new byte[8];
		Buffer.BlockCopy(BitConverter.GetBytes(inObject.x), 0, array, 0, 4);
		Buffer.BlockCopy(BitConverter.GetBytes(inObject.y), 0, array, 4, 4);
		return array;
	}

	public byte[] EncodeVector3(Vector3 inObject)
	{
		byte[] array = new byte[12];
		Buffer.BlockCopy(BitConverter.GetBytes(inObject.x), 0, array, 0, 4);
		Buffer.BlockCopy(BitConverter.GetBytes(inObject.y), 0, array, 4, 4);
		Buffer.BlockCopy(BitConverter.GetBytes(inObject.z), 0, array, 8, 4);
		return array;
	}

	public byte[] EncodeVector4(Vector4 inObject)
	{
		byte[] array = new byte[16];
		Buffer.BlockCopy(BitConverter.GetBytes(inObject.x), 0, array, 0, 4);
		Buffer.BlockCopy(BitConverter.GetBytes(inObject.y), 0, array, 4, 4);
		Buffer.BlockCopy(BitConverter.GetBytes(inObject.z), 0, array, 8, 4);
		Buffer.BlockCopy(BitConverter.GetBytes(inObject.w), 0, array, 12, 4);
		return array;
	}

	private byte[] EncodeQuaternion(Quaternion inObject)
	{
		byte[] array = new byte[16];
		Buffer.BlockCopy(BitConverter.GetBytes(inObject.x), 0, array, 0, 4);
		Buffer.BlockCopy(BitConverter.GetBytes(inObject.y), 0, array, 4, 4);
		Buffer.BlockCopy(BitConverter.GetBytes(inObject.z), 0, array, 8, 4);
		Buffer.BlockCopy(BitConverter.GetBytes(inObject.w), 0, array, 12, 4);
		return array;
	}

	public byte[] EncodeColor(Color inObject)
	{
		byte[] array = new byte[16];
		Buffer.BlockCopy(BitConverter.GetBytes(inObject.r), 0, array, 0, 4);
		Buffer.BlockCopy(BitConverter.GetBytes(inObject.g), 0, array, 4, 4);
		Buffer.BlockCopy(BitConverter.GetBytes(inObject.b), 0, array, 8, 4);
		Buffer.BlockCopy(BitConverter.GetBytes(inObject.a), 0, array, 12, 4);
		return array;
	}

	public void BEGIN_READ(byte[] inBytes, int len, int startpos)
	{
		this.readbuffer = inBytes;
		this.readlen = len;
		this.readpos = startpos;
		this.readerror = false;
	}

	public int READ_BYTE()
	{
		if (this.readpos + 1 > this.readlen)
		{
			this.readerror = true;
			return 0;
		}
		int result = (int)this.readbuffer[this.readpos];
		this.readpos++;
		return result;
	}

	public int READ_SHORT()
	{
		if (this.readpos + 2 > this.readlen)
		{
			this.readerror = true;
			return 0;
		}
		int result = (int)this.DecodeShort2(this.readbuffer, this.readpos);
		this.readpos += 2;
		return result;
	}

	public int READ_LONG()
	{
		if (this.readpos + 4 > this.readlen)
		{
			this.readerror = true;
			return 0;
		}
		int result = this.DecodeInteger(this.readbuffer, this.readpos);
		this.readpos += 4;
		return result;
	}

	public float READ_FLOAT()
	{
		if (this.readpos + 4 > this.readlen)
		{
			this.readerror = true;
			return 0f;
		}
		float result = this.DecodeSingle(this.readbuffer, this.readpos);
		this.readpos += 4;
		return result;
	}

	public string READ_STRING()
	{
		int num = 0;
		int index = this.readpos;
		while (this.readpos < this.readlen)
		{
			if (this.readbuffer[this.readpos] == 0)
			{
				break;
			}
			num++;
			this.readpos++;
		}
		this.readpos++;
		if (num == 0)
		{
			return string.Empty;
		}
		return Encoding.UTF8.GetString(this.readbuffer, index, num);
	}

	public float READ_ANGLE()
	{
		if (this.readpos + 1 > this.readlen)
		{
			this.readerror = true;
			return 0f;
		}
		float result = (float)this.readbuffer[this.readpos] * 360f / 256f;
		this.readpos++;
		return result;
	}

	public float READ_COORD()
	{
		if (this.readpos + 2 > this.readlen)
		{
			this.readerror = true;
			return 0f;
		}
		float result = (float)this.DecodeShort2(this.readbuffer, this.readpos) * (1f / Client.COORDRES) - Client.COORDMAX;
		this.readpos += 2;
		return result;
	}

	public bool READ_ERROR()
	{
		return this.readerror;
	}

	public int DecodeShort(byte[] inBytes, int pos)
	{
		return (int)BitConverter.ToUInt16(inBytes, pos);
	}

	public ushort DecodeShort2(byte[] inBytes, int pos)
	{
		return BitConverter.ToUInt16(inBytes, pos);
	}

	public int DecodeInteger(byte[] inBytes, int pos)
	{
		return BitConverter.ToInt32(inBytes, pos);
	}

	public float DecodeSingle(byte[] inBytes, int pos)
	{
		return BitConverter.ToSingle(inBytes, pos);
	}

	public Vector2 DecodeVector2(byte[] inBytes)
	{
		return new Vector2
		{
			x = BitConverter.ToSingle(inBytes, 0),
			y = BitConverter.ToSingle(inBytes, 4)
		};
	}

	public Vector3 DecodeVector3(byte[] inBytes)
	{
		return new Vector3
		{
			x = BitConverter.ToSingle(inBytes, 0),
			y = BitConverter.ToSingle(inBytes, 4),
			z = BitConverter.ToSingle(inBytes, 8)
		};
	}

	public Vector4 DecodeVector4(byte[] inBytes)
	{
		return new Vector4
		{
			x = BitConverter.ToSingle(inBytes, 0),
			y = BitConverter.ToSingle(inBytes, 4),
			z = BitConverter.ToSingle(inBytes, 8),
			w = BitConverter.ToSingle(inBytes, 12)
		};
	}

	private Quaternion DecodeQuaternion(byte[] inBytes)
	{
		return new Quaternion
		{
			x = BitConverter.ToSingle(inBytes, 0),
			y = BitConverter.ToSingle(inBytes, 4),
			z = BitConverter.ToSingle(inBytes, 8),
			w = BitConverter.ToSingle(inBytes, 12)
		};
	}

	public Color DecodeColor(byte[] inBytes)
	{
		return new Color(0f, 0f, 0f, 0f)
		{
			r = BitConverter.ToSingle(inBytes, 0),
			g = BitConverter.ToSingle(inBytes, 4),
			b = BitConverter.ToSingle(inBytes, 8),
			a = BitConverter.ToSingle(inBytes, 12)
		};
	}
}
