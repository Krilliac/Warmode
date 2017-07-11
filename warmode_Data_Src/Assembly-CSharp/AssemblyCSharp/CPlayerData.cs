using RootMotion.FinalIK;
using RootMotion.FinalIK.Demos;
using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public class CPlayerData
	{
		public GameObject go;

		public GameObject goName;

		public GameObject goCurrWeapon;

		public GameObject goRootBomb;

		public GameObject gosWeapon;

		public GameObject gosStep;

		public GameObject goMuzzle;

		public Animator animator;

		public AudioSource asWeapon;

		public AudioSource asSteps;

		public AimIK aimik;

		public GameObject goAim;

		public Recoil recoil;

		public HitReaction hitReaction;

		public GameObject collider;

		public GameObject colliderTrigger;

		public bool colliderFreeze;

		public bool Active;

		public int State;

		public int AnimState;

		public int DeadFlag;

		public int Team;

		public bool bomb;

		public bool defuse;

		public int zombieAlfa;

		public int Frags;

		public int Deaths;

		public int Points;

		public string sFrags;

		public string sDeaths;

		public string sPoints;

		public int nope;

		public Vector3 oldpos;

		public Vector3 position;

		public Vector3 rotation;

		public Vector3 move;

		public Vector3 currMove;

		public Vector3 currPos;

		public Vector2 animMove;

		public float cx;

		public float cy;

		public int state;

		public int zoom;

		public int duck;

		public int jump;

		public int turn;

		public float turntime;

		public float animSpeed;

		public float animStrafe;

		public float currAnimSpeed;

		public float currAnimStrafe;

		public float oldy;

		public string Name;

		public int currweapon;

		public float flashtime;

		public float steps_time;

		public int level;

		public string sLevel;

		public string ClanName;

		public int badge_back;

		public int badge_icon;

		public int mask_merc;

		public int mask_warcorp;

		public int[] customWeapon = new int[128];

		public int[] currentWeapon = new int[128];

		public int fastGrenade;

		public float ax;

		public float ay;

		public CPlayerData()
		{
			this.Active = false;
			this.AnimState = 0;
			this.State = 0;
			this.DeadFlag = 0;
			this.Name = string.Empty;
			this.ClanName = string.Empty;
			this.oldpos = Vector3.zero;
			this.currPos = Vector3.zero;
			this.position = Vector3.zero;
			this.rotation = Vector3.zero;
			this.move = Vector3.zero;
			this.currMove = Vector3.zero;
			this.animMove = Vector2.zero;
			this.go = null;
			this.gosWeapon = null;
			this.gosStep = null;
			this.goMuzzle = null;
			this.collider = null;
			this.colliderTrigger = null;
			this.colliderFreeze = false;
			this.Team = 255;
			this.SetFrags(0);
			this.SetDeaths(0);
			this.SetPoints(0);
			this.currweapon = 0;
			this.flashtime = 0f;
			this.steps_time = 0f;
			this.SetLevel(1);
			this.ax = 0f;
			this.ay = 0f;
			this.state = 0;
			this.turntime = 0f;
			this.animSpeed = 0f;
			this.animStrafe = 0f;
			this.currAnimSpeed = 0f;
			this.currAnimStrafe = 0f;
			this.bomb = false;
			this.defuse = false;
			this.goCurrWeapon = null;
			this.zombieAlfa = 0;
			this.badge_back = 0;
			this.badge_icon = 0;
			this.fastGrenade = 0;
		}

		public void SetFrags(int val)
		{
			this.Frags = val;
			this.sFrags = this.Frags.ToString();
		}

		public void SetDeaths(int val)
		{
			this.Deaths = val;
			this.sDeaths = this.Deaths.ToString();
		}

		public void SetPoints(int val)
		{
			this.Points = val;
			this.sPoints = this.Points.ToString();
		}

		public void SetFragsInc(int val)
		{
			this.Frags += val;
			this.sFrags = this.Frags.ToString();
		}

		public void SetFragsDec(int val)
		{
			this.Frags -= val;
			this.sFrags = this.Frags.ToString();
		}

		public void SetDeathsInc(int val)
		{
			this.Deaths += val;
			this.sDeaths = this.Deaths.ToString();
		}

		public void SetPointsInc(int val)
		{
			this.Points += val;
			this.sPoints = this.Points.ToString();
		}

		public void SetLevel(int val)
		{
			this.level = val;
			this.sLevel = this.level.ToString();
		}
	}
}
