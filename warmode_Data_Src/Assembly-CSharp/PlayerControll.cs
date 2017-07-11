using AssemblyCSharp;
using RootMotion.FinalIK;
using RootMotion.FinalIK.Demos;
using System;
using UnityEngine;

public class PlayerControll : MonoBehaviour
{
	public const int MAX_PLAYERS = 16;

	[Range(0f, 1.4f)]
	public float moveForward = 0.1f;

	[Range(0f, 0.2f)]
	public float moveUp = 0.1f;

	[Range(0f, 0.4f)]
	public float moveRight = 0.1f;

	public static float sMoveForward = 0.1f;

	public static float sMoveUp = 0.1f;

	public static float sMoveRight = 0.1f;

	public static CPlayerData[] Player = new CPlayerData[16];

	public static GameObject pgoPlayer;

	public static GameObject pgoPlayerRD;

	public static GameObject pgoZombie;

	public static GameObject pgoZombieRD;

	private static GameObject pgoBlood = null;

	private static GameObject LocalPlayer = null;

	private static GameObject pgoMuzzleFlashHeavy = null;

	private static GameObject pgoMuzzleFlashPistol = null;

	private static GameObject pgoMuzzleFlashRifle = null;

	private static GameObject pgoMuzzleFlashShotgun = null;

	private static GameObject playerBomb = null;

	private static Color muzzlecolor = new Color(1f, 0.85f, 0.55f);

	public static bool extralod = false;

	public static bool nord = false;

	public GameObject m_pTracePrefab;

	public static GameObject m_TracePrefab = null;

	private static AudioClip[] steps_walk_concrete = new AudioClip[4];

	private static AudioClip[] steps_run_concrete = new AudioClip[4];

	private static AudioClip[] steps_walk_gravel = new AudioClip[4];

	private static AudioClip[] steps_run_gravel = new AudioClip[4];

	private static RaycastHit hit;

	public static bool[] vp = new bool[16];

	public static bool[] vps = new bool[16];

	private static Vector3[] screenPos = new Vector3[9];

	private static Ray ray;

	private static int castmask = 1;

	public static void Init()
	{
		MonoBehaviour.print("PlayerControll.Init");
		PlayerControll.PlayerAll();
	}

	public static void PlayerAll()
	{
		for (int i = 0; i < 16; i++)
		{
			if (PlayerControll.Player[i] != null)
			{
				if (PlayerControll.Player[i].go)
				{
					UnityEngine.Object.Destroy(PlayerControll.Player[i].go);
					PlayerControll.Player[i].go.name = "_";
					PlayerControll.Player[i].go = null;
				}
				PlayerControll.Player[i] = null;
			}
		}
	}

	public static void ClearScore()
	{
		for (int i = 0; i < 16; i++)
		{
			if (PlayerControll.Player[i] != null)
			{
				PlayerControll.Player[i].SetDeaths(0);
				PlayerControll.Player[i].SetFrags(0);
				PlayerControll.Player[i].SetPoints(0);
			}
		}
	}

	public void PostAwake()
	{
		PlayerControll.LocalPlayer = GameObject.Find("LocalPlayer");
		PlayerControll.pgoPlayer = ContentLoader_.LoadGameObject("pwmplayer");
		PlayerControll.pgoPlayerRD = ContentLoader_.LoadGameObject("pwmplayerrd");
		PlayerControll.pgoZombie = ContentLoader_.LoadGameObject("pwmzombie");
		PlayerControll.pgoZombieRD = ContentLoader_.LoadGameObject("pwmzombierd");
		PlayerControll.pgoBlood = ContentLoader_.LoadGameObject("Blood");
		PlayerControll.pgoMuzzleFlashHeavy = ContentLoader_.LoadGameObject("MuzzleflashHeavy");
		PlayerControll.pgoMuzzleFlashPistol = ContentLoader_.LoadGameObject("MuzzleflashPistol");
		PlayerControll.pgoMuzzleFlashRifle = ContentLoader_.LoadGameObject("MuzzleflashRifle");
		PlayerControll.pgoMuzzleFlashShotgun = ContentLoader_.LoadGameObject("MuzzleflashShotgun");
		PlayerControll.playerBomb = ContentLoader_.LoadGameObject("c4_player");
		PlayerControll.m_TracePrefab = this.m_pTracePrefab;
		PlayerControll.steps_walk_concrete[0] = SND.GetSoundByName("walk_concrete1");
		PlayerControll.steps_walk_concrete[1] = SND.GetSoundByName("walk_concrete2");
		PlayerControll.steps_walk_concrete[2] = SND.GetSoundByName("walk_concrete3");
		PlayerControll.steps_walk_concrete[3] = SND.GetSoundByName("walk_concrete4");
		PlayerControll.steps_run_concrete[0] = SND.GetSoundByName("run_concrete1");
		PlayerControll.steps_run_concrete[1] = SND.GetSoundByName("run_concrete2");
		PlayerControll.steps_run_concrete[2] = SND.GetSoundByName("run_concrete3");
		PlayerControll.steps_run_concrete[3] = SND.GetSoundByName("run_concrete4");
		PlayerControll.steps_walk_gravel[0] = SND.GetSoundByName("walk_gravel1");
		PlayerControll.steps_walk_gravel[1] = SND.GetSoundByName("walk_gravel2");
		PlayerControll.steps_walk_gravel[2] = SND.GetSoundByName("walk_gravel3");
		PlayerControll.steps_walk_gravel[3] = SND.GetSoundByName("walk_gravel4");
		PlayerControll.steps_run_gravel[0] = SND.GetSoundByName("run_gravel1");
		PlayerControll.steps_run_gravel[1] = SND.GetSoundByName("run_gravel2");
		PlayerControll.steps_run_gravel[2] = SND.GetSoundByName("run_gravel3");
		PlayerControll.steps_run_gravel[3] = SND.GetSoundByName("run_gravel4");
	}

	private void Update()
	{
		if (!DeadCam.show && !SpecCam.show)
		{
			PlayerControll.CheckVisible();
		}
		for (int i = 0; i < 16; i++)
		{
			if (PlayerControll.Player[i] != null)
			{
				if (!(PlayerControll.Player[i].go == null))
				{
					PlayerControll.Player[i].currPos = Vector3.Lerp(PlayerControll.Player[i].currPos, PlayerControll.Player[i].position, Time.deltaTime * 10f);
					if (!PlayerControll.vp[i])
					{
						PlayerControll.Player[i].go.SetActive(false);
					}
					else
					{
						if (!PlayerControll.Player[i].go.activeSelf)
						{
							PlayerControll.Player[i].go.SetActive(true);
							PlayerControll.UpdateWeapon(i);
							PlayerControll.UpdateWeaponModel(i);
							PlayerControll.UpdateState(i);
						}
						PlayerControll.Player[i].go.transform.position = PlayerControll.Player[i].currPos;
						float num = 10f;
						if (PlayerControll.Player[i].animator && PlayerControll.Player[i].animator.isActiveAndEnabled && PlayerControll.Player[i].turntime != 0f && Time.time > PlayerControll.Player[i].turntime + 0.25f)
						{
							PlayerControll.Player[i].animator.SetInteger("turn", 0);
							PlayerControll.Player[i].turn = 0;
							PlayerControll.Player[i].turntime = 0f;
						}
						float num2 = PlayerControll.Player[i].go.transform.eulerAngles.y;
						float num3 = PlayerControll.Player[i].oldy - num2;
						if (num3 > 180f)
						{
							num2 += 360f;
						}
						if (num3 < -180f)
						{
							num2 -= 360f;
						}
						num2 = Mathf.Lerp(num2, PlayerControll.Player[i].oldy, Time.deltaTime * num);
						PlayerControll.Player[i].go.transform.eulerAngles = new Vector3(0f, num2, 0f);
						float num4 = PlayerControll.Player[i].goAim.transform.eulerAngles.x;
						float num5 = PlayerControll.Player[i].rotation.x - num4;
						if (num5 > 180f)
						{
							num4 += 360f;
						}
						if (num5 < -180f)
						{
							num4 -= 360f;
						}
						num4 = Mathf.Lerp(num4, PlayerControll.Player[i].rotation.x, Time.deltaTime * 10f);
						float num6 = PlayerControll.Player[i].goAim.transform.eulerAngles.y;
						float num7 = PlayerControll.Player[i].rotation.y - num6;
						if (num7 > 180f)
						{
							num6 += 360f;
						}
						if (num7 < -180f)
						{
							num6 -= 360f;
						}
						num6 = Mathf.Lerp(num6, PlayerControll.Player[i].rotation.y, Time.deltaTime * 10f);
						PlayerControll.Player[i].goAim.transform.localPosition = new Vector3(0f, 1.75f, 0f);
						if (ScoreBoard.gamemode == 3 && PlayerControll.Player[i].Team == 0)
						{
							PlayerControll.Player[i].goAim.transform.localPosition = new Vector3(0f, 0.08139535f, 0f);
							float num8 = num4;
							if (num8 > 0f && num8 <= 90f && num8 > 40f)
							{
								num8 = 40f;
							}
							if (num8 < 360f && num8 >= 270f && num8 < 320f)
							{
								num8 = 320f;
							}
							PlayerControll.Player[i].goAim.transform.eulerAngles = new Vector3(num8, num6, 0f);
						}
						else
						{
							PlayerControll.Player[i].goAim.transform.eulerAngles = new Vector3(num4, num6, 0f);
						}
						PlayerControll.Player[i].goAim.transform.position = PlayerControll.Player[i].goAim.transform.position + PlayerControll.Player[i].goAim.transform.forward * 50f;
						if (PlayerControll.Player[i].animator != null)
						{
							if (ScoreBoard.gamemode == 3 && PlayerControll.Player[i].Team == 0)
							{
								PlayerControll.Player[i].currAnimSpeed = Mathf.Lerp(PlayerControll.Player[i].currAnimSpeed, PlayerControll.Player[i].animSpeed, Time.deltaTime * 4f);
								PlayerControll.Player[i].currAnimStrafe = Mathf.Lerp(PlayerControll.Player[i].currAnimStrafe, PlayerControll.Player[i].animStrafe, Time.deltaTime * 4f);
								if (PlayerControll.Player[i].currAnimSpeed > 0f)
								{
									PlayerControll.Player[i].animator.SetFloat("Speed", PlayerControll.Player[i].currAnimSpeed * 2f);
								}
								else
								{
									PlayerControll.Player[i].animator.SetFloat("Speed", PlayerControll.Player[i].currAnimSpeed * 3f);
								}
								PlayerControll.Player[i].animator.SetFloat("Strafe", PlayerControll.Player[i].currAnimStrafe * 3f);
							}
							else if (PlayerControll.Player[i].state > 0)
							{
								PlayerControll.Player[i].currMove.x = Mathf.Lerp(PlayerControll.Player[i].currMove.x, PlayerControll.Player[i].move.x, Time.deltaTime * 10f);
								PlayerControll.Player[i].currMove.z = Mathf.Lerp(PlayerControll.Player[i].currMove.z, PlayerControll.Player[i].move.z, Time.deltaTime * 10f);
								PlayerControll.Player[i].animator.SetFloat("h", PlayerControll.Player[i].currMove.x);
								PlayerControll.Player[i].animator.SetFloat("v", PlayerControll.Player[i].currMove.z);
							}
							PlayerControll.UpdateJump(i);
						}
					}
				}
			}
		}
	}

	public static void Spawn(int id, float x, float y, float z)
	{
		if (PlayerControll.Player[id] == null)
		{
			return;
		}
		int team = PlayerControll.Player[id].Team;
		int deadflag = 0;
		string name = PlayerControll.Player[id].Name;
		string clanName = PlayerControll.Player[id].ClanName;
		int frags = PlayerControll.Player[id].Frags;
		int deaths = PlayerControll.Player[id].Deaths;
		int points = PlayerControll.Player[id].Points;
		int level = PlayerControll.Player[id].level;
		int badge_back = PlayerControll.Player[id].badge_back;
		int badge_icon = PlayerControll.Player[id].badge_icon;
		int mask_merc = PlayerControll.Player[id].mask_merc;
		int mask_warcorp = PlayerControll.Player[id].mask_warcorp;
		PlayerControll.CreatePlayer(id, name, clanName, team, deadflag, x, y, z, false);
		PlayerControll.SetPlayerStats(id, frags, deaths, points, level);
		PlayerControll.SetPlayerBadge(id, badge_back, badge_icon);
		PlayerControll.SetPlayerMask(id, mask_merc, mask_warcorp);
		PlayerControll.SetPlayerBomb(id, false);
		ScoreTop.UpdateData();
	}

	public static void MakeZombie(int id)
	{
		if (PlayerControll.Player[id] == null)
		{
			return;
		}
		int deadFlag = PlayerControll.Player[id].DeadFlag;
		int team = 0;
		string name = PlayerControll.Player[id].Name;
		string clanName = PlayerControll.Player[id].ClanName;
		int frags = PlayerControll.Player[id].Frags;
		int deaths = PlayerControll.Player[id].Deaths;
		int points = PlayerControll.Player[id].Points;
		int level = PlayerControll.Player[id].level;
		Vector3 position = PlayerControll.Player[id].position;
		PlayerControll.CreatePlayer(id, name, clanName, team, deadFlag, position.x, position.y, position.z, true);
		PlayerControll.SetPlayerStats(id, frags, deaths, points, level);
		PlayerControll.Player[id].gosWeapon.transform.position = PlayerControll.Player[id].currPos;
		PlayerControll.Player[id].asWeapon.PlayOneShot(Zombie.infectionSound);
	}

	public static void RemapLayers(int id = -1)
	{
		for (int i = 0; i < 16; i++)
		{
			if (id < 0 || i == id)
			{
				if (i != Client.ID)
				{
					if (PlayerControll.Player[i] != null)
					{
						if (!(PlayerControll.Player[i].go == null))
						{
							Transform[] componentsInChildren = PlayerControll.Player[i].go.GetComponentsInChildren<Transform>();
							Transform[] array = componentsInChildren;
							for (int j = 0; j < array.Length; j++)
							{
								Transform transform = array[j];
								if (!(transform.gameObject.name == "Collider"))
								{
									if (!(transform.gameObject.name == "ColliderTrigger"))
									{
										if (!(transform.gameObject.name == "PoleTarget"))
										{
											if (!(transform.gameObject.name == "AimTarget"))
											{
												if (BasePlayer.team == PlayerControll.Player[i].Team)
												{
													transform.gameObject.layer = 2;
												}
												else
												{
													transform.gameObject.layer = 25;
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}

	public static void CreatePlayer(int id, string name, string clanname, int team, int deadflag, float x, float y, float z, bool zombie)
	{
		PlayerControll.DestroyPlayer(id);
		PlayerControll.Player[id] = new CPlayerData();
		PlayerControll.Player[id].Name = name;
		PlayerControll.Player[id].ClanName = clanname;
		PlayerControll.Player[id].DeadFlag = deadflag;
		PlayerControll.Player[id].Team = team;
		if (id == Client.ID)
		{
			return;
		}
		if (team == 255)
		{
			return;
		}
		if (zombie)
		{
			PlayerControll.Player[id].go = (UnityEngine.Object.Instantiate(PlayerControll.pgoZombie, new Vector3(x, y, z), PlayerControll.pgoZombie.transform.rotation) as GameObject);
			PlayerControll.Player[id].currweapon = 31;
		}
		else
		{
			PlayerControll.Player[id].go = (UnityEngine.Object.Instantiate(PlayerControll.pgoPlayer, new Vector3(x, y, z), PlayerControll.pgoPlayer.transform.rotation) as GameObject);
		}
		PlayerControll.Player[id].go.name = "player_" + id.ToString();
		PlayerControll.Player[id].position = new Vector3(x, y, z);
		PlayerControll.Player[id].oldpos = new Vector3(x, y, z);
		PlayerControll.Player[id].currPos = new Vector3(x, y, z);
		PlayerControll.Player[id].animator = PlayerControll.Player[id].go.GetComponent<Animator>();
		PlayerControll.Player[id].gosWeapon = new GameObject("sound_weapon_" + id);
		PlayerControll.Player[id].asWeapon = PlayerControll.Player[id].gosWeapon.AddComponent<AudioSource>();
		PlayerControll.Player[id].asWeapon.maxDistance = 50f;
		PlayerControll.Player[id].asWeapon.rolloffMode = AudioRolloffMode.Linear;
		PlayerControll.Player[id].asWeapon.volume = 0.5f * Options.gamevol;
		PlayerControll.Player[id].asWeapon.spatialBlend = 1f;
		PlayerControll.Player[id].gosStep = new GameObject("sound_step_" + id);
		PlayerControll.Player[id].asSteps = PlayerControll.Player[id].gosStep.AddComponent<AudioSource>();
		PlayerControll.Player[id].asSteps.maxDistance = 25f;
		PlayerControll.Player[id].asSteps.rolloffMode = AudioRolloffMode.Linear;
		PlayerControll.Player[id].asSteps.volume = 0.5f * Options.gamevol;
		PlayerControll.Player[id].asSteps.spatialBlend = 1f;
		PlayerControll.Player[id].aimik = PlayerControll.Player[id].go.GetComponent<AimIK>();
		PlayerControll.Player[id].recoil = PlayerControll.Player[id].go.GetComponent<Recoil>();
		GameObject gameObject = GameObject.Find(PlayerControll.Player[id].go.name + "/HitReaction");
		PlayerControll.Player[id].hitReaction = gameObject.GetComponent<HitReaction>();
		PlayerControll.Player[id].goAim = GameObject.Find(PlayerControll.Player[id].go.name + "/AimTarget");
		if (PlayerControll.Player[id].goAim == null)
		{
			MonoBehaviour.print("goAim not found");
		}
		if (zombie)
		{
			GameObject gameObject2 = GameObject.Find(PlayerControll.Player[id].go.name + "/Hip/Spine_1");
			gameObject2.GetComponent<vp_DamageHandlerPlayer>().Set((byte)id, 3);
			gameObject2 = GameObject.Find(PlayerControll.Player[id].go.name + "/Hip/Spine_1/Spine_2");
			gameObject2.GetComponent<vp_DamageHandlerPlayer>().Set((byte)id, 2);
			gameObject2 = GameObject.Find(PlayerControll.Player[id].go.name + "/Hip/Spine_1/Spine_2/Spine_3/Neck/Head");
			gameObject2.GetComponent<vp_DamageHandlerPlayer>().Set((byte)id, 1);
			gameObject2 = GameObject.Find(PlayerControll.Player[id].go.name + "/Hip/Spine_1/Spine_2/Spine_3/L_Sholder/L_Hand");
			gameObject2.GetComponent<vp_DamageHandlerPlayer>().Set((byte)id, 4);
			gameObject2 = GameObject.Find(PlayerControll.Player[id].go.name + "/Hip/Spine_1/Spine_2/Spine_3/L_Sholder/L_Hand/L_Elbow/L_Elbow1");
			gameObject2.GetComponent<vp_DamageHandlerPlayer>().Set((byte)id, 4);
			gameObject2 = GameObject.Find(PlayerControll.Player[id].go.name + "/Hip/Spine_1/Spine_2/Spine_3/R_Sholder/R_Hand");
			gameObject2.GetComponent<vp_DamageHandlerPlayer>().Set((byte)id, 4);
			gameObject2 = GameObject.Find(PlayerControll.Player[id].go.name + "/Hip/Spine_1/Spine_2/Spine_3/R_Sholder/R_Hand/R_Elbow/R_Elbow1");
			gameObject2.GetComponent<vp_DamageHandlerPlayer>().Set((byte)id, 4);
			gameObject2 = GameObject.Find(PlayerControll.Player[id].go.name + "/Hip/L_Arm");
			gameObject2.GetComponent<vp_DamageHandlerPlayer>().Set((byte)id, 5);
			gameObject2 = GameObject.Find(PlayerControll.Player[id].go.name + "/Hip/L_Arm/L_Knee");
			gameObject2.GetComponent<vp_DamageHandlerPlayer>().Set((byte)id, 5);
			gameObject2 = GameObject.Find(PlayerControll.Player[id].go.name + "/Hip/R_Arm");
			gameObject2.GetComponent<vp_DamageHandlerPlayer>().Set((byte)id, 5);
			gameObject2 = GameObject.Find(PlayerControll.Player[id].go.name + "/Hip/R_Arm/R_Knee");
			gameObject2.GetComponent<vp_DamageHandlerPlayer>().Set((byte)id, 5);
		}
		else
		{
			GameObject gameObject2 = GameObject.Find(PlayerControll.Player[id].go.name + "/Bip001/Bip001 Pelvis/Bip001 Spine");
			gameObject2.GetComponent<vp_DamageHandlerPlayer>().Set((byte)id, 3);
			gameObject2 = GameObject.Find(PlayerControll.Player[id].go.name + "/Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1");
			gameObject2.GetComponent<vp_DamageHandlerPlayer>().Set((byte)id, 2);
			PlayerControll.Player[id].goRootBomb = UnityEngine.Object.Instantiate<GameObject>(PlayerControll.playerBomb);
			PlayerControll.Player[id].goRootBomb.transform.SetParent(gameObject2.transform, false);
			PlayerControll.Player[id].goRootBomb.layer = 25;
			PlayerControll.Player[id].goRootBomb.transform.localPosition = new Vector3(-0.215f, -0.206f, 0.024f);
			PlayerControll.Player[id].goRootBomb.transform.localEulerAngles = new Vector3(0f, 0f, 180f);
			PlayerControll.Player[id].goRootBomb.SetActive(false);
			gameObject2 = GameObject.Find(PlayerControll.Player[id].go.name + "/Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 Head");
			gameObject2.GetComponent<vp_DamageHandlerPlayer>().Set((byte)id, 1);
			gameObject2 = GameObject.Find(PlayerControll.Player[id].go.name + "/Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 L Clavicle/Bip001 L UpperArm");
			gameObject2.GetComponent<vp_DamageHandlerPlayer>().Set((byte)id, 4);
			gameObject2 = GameObject.Find(PlayerControll.Player[id].go.name + "/Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 L Clavicle/Bip001 L UpperArm/Bip001 L Forearm");
			gameObject2.GetComponent<vp_DamageHandlerPlayer>().Set((byte)id, 4);
			gameObject2 = GameObject.Find(PlayerControll.Player[id].go.name + "/Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 R Clavicle/Bip001 R UpperArm");
			gameObject2.GetComponent<vp_DamageHandlerPlayer>().Set((byte)id, 4);
			gameObject2 = GameObject.Find(PlayerControll.Player[id].go.name + "/Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm");
			gameObject2.GetComponent<vp_DamageHandlerPlayer>().Set((byte)id, 4);
			gameObject2 = GameObject.Find(PlayerControll.Player[id].go.name + "/Bip001/Bip001 Pelvis/Bip001 L Thigh");
			gameObject2.GetComponent<vp_DamageHandlerPlayer>().Set((byte)id, 5);
			gameObject2 = GameObject.Find(PlayerControll.Player[id].go.name + "/Bip001/Bip001 Pelvis/Bip001 L Thigh/Bip001 L Calf");
			gameObject2.GetComponent<vp_DamageHandlerPlayer>().Set((byte)id, 5);
			gameObject2 = GameObject.Find(PlayerControll.Player[id].go.name + "/Bip001/Bip001 Pelvis/Bip001 R Thigh");
			gameObject2.GetComponent<vp_DamageHandlerPlayer>().Set((byte)id, 5);
			gameObject2 = GameObject.Find(PlayerControll.Player[id].go.name + "/Bip001/Bip001 Pelvis/Bip001 R Thigh/Bip001 R Calf");
			gameObject2.GetComponent<vp_DamageHandlerPlayer>().Set((byte)id, 5);
			GameObject.Find(PlayerControll.Player[id].go.name + "/Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/AimTransform/Laser").SetActive(false);
		}
		PlayerControll.Player[id].collider = GameObject.Find(PlayerControll.Player[id].go.name + "/Collider");
		PlayerControll.Player[id].colliderTrigger = GameObject.Find(PlayerControll.Player[id].go.name + "/ColliderTrigger");
		if (ScoreBoard.gamemode == 3)
		{
			PlayerControll.Player[id].collider.SetActive(false);
			PlayerControll.Player[id].colliderTrigger.SetActive(false);
		}
		Transform[] componentsInChildren = PlayerControll.Player[id].go.GetComponentsInChildren<Transform>();
		Transform[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Transform transform = array[i];
			if (transform.gameObject.name == "Collider")
			{
				transform.gameObject.layer = 21;
			}
			else if (transform.gameObject.name == "ColliderTrigger")
			{
				transform.gameObject.layer = 27;
			}
			else if (transform.gameObject.name == "PoleTarget")
			{
				transform.gameObject.GetComponent<Renderer>().enabled = false;
			}
			else if (transform.gameObject.name == "AimTarget")
			{
				transform.gameObject.GetComponent<Renderer>().enabled = false;
			}
			else if (zombie)
			{
				transform.gameObject.layer = 18;
			}
			else
			{
				transform.gameObject.layer = 25;
			}
		}
		LightmapModel lightmapModel = null;
		if (Options.xLightmapShadow == 1)
		{
			lightmapModel = PlayerControll.Player[id].go.AddComponent<LightmapModel>();
		}
		if (zombie)
		{
			if (lightmapModel)
			{
				lightmapModel.go = GameObject.Find(PlayerControll.Player[id].go.name + "/player_zombie");
			}
		}
		else if (team == 0)
		{
			GameObject.Find(PlayerControll.Player[id].go.name + "/player_warcorp").SetActive(false);
			if (lightmapModel)
			{
				lightmapModel.go = GameObject.Find(PlayerControll.Player[id].go.name + "/player_merc");
			}
		}
		else if (team == 1)
		{
			GameObject.Find(PlayerControll.Player[id].go.name + "/player_merc").SetActive(false);
			if (lightmapModel)
			{
				lightmapModel.go = GameObject.Find(PlayerControll.Player[id].go.name + "/player_warcorp");
			}
		}
	}

	public static void SetPlayerStats(int id, int frags, int deaths, int points, int level)
	{
		if (PlayerControll.Player[id] == null)
		{
			return;
		}
		PlayerControll.Player[id].SetFrags(frags);
		PlayerControll.Player[id].SetDeaths(deaths);
		PlayerControll.Player[id].SetPoints(points);
		PlayerControll.Player[id].SetLevel(level);
	}

	public static void SetPlayerBadge(int id, int badge_back, int badge_icon)
	{
		if (PlayerControll.Player[id] == null)
		{
			return;
		}
		PlayerControll.Player[id].badge_back = badge_back;
		PlayerControll.Player[id].badge_icon = badge_icon;
	}

	public static void SetPlayerBomb(int id, bool val)
	{
		if (PlayerControll.Player[id] == null)
		{
			return;
		}
		PlayerControll.Player[id].bomb = val;
	}

	public static void SetRootBombVisible(int id, bool val)
	{
		if (PlayerControll.Player[id] == null || PlayerControll.Player[id].goRootBomb == null)
		{
			return;
		}
		PlayerControll.Player[id].goRootBomb.SetActive(val);
	}

	public static void SetBombAnimation(int id, int val)
	{
		if (PlayerControll.Player[id] == null)
		{
			return;
		}
		if (PlayerControll.Player[id].animator == null)
		{
			return;
		}
		if (!PlayerControll.Player[id].go.active)
		{
			return;
		}
		PlayerControll.Player[id].animator.SetInteger("w", val);
	}

	public static void SetGrenadeAnimation(int id, int val)
	{
		if (PlayerControll.Player[id] == null)
		{
			return;
		}
		if (PlayerControll.Player[id].animator == null)
		{
			return;
		}
		if (!PlayerControll.Player[id].go.active)
		{
			return;
		}
		PlayerControll.Player[id].animator.SetInteger("w", val);
	}

	public static void SetBombSound(int id, int val)
	{
		if (PlayerControll.Player[id] == null)
		{
			return;
		}
		PlayerControll.Player[id].gosWeapon.transform.position = PlayerControll.Player[id].currPos;
		if (val == 0)
		{
			PlayerControll.Player[id].asWeapon.Stop();
		}
		else if (val == 1)
		{
			PlayerControll.Player[id].asWeapon.PlayOneShot(C4.plantStartSound);
		}
		else if (val == 2)
		{
			PlayerControll.Player[id].asWeapon.PlayOneShot(C4Place.plantEndSound);
		}
	}

	public static void SetPlayerMask(int id, int mask_merc, int mask_warcorp)
	{
		if (PlayerControll.Player[id] == null)
		{
			return;
		}
		PlayerControll.Player[id].mask_merc = mask_merc;
		PlayerControll.Player[id].mask_warcorp = mask_warcorp;
		if (PlayerControll.Player[id].go == null)
		{
			return;
		}
		if (ScoreBoard.gamemode != 3 || PlayerControll.Player[id].Team == 0)
		{
		}
		if (PlayerControll.Player[id].Team == 0 && PlayerControll.Player[id].mask_merc != 0)
		{
			GameObject gameObject = GameObject.Find(PlayerControll.Player[id].go.name + "/player_merc");
			SkinnedMeshRenderer component = gameObject.GetComponent<SkinnedMeshRenderer>();
			Texture2D textureByName = TEX.GetTextureByName("_" + MenuShop.shopdata[mask_merc].iconname);
			component.materials[0].SetTexture(0, textureByName);
		}
		else if (PlayerControll.Player[id].Team == 1 && PlayerControll.Player[id].mask_warcorp != 0)
		{
			GameObject gameObject2 = GameObject.Find(PlayerControll.Player[id].go.name + "/player_warcorp");
			SkinnedMeshRenderer component2 = gameObject2.GetComponent<SkinnedMeshRenderer>();
			Texture2D textureByName2 = TEX.GetTextureByName("_" + MenuShop.shopdata[mask_warcorp].iconname);
			component2.materials[0].SetTexture(0, textureByName2);
		}
	}

	public static void SetPlayerCustom(int id, int[] recvCustomWeapon)
	{
		if (PlayerControll.Player[id] == null)
		{
			return;
		}
		for (int i = 0; i < 128; i++)
		{
			PlayerControll.Player[id].customWeapon[i] = recvCustomWeapon[i];
		}
	}

	public static void SetPlayerCurrent(int id, int ak47, int aks74u, int asval, int aug, int awp, int beretta, int bm4, int colt, int deagle, int famas, int glock17, int m4a1, int m24, int m90, int m110, int m249, int mp5, int mp7, int p90, int pkp, int qbz95, int remington, int spas12, int svd, int ump45)
	{
	}

	public static void ResetCurrent(int id)
	{
		if (PlayerControll.Player[id] == null)
		{
			return;
		}
		for (int i = 0; i < 128; i++)
		{
			PlayerControll.Player[id].currentWeapon[i] = PlayerControll.Player[id].customWeapon[i];
		}
	}

	public static GameObject CreatePlayerRD(int id, int hitzone, int aid, bool deathRD = false)
	{
		if (PlayerControll.Player[id] == null)
		{
			return null;
		}
		int num = PlayerControll.Player[id].Team;
		if (deathRD)
		{
			num = 1;
		}
		Vector3 position = PlayerControll.Player[id].currPos;
		Vector3 euler = PlayerControll.Player[id].rotation;
		if (PlayerControll.Player[id].go != null)
		{
			position = PlayerControll.Player[id].go.transform.position;
			euler = PlayerControll.Player[id].go.transform.eulerAngles;
		}
		if (Client.ID == id)
		{
			GameObject gameObject = GameObject.Find("LocalPlayer");
			position = gameObject.transform.position;
			euler = gameObject.transform.eulerAngles;
		}
		else
		{
			if (PlayerControll.nord)
			{
				return null;
			}
			if (PlayerControll.Player[id].go == null)
			{
				return null;
			}
			if (!PlayerControll.Player[id].go.activeSelf)
			{
				return null;
			}
			if (!PlayerControll.vp[id] && !SpecCam.show)
			{
				return null;
			}
		}
		GameObject gameObject2 = new GameObject();
		if (ScoreBoard.gamemode == 3 && num == 0)
		{
			gameObject2 = (UnityEngine.Object.Instantiate(PlayerControll.pgoZombieRD, position, Quaternion.Euler(euler)) as GameObject);
		}
		else
		{
			gameObject2 = (UnityEngine.Object.Instantiate(PlayerControll.pgoPlayerRD, position, Quaternion.Euler(euler)) as GameObject);
		}
		gameObject2.name = "ragdoll_" + Time.time.ToString();
		Animator component = gameObject2.GetComponent<Animator>();
		if (Client.ID == id)
		{
			gameObject2.GetComponent<DelayDestroy>().destroytime = 6f;
		}
		if (Client.ID != id)
		{
			Transform[] componentsInChildren = gameObject2.GetComponentsInChildren<Transform>();
			Transform[] componentsInChildren2 = PlayerControll.Player[id].go.GetComponentsInChildren<Transform>();
			Transform[] array = componentsInChildren2;
			for (int i = 0; i < array.Length; i++)
			{
				Transform transform = array[i];
				Transform[] array2 = componentsInChildren;
				for (int j = 0; j < array2.Length; j++)
				{
					Transform transform2 = array2[j];
					if (transform.name == transform2.name)
					{
						transform2.transform.position = transform.transform.position;
						transform2.transform.rotation = transform.transform.rotation;
						transform2.gameObject.layer = 23;
						break;
					}
				}
			}
		}
		else
		{
			Transform[] componentsInChildren3 = gameObject2.GetComponentsInChildren<Transform>();
			Transform[] array3 = componentsInChildren3;
			for (int k = 0; k < array3.Length; k++)
			{
				Transform transform3 = array3[k];
				transform3.gameObject.layer = 23;
			}
		}
		GameObject gameObject3 = GameObject.Find(gameObject2.name + "/Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 Head");
		GameObject gameObject4 = GameObject.Find(gameObject2.name + "/Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1");
		if (ScoreBoard.gamemode == 3 && num == 0)
		{
			gameObject3 = GameObject.Find(gameObject2.name + "/Hip/Spine_1/Spine_2/Spine_3/Neck/Head");
			gameObject4 = GameObject.Find(gameObject2.name + "/Hip/Spine_1/Spine_2");
		}
		if (Client.ID != id)
		{
			Rigidbody[] componentsInChildren4 = gameObject2.GetComponentsInChildren<Rigidbody>();
			for (int l = 0; l < componentsInChildren4.Length; l++)
			{
				Rigidbody rigidbody = componentsInChildren4[l];
				rigidbody.isKinematic = true;
			}
		}
		LightmapModel lightmapModel = null;
		if (Options.xLightmapShadow == 1)
		{
			lightmapModel = gameObject2.AddComponent<LightmapModel>();
		}
		if (num == 0)
		{
			if (ScoreBoard.gamemode == 3)
			{
				if (lightmapModel)
				{
					lightmapModel.go = GameObject.Find(gameObject2.name + "/player_zombie");
				}
			}
			else
			{
				GameObject.Find(gameObject2.name + "/player_warcorp").SetActive(false);
				if (lightmapModel)
				{
					lightmapModel.go = GameObject.Find(gameObject2.name + "/player_merc");
				}
			}
		}
		else if (num == 1)
		{
			GameObject.Find(gameObject2.name + "/player_merc").SetActive(false);
			if (lightmapModel)
			{
				lightmapModel.go = GameObject.Find(gameObject2.name + "/player_warcorp");
			}
		}
		if (ScoreBoard.gamemode != 3 || num != 0)
		{
			if (num == 0 && PlayerControll.Player[id].mask_merc != 0)
			{
				GameObject gameObject5 = GameObject.Find(gameObject2.name + "/player_merc");
				SkinnedMeshRenderer component2 = gameObject5.GetComponent<SkinnedMeshRenderer>();
				Texture2D textureByName = TEX.GetTextureByName("_" + MenuShop.shopdata[PlayerControll.Player[id].mask_merc].iconname);
				component2.materials[0].SetTexture(0, textureByName);
			}
			else if (num == 1 && PlayerControll.Player[id].mask_warcorp != 0)
			{
				GameObject gameObject6 = GameObject.Find(gameObject2.name + "/player_warcorp");
				SkinnedMeshRenderer component3 = gameObject6.GetComponent<SkinnedMeshRenderer>();
				Texture2D textureByName2 = TEX.GetTextureByName("_" + MenuShop.shopdata[PlayerControll.Player[id].mask_warcorp].iconname);
				component3.materials[0].SetTexture(0, textureByName2);
			}
		}
		if (PlayerControll.Player[id].jump == 1)
		{
			component.enabled = false;
		}
		else if (PlayerControll.Player[id].duck == 1)
		{
			component.SetInteger("death", 11);
		}
		else if (hitzone == 1)
		{
			component.SetInteger("death", 10);
		}
		else
		{
			component.SetInteger("death", UnityEngine.Random.Range(1, 6));
		}
		return gameObject2;
	}

	public static void DestroyPlayer(int id)
	{
		if (PlayerControll.Player[id] != null)
		{
			if (PlayerControll.Player[id].go)
			{
				PlayerControll.Player[id].go.name = "dead_" + Time.time.ToString();
				UnityEngine.Object.Destroy(PlayerControll.Player[id].go);
			}
			if (PlayerControll.Player[id].gosWeapon)
			{
				PlayerControll.Player[id].gosWeapon.name = "dead_sound_weapon_" + Time.time.ToString();
				UnityEngine.Object.Destroy(PlayerControll.Player[id].gosWeapon);
			}
			if (PlayerControll.Player[id].gosStep)
			{
				PlayerControll.Player[id].gosStep.name = "dead_sound_step_" + Time.time.ToString();
				UnityEngine.Object.Destroy(PlayerControll.Player[id].gosStep);
			}
			PlayerControll.Player[id] = null;
		}
	}

	public static void UpdatePosition(int id, float pX, float pY, float pZ, float rX, float rY, int state, int zoom, float animSpeed, float animStrafe)
	{
		if (Client.ID == id)
		{
			return;
		}
		if (PlayerControll.Player[id] == null)
		{
			return;
		}
		PlayerControll.Player[id].oldpos = PlayerControll.Player[id].position;
		PlayerControll.Player[id].position = new Vector3(pX, pY, pZ);
		PlayerControll.Player[id].rotation = new Vector3(rX, rY, 0f);
		PlayerControll.Player[id].state = state;
		PlayerControll.Player[id].zoom = zoom;
		PlayerControll.Player[id].animSpeed = animSpeed;
		PlayerControll.Player[id].animStrafe = animStrafe;
		if (state != 1)
		{
			if (state != 2)
			{
				if (state == 4)
				{
					PlayerControll.Player[id].state = 0;
					PlayerControll.Player[id].duck = 1;
				}
				else if (state == 5)
				{
					PlayerControll.Player[id].state = 1;
					PlayerControll.Player[id].duck = 1;
				}
				else
				{
					PlayerControll.Player[id].duck = 0;
				}
			}
		}
		if (PlayerControll.Player[id].collider != null)
		{
			float num = 1f;
			float y = 0.75f;
			if (ScoreBoard.gamemode == 3 && PlayerControll.Player[id].Team == 0)
			{
				num = 0.04651163f;
				y = 0.0348837227f;
			}
			if (state == 4)
			{
				PlayerControll.Player[id].collider.transform.localPosition = new Vector3(0f, y, 0f);
				PlayerControll.Player[id].collider.transform.localScale = new Vector3(num, y, num);
				PlayerControll.Player[id].colliderTrigger.transform.localPosition = new Vector3(0f, y, 0f);
				PlayerControll.Player[id].colliderTrigger.transform.localScale = new Vector3(num, y, num);
			}
			else
			{
				PlayerControll.Player[id].collider.transform.localPosition = new Vector3(0f, num, 0f);
				PlayerControll.Player[id].collider.transform.localScale = new Vector3(num, num, num);
				PlayerControll.Player[id].colliderTrigger.transform.localPosition = new Vector3(0f, num, 0f);
				PlayerControll.Player[id].colliderTrigger.transform.localScale = new Vector3(num, num, num);
			}
		}
		PlayerControll.UpdateState(id);
		float num2 = 0f;
		if (rX >= 270f && rX <= 360f)
		{
			num2 = (360f - rX) / 90f;
		}
		else if (rX >= 0f && rX <= 90f)
		{
			num2 = (0f - rX) / 90f;
		}
		num2 *= 2f;
		if (num2 > 1f)
		{
			num2 = 1f;
		}
		if (num2 < -1f)
		{
			num2 = -1f;
		}
		float num3 = rY;
		float num4 = PlayerControll.Player[id].oldy - num3;
		if (num4 > 180f)
		{
			num3 += 360f;
		}
		if (num4 < -180f)
		{
			num3 -= 360f;
		}
		if (Mathf.Abs(PlayerControll.Player[id].oldy - num3) > 60f)
		{
			if (PlayerControll.Player[id].animator && PlayerControll.Player[id].animator.isActiveAndEnabled && PlayerControll.Player[id].state == 0)
			{
				if (PlayerControll.Player[id].oldy >= num3)
				{
					PlayerControll.Player[id].animator.SetInteger("turn", 1);
					PlayerControll.Player[id].turn = 1;
				}
				else
				{
					PlayerControll.Player[id].animator.SetInteger("turn", 2);
					PlayerControll.Player[id].turn = 2;
				}
				PlayerControll.Player[id].turntime = Time.time;
			}
			PlayerControll.Player[id].oldy = num3;
		}
		if (PlayerControll.Player[id].state != 0 && PlayerControll.Player[id].state != 4 && PlayerControll.Player[id].turn > 0)
		{
			if (PlayerControll.Player[id].animator && PlayerControll.Player[id].animator.isActiveAndEnabled)
			{
				PlayerControll.Player[id].animator.SetInteger("turn", 0);
			}
			PlayerControll.Player[id].turn = 0;
			PlayerControll.Player[id].turntime = 0f;
		}
		float num5 = PlayerControll.Player[id].oldy - num3;
		float ay = num5 / -30f;
		PlayerControll.Player[id].ax = num2 * -1f;
		PlayerControll.Player[id].ay = ay;
		if (state > 0)
		{
			if (PlayerControll.Player[id].position == PlayerControll.Player[id].oldpos)
			{
				return;
			}
			PlayerControll.Player[id].oldy = rY;
			Vector3 vector = PlayerControll.Player[id].position - PlayerControll.Player[id].oldpos;
			Vector3 move = Quaternion.Euler(0f, rY * -1f, 0f) * vector.normalized;
			float num6 = 0.65f;
			if (state == 3)
			{
				num6 = 0.25f;
			}
			if (move.x > num6)
			{
				move.x = 1f;
			}
			else if (move.x < -num6)
			{
				move.x = -1f;
			}
			else
			{
				move.x = 0f;
			}
			if (move.z > num6)
			{
				move.z = 1f;
			}
			else if (move.z < -num6)
			{
				move.z = -1f;
			}
			else
			{
				move.z = 0f;
			}
			PlayerControll.Player[id].move = move;
		}
		if (PlayerControll.Player[id].go != null && PlayerControll.Player[id].asSteps != null && PlayerControll.Player[id].asSteps.enabled)
		{
			if (PlayerControll.Player[id].asSteps.isPlaying)
			{
				return;
			}
			if (state == 0)
			{
				return;
			}
			PlayerControll.Player[id].gosStep.transform.position = PlayerControll.Player[id].currPos;
			if (Time.time > PlayerControll.Player[id].steps_time)
			{
				if (state == 3)
				{
					PlayerControll.Player[id].asSteps.volume = 0.5f * Options.gamevol;
					PlayerControll.Player[id].steps_time = Time.time + 0.25f + UnityEngine.Random.Range(0f, 0.025f);
					int num7 = UnityEngine.Random.Range(0, 3);
					if (num7 == 0)
					{
						num7 = 3;
					}
					PlayerControll.Player[id].asSteps.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
					if (PlayerControll.steps_run_gravel[num7] != null)
					{
						PlayerControll.Player[id].asSteps.PlayOneShot(PlayerControll.steps_run_gravel[num7]);
					}
				}
				else if (state == 2)
				{
					PlayerControll.Player[id].asSteps.volume = 0.5f * Options.gamevol;
					PlayerControll.Player[id].steps_time = Time.time + 0.38f + UnityEngine.Random.Range(0f, 0.05f);
					int num8 = UnityEngine.Random.Range(0, 3);
					if (num8 == 0)
					{
						num8 = 3;
					}
					PlayerControll.Player[id].asSteps.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
					if (PlayerControll.steps_walk_gravel[num8] != null)
					{
						PlayerControll.Player[id].asSteps.PlayOneShot(PlayerControll.steps_walk_gravel[num8]);
					}
				}
			}
		}
	}

	public static void PlayZombieAttack(int id)
	{
		if (PlayerControll.Player[id] == null)
		{
			return;
		}
		if (PlayerControll.Player[id].go == null)
		{
			return;
		}
		if (PlayerControll.Player[id].animator == null)
		{
			return;
		}
		if (UnityEngine.Random.Range(0, 2) == 0)
		{
			PlayerControll.Player[id].animator.SetBool("Attack1", true);
		}
		else
		{
			PlayerControll.Player[id].animator.SetBool("Attack2", true);
		}
		PlayerControll.Player[id].gosWeapon.transform.position = PlayerControll.Player[id].currPos;
		if (PlayerControll.Player[id].asWeapon.enabled)
		{
			PlayerControll.Player[id].asWeapon.PlayOneShot(Zombie.attackSound);
		}
	}

	public static void PlayFire(int id)
	{
		if (PlayerControll.Player[id] == null)
		{
			return;
		}
		if (PlayerControll.Player[id].go == null)
		{
			return;
		}
		if (SpecCam.Fire(id))
		{
			return;
		}
		int currweapon = PlayerControll.Player[id].currweapon;
		if (ScoreBoard.gamemode == 3 && PlayerControll.Player[id].Team == 0 && currweapon == 31)
		{
			PlayerControll.PlayZombieAttack(id);
			return;
		}
		if (WeaponData.GetData(currweapon).fire1 && PlayerControll.Player[id].asWeapon.enabled)
		{
			PlayerControll.Player[id].gosWeapon.transform.position = PlayerControll.Player[id].currPos;
			PlayerControll.Player[id].asWeapon.PlayOneShot(WeaponData.GetData(currweapon).fire1);
		}
		if (PlayerControll.Player[id].go.activeSelf && PlayerControll.Player[id].recoil)
		{
			PlayerControll.Player[id].recoil.Fire(1f);
		}
		if (PlayerControll.m_TracePrefab != null && currweapon != 26 && PlayerControll.Player[id].goCurrWeapon)
		{
			Vector3 vector;
			Vector3 normalized;
			if (PlayerControll.Player[id].go.activeSelf)
			{
				Vector3 a = PlayerControll.Player[id].goAim.transform.position;
				vector = PlayerControll.Player[id].goCurrWeapon.transform.position;
				normalized = (a - vector).normalized;
				if (currweapon != 8)
				{
					Vector3 a2 = PlayerControll.Player[id].currPos + Quaternion.Euler(PlayerControll.Player[id].rotation) * Vector3.forward * 50f;
					Vector3 vector2 = PlayerControll.Player[id].currPos + Quaternion.Euler(PlayerControll.Player[id].rotation) * Vector3.forward + new Vector3(0f, 0.2f, 0f);
					Vector3 normalized2 = (a2 - vector2).normalized;
					Vector3 worldPosition = vector2 + normalized * 200f;
					GameObject gameObject;
					if (WeaponData.GetData(currweapon).weaponClass == 0)
					{
						gameObject = (UnityEngine.Object.Instantiate(PlayerControll.pgoMuzzleFlashPistol, vector, PlayerControll.Player[id].goAim.transform.rotation) as GameObject);
						gameObject.transform.LookAt(worldPosition);
					}
					else if (WeaponData.GetData(currweapon).weaponClass == 1)
					{
						gameObject = (UnityEngine.Object.Instantiate(PlayerControll.pgoMuzzleFlashShotgun, vector, PlayerControll.Player[id].goAim.transform.rotation) as GameObject);
						gameObject.transform.LookAt(worldPosition);
					}
					else if (WeaponData.GetData(currweapon).weaponClass == 5)
					{
						gameObject = (UnityEngine.Object.Instantiate(PlayerControll.pgoMuzzleFlashHeavy, vector, PlayerControll.Player[id].goAim.transform.rotation) as GameObject);
						gameObject.transform.LookAt(worldPosition);
					}
					else
					{
						gameObject = (UnityEngine.Object.Instantiate(PlayerControll.pgoMuzzleFlashRifle, vector, PlayerControll.Player[id].goAim.transform.rotation) as GameObject);
						gameObject.transform.LookAt(worldPosition);
					}
					if (currweapon == 1)
					{
						gameObject.transform.Translate(gameObject.transform.forward * 0.05f + gameObject.transform.up * 0.05f + gameObject.transform.right * -0.01f, Space.World);
					}
					else if (currweapon == 2)
					{
						gameObject.transform.Translate(gameObject.transform.forward * 0.07f + gameObject.transform.up * 0.07f + gameObject.transform.right * -0.01f, Space.World);
					}
					else if (currweapon == 3)
					{
						gameObject.transform.Translate(gameObject.transform.forward * 0.07f + gameObject.transform.up * 0.07f + gameObject.transform.right * -0.01f, Space.World);
					}
					else if (currweapon == 4)
					{
						gameObject.transform.Translate(gameObject.transform.forward * 0.07f + gameObject.transform.up * 0.07f + gameObject.transform.right * -0.01f, Space.World);
					}
					else if (currweapon == 5)
					{
						gameObject.transform.Translate(gameObject.transform.forward * 0.05f + gameObject.transform.up * 0.07f + gameObject.transform.right * -0.01f, Space.World);
					}
					else if (currweapon == 9)
					{
						gameObject.transform.Translate(gameObject.transform.forward * 0.43f + gameObject.transform.up * 0.039f + gameObject.transform.right * -0.05f, Space.World);
					}
					else if (currweapon == 16)
					{
						gameObject.transform.Translate(gameObject.transform.forward * 0.64f + gameObject.transform.up * 0.07f + gameObject.transform.right * -1.25f, Space.World);
					}
					else if (currweapon == 12)
					{
						gameObject.transform.Translate(gameObject.transform.forward * 0.46f + gameObject.transform.up * 0.05f + gameObject.transform.right * -0.12f, Space.World);
					}
					else if (currweapon == 13)
					{
						gameObject.transform.Translate(gameObject.transform.forward * 0.36f + gameObject.transform.up * 0.065f + gameObject.transform.right * -0.05f, Space.World);
					}
					else if (currweapon == 14)
					{
						gameObject.transform.Translate(gameObject.transform.forward * 0.42f + gameObject.transform.up * 0.039f + gameObject.transform.right * -0.14f, Space.World);
					}
					else if (currweapon == 17)
					{
						gameObject.transform.Translate(gameObject.transform.forward * 0.36f + gameObject.transform.up * 0.08f + gameObject.transform.right * -0.1f, Space.World);
					}
					else if (currweapon == 6)
					{
						gameObject.transform.Translate(gameObject.transform.forward * 0.65f + gameObject.transform.up * 0.047f + gameObject.transform.right * -0.2f, Space.World);
					}
					else if (currweapon == 7)
					{
						gameObject.transform.Translate(gameObject.transform.forward * 0.58f + gameObject.transform.up * 0.065f + gameObject.transform.right * -0.2f, Space.World);
					}
					else if (currweapon == 10)
					{
						gameObject.transform.Translate(gameObject.transform.forward * 0.36f + gameObject.transform.up * 0.068f + gameObject.transform.right * -0.09f, Space.World);
					}
					else if (currweapon == 11)
					{
						gameObject.transform.Translate(gameObject.transform.forward * 0.6f + gameObject.transform.up * 0.09f + gameObject.transform.right * -0.15f, Space.World);
					}
					else if (currweapon == 15)
					{
						gameObject.transform.Translate(gameObject.transform.forward * 0.35f + gameObject.transform.up * 0.075f + gameObject.transform.right * -0.1f, Space.World);
					}
					else if (currweapon == 18)
					{
						gameObject.transform.Translate(gameObject.transform.forward * 0.3f + gameObject.transform.up * 0.07f + gameObject.transform.right * -0.1f, Space.World);
					}
					else if (currweapon == 19)
					{
						gameObject.transform.Translate(gameObject.transform.forward * 0.7f + gameObject.transform.up * 0.08f + gameObject.transform.right * -0.2f, Space.World);
					}
					else if (currweapon == 20)
					{
						gameObject.transform.Translate(gameObject.transform.forward * 0.66f + gameObject.transform.up * 0.065f + gameObject.transform.right * -0.2f, Space.World);
					}
					else if (currweapon == 21)
					{
						gameObject.transform.Translate(gameObject.transform.forward * 0.5f + gameObject.transform.up * 0.06f + gameObject.transform.right * -0.2f, Space.World);
					}
					else if (currweapon == 22)
					{
						gameObject.transform.Translate(gameObject.transform.forward * 0.75f + gameObject.transform.up * 0.06f + gameObject.transform.right * -0.2f, Space.World);
					}
					else if (currweapon == 23)
					{
						gameObject.transform.Translate(gameObject.transform.forward * 0.53f + gameObject.transform.up * 0.05f + gameObject.transform.right * -0.1f, Space.World);
					}
					else if (currweapon == 24)
					{
						gameObject.transform.Translate(gameObject.transform.forward * 0.9f + gameObject.transform.up * 0.141f + gameObject.transform.right * -0.2f, Space.World);
					}
					else if (currweapon == 25)
					{
						gameObject.transform.Translate(gameObject.transform.forward * 0.65f + gameObject.transform.up * 0.08f + gameObject.transform.right * -0.2f, Space.World);
					}
					gameObject.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
					Light light = gameObject.AddComponent<Light>();
					light.type = LightType.Point;
					light.shadows = LightShadows.None;
					light.range = 2.25f;
					light.color = PlayerControll.muzzlecolor;
					light.intensity = 2f;
					DelayDestroy delayDestroy = gameObject.AddComponent<DelayDestroy>();
					delayDestroy.destroytime = 0.035f;
				}
			}
			else
			{
				Vector3 a = PlayerControll.Player[id].currPos + Quaternion.Euler(PlayerControll.Player[id].rotation) * Vector3.forward * 50f;
				vector = PlayerControll.Player[id].currPos + Quaternion.Euler(PlayerControll.Player[id].rotation) * Vector3.forward + new Vector3(0f, 1.7f, 0f);
				normalized = (a - vector).normalized;
			}
			GameObject gameObject2 = UnityEngine.Object.Instantiate(PlayerControll.m_TracePrefab, vector, new Quaternion(0f, 0f, 0f, 0f)) as GameObject;
			LineRenderer component = gameObject2.GetComponent<LineRenderer>();
			Ray ray = new Ray(vector + normalized, normalized);
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit, 200f, vp_Layer.GetBulletBlockers()))
			{
				gameObject2.GetComponent<vp_Trace>().e = raycastHit.point;
			}
			else
			{
				gameObject2.GetComponent<vp_Trace>().e = vector + normalized * 200f;
			}
		}
	}

	public static void SetBlood(float x, float y, float z, float scale)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(PlayerControll.pgoBlood, new Vector3(x, y, z), PlayerControll.LocalPlayer.transform.rotation) as GameObject;
		gameObject.transform.localScale = new Vector3(scale + UnityEngine.Random.Range(0f, 0.2f), scale + UnityEngine.Random.Range(0f, 0.2f), 0f);
		gameObject.AddComponent<BloodRotate>();
	}

	public static void CheckVisible()
	{
		for (int i = 0; i < 16; i++)
		{
			PlayerControll.vp[i] = false;
			PlayerControll.vps[i] = false;
			if (PlayerControll.Player[i] != null)
			{
				if (!(PlayerControll.Player[i].go == null))
				{
					if (Vector3.Distance(PlayerControll.Player[i].currPos + Vector3.up * 2f, Camera.main.transform.position) < 2.5f)
					{
						PlayerControll.vp[i] = true;
					}
					else
					{
						PlayerControll.screenPos[0] = Camera.main.WorldToScreenPoint(PlayerControll.Player[i].currPos + Vector3.up * 2f + Camera.main.transform.right * -0.5f);
						PlayerControll.screenPos[1] = Camera.main.WorldToScreenPoint(PlayerControll.Player[i].currPos + Vector3.up * 2f + Camera.main.transform.right * 0.5f);
						PlayerControll.screenPos[2] = Camera.main.WorldToScreenPoint(PlayerControll.Player[i].currPos + Vector3.up * 0.02f + Camera.main.transform.right * -0.5f);
						PlayerControll.screenPos[3] = Camera.main.WorldToScreenPoint(PlayerControll.Player[i].currPos + Vector3.up * 0.02f + Camera.main.transform.right * 0.5f);
						PlayerControll.screenPos[4] = Camera.main.WorldToScreenPoint(PlayerControll.Player[i].currPos + Vector3.up * 1f + Camera.main.transform.right * -1f);
						PlayerControll.screenPos[5] = Camera.main.WorldToScreenPoint(PlayerControll.Player[i].currPos + Vector3.up * 1f + Camera.main.transform.right * 1f);
						PlayerControll.screenPos[6] = Camera.main.WorldToScreenPoint(PlayerControll.Player[i].currPos + Vector3.up * 1.7f);
						PlayerControll.screenPos[7] = Camera.main.WorldToScreenPoint(PlayerControll.Player[i].currPos + Vector3.up * 0.3f);
						PlayerControll.screenPos[8] = Camera.main.WorldToScreenPoint(PlayerControll.Player[i].currPos + Vector3.up * 3f);
						for (int j = 0; j < 9; j++)
						{
							if (PlayerControll.screenPos[j].z >= 0f)
							{
								if (PlayerControll.screenPos[j].x >= 0f && PlayerControll.screenPos[j].x <= HUD.currwidth)
								{
									if (PlayerControll.screenPos[j].y >= 0f && PlayerControll.screenPos[j].y <= HUD.currheight)
									{
										PlayerControll.vps[i] = true;
										PlayerControll.ray = Camera.main.ScreenPointToRay(PlayerControll.screenPos[j]);
										if (!Physics.Raycast(PlayerControll.ray, PlayerControll.screenPos[j].z, PlayerControll.castmask))
										{
											PlayerControll.vp[i] = true;
											break;
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}

	public static void UpdateWeapon(int id)
	{
		int currweapon = PlayerControll.Player[id].currweapon;
		if (WeaponData.GetData(currweapon).weaponClass == 0)
		{
			PlayerControll.Player[id].animator.SetInteger("w", 2);
		}
		else if (currweapon == 26)
		{
			PlayerControll.Player[id].animator.SetInteger("w", 99);
		}
		else if (currweapon == 28)
		{
			PlayerControll.Player[id].animator.SetInteger("w", 3);
		}
		else if (currweapon == 27 || currweapon == 29 || currweapon == 30)
		{
			if (PlayerControll.Player[id].fastGrenade == 1)
			{
				PlayerControll.Player[id].animator.SetInteger("w", 4);
			}
			else
			{
				PlayerControll.Player[id].animator.SetInteger("w", 8);
			}
		}
		else if (currweapon == 50)
		{
			PlayerControll.Player[id].animator.SetInteger("w", 5);
		}
		else if (currweapon == 24 || currweapon == 25 || currweapon == 16 || currweapon == 9)
		{
			PlayerControll.Player[id].animator.SetInteger("w", 1);
		}
		else
		{
			PlayerControll.Player[id].animator.SetInteger("w", 0);
		}
	}

	public static void UpdateWeaponModel(int id)
	{
		int currweapon = PlayerControll.Player[id].currweapon;
		if (PlayerControll.Player[id].bomb && currweapon != 50)
		{
			PlayerControll.SetRootBombVisible(id, true);
		}
		else
		{
			PlayerControll.SetRootBombVisible(id, false);
		}
		PlayerControll.CreateWeapon(id, currweapon);
	}

	public static void CreateWeapon(int id, int wid)
	{
		if (PlayerControll.Player[id].goCurrWeapon)
		{
			PlayerControll.Player[id].goCurrWeapon.name = "dead_weapon" + Time.time;
			UnityEngine.Object.Destroy(PlayerControll.Player[id].goCurrWeapon);
		}
		if (wid == 0)
		{
			return;
		}
		if (!WeaponData.CheckWeapon(wid))
		{
			Debug.Log("weapondata is null");
			return;
		}
		GameObject go = PlayerControll.Player[id].go;
		GameObject gameObject = ContentLoader_.LoadGameObject("p_" + WeaponData.GetData(wid).wName.ToLower());
		if (gameObject == null)
		{
			return;
		}
		PlayerControll.Player[id].goCurrWeapon = (UnityEngine.Object.Instantiate(gameObject, Vector3.zero, Quaternion.identity) as GameObject);
		Transform[] componentsInChildren = PlayerControll.Player[id].goCurrWeapon.GetComponentsInChildren<Transform>();
		Transform[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Transform transform = array[i];
			transform.gameObject.layer = 25;
		}
		PlayerControll.Player[id].goCurrWeapon.layer = 25;
		GameObject gameObject2;
		if (WeaponData.GetData(wid).hand == 0)
		{
			gameObject2 = GameObject.Find(go.name + "/Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 L Clavicle/Bip001 L UpperArm/Bip001 L Forearm/Bip001 L Hand");
		}
		else
		{
			gameObject2 = GameObject.Find(go.name + "/Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand");
		}
		PlayerControll.Player[id].goCurrWeapon.transform.parent = gameObject2.transform;
		PlayerControll.Player[id].goCurrWeapon.transform.localPosition = gameObject.transform.localPosition;
		PlayerControll.Player[id].goCurrWeapon.transform.localRotation = gameObject.transform.localRotation;
		if (ScoreBoard.gamemode == 3)
		{
			MeshRenderer component = PlayerControll.Player[id].goCurrWeapon.GetComponent<MeshRenderer>();
			ContentLoader_.ReplaceMaterialsFromRes(ref component);
		}
		if (PlayerControll.Player[id].currentWeapon[wid] > 0)
		{
			MeshRenderer component2 = PlayerControll.Player[id].goCurrWeapon.GetComponent<MeshRenderer>();
			Texture2D textureByName = TEX.GetTextureByName(MenuShop.shopdata[PlayerControll.Player[id].currentWeapon[wid]].iconname);
			if (textureByName != null)
			{
				component2.materials[0].SetTexture(0, textureByName);
			}
		}
	}

	public static void UpdateState(int id)
	{
		if (PlayerControll.Player[id].go == null)
		{
			return;
		}
		if (!PlayerControll.Player[id].go.activeSelf)
		{
			return;
		}
		if (PlayerControll.Player[id].animator == null)
		{
			return;
		}
		if (!PlayerControll.Player[id].animator.isActiveAndEnabled)
		{
			return;
		}
		if (ScoreBoard.gamemode != 3 || (ScoreBoard.gamemode == 3 && PlayerControll.Player[id].Team != 0))
		{
			PlayerControll.Player[id].animator.SetInteger("state", PlayerControll.Player[id].state);
			PlayerControll.Player[id].animator.SetInteger("duck", PlayerControll.Player[id].duck);
			PlayerControll.Player[id].animator.SetInteger("zoom", PlayerControll.Player[id].zoom);
		}
		else
		{
			bool value = PlayerControll.Player[id].duck == 1;
			PlayerControll.Player[id].animator.SetBool("Sit", value);
		}
		if (PlayerControll.Player[id].aimik)
		{
			if (PlayerControll.Player[id].state == 3)
			{
				PlayerControll.Player[id].aimik.solver.IKPositionWeight = 0f;
			}
			else
			{
				PlayerControll.Player[id].aimik.solver.IKPositionWeight = 1f;
			}
		}
	}

	public static void UpdateJump(int id)
	{
		int num = 0;
		RaycastHit raycastHit;
		if (Physics.SphereCast(new Ray(PlayerControll.Player[id].currPos + Vector3.up * 1.25f, Vector3.down), 1f, out raycastHit, 64f, 1))
		{
			if (raycastHit.distance > 1.5f)
			{
				num = 1;
			}
			else if (PlayerControll.Player[id].jump == 1 && raycastHit.distance <= 1.5f)
			{
				num = 0;
			}
		}
		if (Physics.Raycast(new Ray(PlayerControll.Player[id].currPos + Vector3.up * 0.25f, Vector3.down), out raycastHit, 64f, 16777216))
		{
			if (raycastHit.distance > 0.75f && PlayerControll.Player[id].jump == 0)
			{
				PlayerControll.Player[id].animator.speed = 0.2f;
			}
			else
			{
				PlayerControll.Player[id].animator.speed = 1f;
			}
		}
		if (PlayerControll.Player[id].jump != num)
		{
			PlayerControll.Player[id].animator.SetInteger("jump", num);
			PlayerControll.Player[id].jump = num;
		}
	}
}
