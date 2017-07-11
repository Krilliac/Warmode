using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ABTest : MonoBehaviour
{
	public class CWeaponDat
	{
		public int index;

		public string texName;

		public string name;

		public string prefabName;

		public Texture2D tex;

		public int points;

		public CWeaponDat(int index, string texName, string name, string prefabName, Texture2D tex, int points)
		{
			this.index = index;
			this.texName = texName;
			this.name = name;
			this.prefabName = prefabName;
			this.tex = tex;
			this.points = points;
		}
	}

	public GameObject previewA;

	public GameObject previewB;

	public Transform mainCam;

	public Text levelText;

	public Text nameTextA;

	public Text nameTextB;

	public Dictionary<string, GameObject> weaponsA;

	public Dictionary<string, GameObject> weaponsB;

	public List<Texture2D> tex_ak47 = new List<Texture2D>();

	public List<Texture2D> tex_aks74u = new List<Texture2D>();

	public List<Texture2D> tex_asval = new List<Texture2D>();

	public List<Texture2D> tex_aug = new List<Texture2D>();

	public List<Texture2D> tex_awp = new List<Texture2D>();

	public List<Texture2D> tex_beretta = new List<Texture2D>();

	public List<Texture2D> tex_bm4 = new List<Texture2D>();

	public List<Texture2D> tex_colt = new List<Texture2D>();

	public List<Texture2D> tex_deagle = new List<Texture2D>();

	public List<Texture2D> tex_famas = new List<Texture2D>();

	public List<Texture2D> tex_glock17 = new List<Texture2D>();

	public List<Texture2D> tex_m4a1 = new List<Texture2D>();

	public List<Texture2D> tex_m24 = new List<Texture2D>();

	public List<Texture2D> tex_m90 = new List<Texture2D>();

	public List<Texture2D> tex_m110 = new List<Texture2D>();

	public List<Texture2D> tex_m249 = new List<Texture2D>();

	public List<Texture2D> tex_mp5 = new List<Texture2D>();

	public List<Texture2D> tex_mp7 = new List<Texture2D>();

	public List<Texture2D> tex_p90 = new List<Texture2D>();

	public List<Texture2D> tex_pkp = new List<Texture2D>();

	public List<Texture2D> tex_qbz95 = new List<Texture2D>();

	public List<Texture2D> tex_remington = new List<Texture2D>();

	public List<Texture2D> tex_spas12 = new List<Texture2D>();

	public List<Texture2D> tex_svd = new List<Texture2D>();

	public List<Texture2D> tex_ump45 = new List<Texture2D>();

	private List<ABTest.CWeaponDat> ak47;

	private List<ABTest.CWeaponDat> aks74u;

	private List<ABTest.CWeaponDat> asval;

	private List<ABTest.CWeaponDat> aug;

	private List<ABTest.CWeaponDat> awp;

	private List<ABTest.CWeaponDat> beretta;

	private List<ABTest.CWeaponDat> bm4;

	private List<ABTest.CWeaponDat> colt;

	private List<ABTest.CWeaponDat> deagle;

	private List<ABTest.CWeaponDat> famas;

	private List<ABTest.CWeaponDat> glock17;

	private List<ABTest.CWeaponDat> m4a1;

	private List<ABTest.CWeaponDat> m24;

	private List<ABTest.CWeaponDat> m90;

	private List<ABTest.CWeaponDat> m110;

	private List<ABTest.CWeaponDat> m249;

	private List<ABTest.CWeaponDat> mp5;

	private List<ABTest.CWeaponDat> mp7;

	private List<ABTest.CWeaponDat> p90;

	private List<ABTest.CWeaponDat> pkp;

	private List<ABTest.CWeaponDat> qbz95;

	private List<ABTest.CWeaponDat> remington;

	private List<ABTest.CWeaponDat> spas12;

	private List<ABTest.CWeaponDat> svd;

	private List<ABTest.CWeaponDat> ump45;

	private List<ABTest.CWeaponDat> tempWeaponList;

	private Dictionary<string, Vector3> initRotate;

	private Vector3 mainCamPos;

	private int lastWeaponId = -1;

	private int lastSelectedWId = -1;

	private int variant = -1;

	private void Start()
	{
		this.mainCamPos = this.mainCam.position;
		this.weaponsA = new Dictionary<string, GameObject>();
		this.initRotate = new Dictionary<string, Vector3>();
		foreach (Transform transform in this.previewA.transform)
		{
			this.weaponsA.Add(transform.name, transform.gameObject);
			this.initRotate.Add(transform.name, transform.gameObject.transform.eulerAngles);
			RotateModelAB rotateModelAB = transform.gameObject.AddComponent<RotateModelAB>();
			rotateModelAB.fullrotate = true;
		}
		this.weaponsB = new Dictionary<string, GameObject>();
		foreach (Transform transform2 in this.previewB.transform)
		{
			this.weaponsB.Add(transform2.name, transform2.gameObject);
			RotateModelAB rotateModelAB2 = transform2.gameObject.AddComponent<RotateModelAB>();
			rotateModelAB2.fullrotate = true;
		}
		this.WeaponShow(this.weaponsA, string.Empty, null);
		this.WeaponShow(this.weaponsB, string.Empty, null);
		this.ak47 = new List<ABTest.CWeaponDat>();
		this.ak47.Add(new ABTest.CWeaponDat(0, "ak47_art_colorista", "AK47 COLORISTA", "w_ak47", this.tex_ak47[0], -1));
		this.ak47.Add(new ABTest.CWeaponDat(1, "ak47_art_hexagon", "AK47 HEXAGON", "w_ak47", this.tex_ak47[1], -1));
		this.ak47.Add(new ABTest.CWeaponDat(2, "ak47_art_romb", "AK47 RAINBOW ROMB", "w_ak47", this.tex_ak47[2], -1));
		this.ak47.Add(new ABTest.CWeaponDat(3, "ak47_art_rust", "AK47 ORANGE RUST", "w_ak47", this.tex_ak47[3], -1));
		this.ak47.Add(new ABTest.CWeaponDat(4, "ak47_art_splat", "AK47 COLORFUL SPLAT", "w_ak47", this.tex_ak47[4], -1));
		this.ak47.Add(new ABTest.CWeaponDat(5, "ak47_camo_digitaldesert", "AK47 DIGITAL DESERT", "w_ak47", this.tex_ak47[5], -1));
		this.ak47.Add(new ABTest.CWeaponDat(6, "ak47_camo_multicam", "AK47 MODERN MULTICAM", "w_ak47", this.tex_ak47[6], -1));
		this.ak47.Add(new ABTest.CWeaponDat(7, "ak47_color_od", "AK47 OD", "w_ak47", this.tex_ak47[7], -1));
		this.ak47.Add(new ABTest.CWeaponDat(8, "ak47_color_tan", "AK47 TAN", "w_ak47", this.tex_ak47[8], -1));
		this.ak47.Add(new ABTest.CWeaponDat(9, "ak47_color_white", "AK47 WHITE", "w_ak47", this.tex_ak47[9], -1));
		this.ak47.Add(new ABTest.CWeaponDat(10, "ak47_art_pinkiecat", "AK47 PINKIE CAT", "w_ak47", this.tex_ak47[10], -1));
		this.ak47.Add(new ABTest.CWeaponDat(11, "ak47_camo_junglesquare", "AK47 JUNGLE SQUARE", "w_ak47", this.tex_ak47[11], -1));
		this.asval = new List<ABTest.CWeaponDat>();
		this.asval.Add(new ABTest.CWeaponDat(0, "asval_art_colorfulsplat", "ASVAL COLORFUL SPLAT", "w_asval", this.tex_asval[0], -1));
		this.asval.Add(new ABTest.CWeaponDat(1, "asval_art_greenmonster", "ASVAL GREEN MONSTER", "w_asval", this.tex_asval[1], -1));
		this.asval.Add(new ABTest.CWeaponDat(2, "asval_art_hexagon", "ASVAL HEXAGON", "w_asval", this.tex_asval[2], -1));
		this.asval.Add(new ABTest.CWeaponDat(3, "asval_art_orangerust", "ASVAL ORANGE RUST", "w_asval", this.tex_asval[3], -1));
		this.asval.Add(new ABTest.CWeaponDat(4, "asval_art_rainbowromb", "ASVAL RAINBOW ROMB", "w_asval", this.tex_asval[4], -1));
		this.asval.Add(new ABTest.CWeaponDat(5, "asval_camo_desertdigital", "ASVAL DIGITAL DESERT", "w_asval", this.tex_asval[5], -1));
		this.asval.Add(new ABTest.CWeaponDat(6, "asval_camo_krypteksnake", "ASVAL KRYPTEK SNAKE", "w_asval", this.tex_asval[6], -1));
		this.asval.Add(new ABTest.CWeaponDat(7, "asval_camo_modernmulticam", "ASVAL MODERN MULTICAM", "w_asval", this.tex_asval[7], -1));
		this.awp = new List<ABTest.CWeaponDat>();
		this.awp.Add(new ABTest.CWeaponDat(0, "awp_art_hexagon", "AWP HEXAGON", "w_awp", this.tex_awp[0], -1));
		this.awp.Add(new ABTest.CWeaponDat(1, "awp_art_romb", "AWP RAINBOW ROMB", "w_awp", this.tex_awp[1], -1));
		this.awp.Add(new ABTest.CWeaponDat(2, "awp_art_rust", "AWP ORANGE RUST", "w_awp", this.tex_awp[2], -1));
		this.awp.Add(new ABTest.CWeaponDat(3, "awp_art_splat", "AWP COLORFUL SPLAT", "w_awp", this.tex_awp[3], -1));
		this.awp.Add(new ABTest.CWeaponDat(4, "awp_camo_desertdigital", "AWP DIGITAL DESERT", "w_awp", this.tex_awp[4], -1));
		this.awp.Add(new ABTest.CWeaponDat(5, "awp_camo_kryptek", "AWP KRYPTEK SNAKE", "w_awp", this.tex_awp[5], -1));
		this.awp.Add(new ABTest.CWeaponDat(6, "awp_camo_multicam", "AWP MODERN MULTICAM", "w_awp", this.tex_awp[6], -1));
		this.awp.Add(new ABTest.CWeaponDat(7, "awp_color_cyan", "AWP CYAN", "w_awp", this.tex_awp[7], -1));
		this.awp.Add(new ABTest.CWeaponDat(8, "awp_color_pink", "AWP PINK", "w_awp", this.tex_awp[8], -1));
		this.awp.Add(new ABTest.CWeaponDat(9, "awp_color_tan", "AWP TAN", "w_awp", this.tex_awp[9], -1));
		this.awp.Add(new ABTest.CWeaponDat(10, "awp_color_white", "AWP WHITE", "w_awp", this.tex_awp[10], -1));
		this.awp.Add(new ABTest.CWeaponDat(11, "awp_color_od", "AWP OD", "w_awp", this.tex_awp[11], -1));
		this.glock17 = new List<ABTest.CWeaponDat>();
		this.glock17.Add(new ABTest.CWeaponDat(0, "glock17_camo_badboy", "GLOCK17 BADBOY", "w_glock17", this.tex_glock17[0], -1));
		this.glock17.Add(new ABTest.CWeaponDat(1, "glock17_color_od", "GLOCK17 OD", "w_glock17", this.tex_glock17[1], -1));
		this.glock17.Add(new ABTest.CWeaponDat(2, "glock17_color_tan", "GLOCK17 TAN", "w_glock17", this.tex_glock17[2], -1));
		this.glock17.Add(new ABTest.CWeaponDat(3, "glock17_camo_carbon", "GLOCK17 CARBON", "w_glock17", this.tex_glock17[3], -1));
		this.glock17.Add(new ABTest.CWeaponDat(4, "glock17_camo_digital", "GLOCK17 DIGITAL DESERT", "w_glock17", this.tex_glock17[4], -1));
		this.glock17.Add(new ABTest.CWeaponDat(5, "glock17_art_moneyback", "GLOCK17 MONEYBACK", "w_glock17", this.tex_glock17[5], -1));
		this.glock17.Add(new ABTest.CWeaponDat(6, "glock17_art_skullornament", "GLOCK17 SKULL ORNAMENT", "w_glock17", this.tex_glock17[6], -1));
		this.aks74u = new List<ABTest.CWeaponDat>();
		this.aks74u.Add(new ABTest.CWeaponDat(0, "aks74u_camo_01", "AKS-74U FOREST", "w_aks74u", this.tex_aks74u[0], -1));
		this.aks74u.Add(new ABTest.CWeaponDat(1, "aks74u_camo_02", "AKS-74U JUNGLE SQUARE", "w_aks74u", this.tex_aks74u[1], -1));
		this.aks74u.Add(new ABTest.CWeaponDat(2, "aks74u_pops_01", "AKS-74U BLACK", "w_aks74u", this.tex_aks74u[2], -1));
		this.aks74u.Add(new ABTest.CWeaponDat(3, "aks74u_pops_02", "AKS-74U SKULL ORNAMENT", "w_aks74u", this.tex_aks74u[3], -1));
		this.aug = new List<ABTest.CWeaponDat>();
		this.aug.Add(new ABTest.CWeaponDat(0, "aug_camo_01", "AUG HEXAGON", "w_aug", this.tex_aug[0], -1));
		this.aug.Add(new ABTest.CWeaponDat(1, "aug_camo_02", "AUG DIGITAL DESERT", "w_aug", this.tex_aug[1], -1));
		this.aug.Add(new ABTest.CWeaponDat(2, "aug_pop_01", "AUG BLACK", "w_aug", this.tex_aug[2], -1));
		this.beretta = new List<ABTest.CWeaponDat>();
		this.beretta.Add(new ABTest.CWeaponDat(0, "beretta_camo_01", "BERETTA HEXAGON", "w_beretta", this.tex_beretta[0], -1));
		this.beretta.Add(new ABTest.CWeaponDat(1, "beretta_pops_01", "BERETTA TAN", "w_beretta", this.tex_beretta[1], -1));
		this.beretta.Add(new ABTest.CWeaponDat(2, "beretta_pops_02", "BERETTA MONEYBACK", "w_beretta", this.tex_beretta[2], -1));
		this.bm4 = new List<ABTest.CWeaponDat>();
		this.bm4.Add(new ABTest.CWeaponDat(0, "bm4_camo_01", "BENELLI M4 DIGITAL DESERT", "w_bm4", this.tex_bm4[0], -1));
		this.bm4.Add(new ABTest.CWeaponDat(1, "bm4_camo_02", "BENELLI M4 KRYPTEK SNAKE", "w_bm4", this.tex_bm4[1], -1));
		this.bm4.Add(new ABTest.CWeaponDat(2, "bm4_pop_01", "BENELLI M4 MONEYBACK", "w_bm4", this.tex_bm4[2], -1));
		this.colt = new List<ABTest.CWeaponDat>();
		this.colt.Add(new ABTest.CWeaponDat(0, "colt_camo_01", "COLT SSP JUNGLE SQUARE", "w_colt", this.tex_colt[0], -1));
		this.colt.Add(new ABTest.CWeaponDat(1, "colt_pop_01", "COLT SSP TAN", "w_colt", this.tex_colt[1], -1));
		this.colt.Add(new ABTest.CWeaponDat(2, "colt_pop_02", "COLT SSP BLACK", "w_colt", this.tex_colt[2], -1));
		this.deagle = new List<ABTest.CWeaponDat>();
		this.deagle.Add(new ABTest.CWeaponDat(0, "deagle_color_black", "DESERT EAGLE BLACK", "w_deagle", this.tex_deagle[0], -1));
		this.deagle.Add(new ABTest.CWeaponDat(1, "deagle_color_blacktan", "DESERT EAGLE TAN", "w_deagle", this.tex_deagle[1], -1));
		this.deagle.Add(new ABTest.CWeaponDat(2, "deagle_camo_01", "DESERT EAGLE JUNGLE SQUARE", "w_deagle", this.tex_deagle[2], -1));
		this.deagle.Add(new ABTest.CWeaponDat(3, "deagle_camo_02", "DESERT EAGLE HEXAGON", "w_deagle", this.tex_deagle[3], -1));
		this.famas = new List<ABTest.CWeaponDat>();
		this.famas.Add(new ABTest.CWeaponDat(0, "famas_camo_01", "FAMAS DIGITAL DESERT", "w_famas", this.tex_famas[0], -1));
		this.famas.Add(new ABTest.CWeaponDat(1, "famas_camo_02", "FAMAS KRYPTEK SNAKE", "w_famas", this.tex_famas[1], -1));
		this.famas.Add(new ABTest.CWeaponDat(2, "famas_pop_01", "FAMAS MONEYBACK", "w_famas", this.tex_famas[2], -1));
		this.famas.Add(new ABTest.CWeaponDat(3, "famas_pop_02", "FAMAS COLORISTA", "w_famas", this.tex_famas[3], -1));
		this.m4a1 = new List<ABTest.CWeaponDat>();
		this.m4a1.Add(new ABTest.CWeaponDat(0, "m4a1_camo_01", "M4A1 DIGITAL DESERT", "w_m4a1", this.tex_m4a1[0], -1));
		this.m4a1.Add(new ABTest.CWeaponDat(1, "m4a1_camo_02", "M4A1 MODERN MULTICAM", "w_m4a1", this.tex_m4a1[1], -1));
		this.m4a1.Add(new ABTest.CWeaponDat(2, "m4a1_pop_01", "M4A1 COLORISTA", "w_m4a1", this.tex_m4a1[2], -1));
		this.m4a1.Add(new ABTest.CWeaponDat(3, "m4a1_pop_02", "M4A1 MONEYBACK", "w_m4a1", this.tex_m4a1[3], -1));
		this.m24 = new List<ABTest.CWeaponDat>();
		this.m24.Add(new ABTest.CWeaponDat(0, "m24_camo_01", "M24 JUNGLE SQUARE", "w_m24", this.tex_m24[0], -1));
		this.m24.Add(new ABTest.CWeaponDat(1, "m24_camo_02", "M24 DIGITAL DESERT", "w_m24", this.tex_m24[1], -1));
		this.m24.Add(new ABTest.CWeaponDat(2, "m24_pop_01", "M24 MONEYBACK", "w_m24", this.tex_m24[2], -1));
		this.m90 = new List<ABTest.CWeaponDat>();
		this.m90.Add(new ABTest.CWeaponDat(0, "m90_camo_01", "M90 DIGITAL DESERT", "w_m90", this.tex_m90[0], -1));
		this.m90.Add(new ABTest.CWeaponDat(1, "m90_camo_02", "M90 JUNGLE SQUARE", "w_m90", this.tex_m90[1], -1));
		this.m90.Add(new ABTest.CWeaponDat(2, "m90_pop_01", "M90 SKULL ORNAMENT", "w_m90", this.tex_m90[2], -1));
		this.m90.Add(new ABTest.CWeaponDat(3, "m90_pop_02", "M90 RAINBOW ROMB", "w_m90", this.tex_m90[3], -1));
		this.m110 = new List<ABTest.CWeaponDat>();
		this.m110.Add(new ABTest.CWeaponDat(0, "m110_camo_01", "M110 DIGITAL DESERT", "w_m110", this.tex_m110[0], -1));
		this.m110.Add(new ABTest.CWeaponDat(1, "m110_camo_02", "M110 HEXAGON", "w_m110", this.tex_m110[1], -1));
		this.m110.Add(new ABTest.CWeaponDat(2, "m110_pop_01", "M110 SKULL ORNAMENT", "w_m110", this.tex_m110[2], -1));
		this.m249 = new List<ABTest.CWeaponDat>();
		this.m249.Add(new ABTest.CWeaponDat(0, "m249_camo_01", "M249 HEXAGON", "w_m249", this.tex_m249[0], -1));
		this.m249.Add(new ABTest.CWeaponDat(1, "m249_camo_02", "M249 KRYPTEK SNAKE", "w_m249", this.tex_m249[1], -1));
		this.m249.Add(new ABTest.CWeaponDat(2, "m249_pop_01", "M249 MONEYBACK", "w_m249", this.tex_m249[2], -1));
		this.mp5 = new List<ABTest.CWeaponDat>();
		this.mp5.Add(new ABTest.CWeaponDat(0, "mp5_camo_01", "MP5 KRYPTEK SNAKE", "w_mp5", this.tex_mp5[0], -1));
		this.mp5.Add(new ABTest.CWeaponDat(1, "mp5_camo_02", "MP5 MODERN MULTICAM", "w_mp5", this.tex_mp5[1], -1));
		this.mp5.Add(new ABTest.CWeaponDat(2, "mp5_pop_01", "MP5 HEXAGON", "w_mp5", this.tex_mp5[2], -1));
		this.mp5.Add(new ABTest.CWeaponDat(3, "mp5_pop_02", "MP5 MONEYBACK", "w_mp5", this.tex_mp5[3], -1));
		this.mp7 = new List<ABTest.CWeaponDat>();
		this.mp7.Add(new ABTest.CWeaponDat(0, "mp7_camo_01", "MP7 DIGITAL DESERT", "w_mp7", this.tex_mp7[0], -1));
		this.mp7.Add(new ABTest.CWeaponDat(1, "mp7_camo_02", "MP7 HEXAGON", "w_mp7", this.tex_mp7[1], -1));
		this.mp7.Add(new ABTest.CWeaponDat(2, "mp7_pop_01", "MP7 RAINBOW ROMB", "w_mp7", this.tex_mp7[2], -1));
		this.mp7.Add(new ABTest.CWeaponDat(3, "mp7_pop_02", "MP7 SKULL ORNAMENT", "w_mp7", this.tex_mp7[3], -1));
		this.p90 = new List<ABTest.CWeaponDat>();
		this.p90.Add(new ABTest.CWeaponDat(0, "p90_camo_01", "P90 HEXAGON", "w_p90", this.tex_p90[0], -1));
		this.p90.Add(new ABTest.CWeaponDat(1, "p90_camo_02", "P90 JUNGLE SQUARE", "w_p90", this.tex_p90[1], -1));
		this.p90.Add(new ABTest.CWeaponDat(2, "p90_pop_01", "P90 MONEYBACK", "w_p90", this.tex_p90[2], -1));
		this.p90.Add(new ABTest.CWeaponDat(3, "p90_pop_02", "P90 COLORISTA", "w_p90", this.tex_p90[3], -1));
		this.pkp = new List<ABTest.CWeaponDat>();
		this.pkp.Add(new ABTest.CWeaponDat(0, "pkp_camo_01", "PKP DIGITAL DESERT", "w_pkp", this.tex_pkp[0], -1));
		this.pkp.Add(new ABTest.CWeaponDat(1, "pkp_pop_01", "PKP TAN", "w_pkp", this.tex_pkp[1], -1));
		this.pkp.Add(new ABTest.CWeaponDat(2, "pkp_pop_02", "PKP RAINBOW ROMB", "w_pkp", this.tex_pkp[2], -1));
		this.qbz95 = new List<ABTest.CWeaponDat>();
		this.qbz95.Add(new ABTest.CWeaponDat(0, "qbz95_camo_01", "QBZ-95 DIGITAL DESERT", "w_qbz95", this.tex_qbz95[0], -1));
		this.qbz95.Add(new ABTest.CWeaponDat(1, "qbz95_camo_02", "QBZ-95 JUNGLE SQUARE", "w_qbz95", this.tex_qbz95[1], -1));
		this.qbz95.Add(new ABTest.CWeaponDat(2, "qbz95_pop_01", "QBZ-95 SKULL ORNAMENT", "w_qbz95", this.tex_qbz95[2], -1));
		this.qbz95.Add(new ABTest.CWeaponDat(3, "qbz95_pop_02", "QBZ-95 RAINBOW ROMB", "w_qbz95", this.tex_qbz95[3], -1));
		this.remington = new List<ABTest.CWeaponDat>();
		this.remington.Add(new ABTest.CWeaponDat(0, "remington_camo_01", "REMINGTON KRYPTEK SNAKE", "w_remington", this.tex_remington[0], -1));
		this.remington.Add(new ABTest.CWeaponDat(1, "remington_camo_02", "REMINGTON DIGITAL DESERT", "w_remington", this.tex_remington[1], -1));
		this.remington.Add(new ABTest.CWeaponDat(2, "remington_pop_01", "REMINGTON SKULL ORNAMENT", "w_remington", this.tex_remington[2], -1));
		this.remington.Add(new ABTest.CWeaponDat(3, "remington_pop_02", "REMINGTON ORANGE RUST", "w_remington", this.tex_remington[3], -1));
		this.spas12 = new List<ABTest.CWeaponDat>();
		this.spas12.Add(new ABTest.CWeaponDat(0, "spas12_camo_01", "SPAS-12 DIGITAL DESERT", "w_spas12", this.tex_spas12[0], -1));
		this.spas12.Add(new ABTest.CWeaponDat(1, "spas12_camo_02", "SPAS-12 JUNGLE SQUARE", "w_spas12", this.tex_spas12[1], -1));
		this.spas12.Add(new ABTest.CWeaponDat(2, "spas12_pop_01", "SPAS-12 SKULL ORNAMENT", "w_spas12", this.tex_spas12[2], -1));
		this.svd = new List<ABTest.CWeaponDat>();
		this.svd.Add(new ABTest.CWeaponDat(0, "svd_camo_01", "SVD KRYPTEK SNAKE", "w_svd", this.tex_svd[0], -1));
		this.svd.Add(new ABTest.CWeaponDat(1, "svd_camo_02", "SVD JUNGLE SQUARE", "w_svd", this.tex_svd[1], -1));
		this.svd.Add(new ABTest.CWeaponDat(2, "svd_pop_01", "SVD BLACK", "w_svd", this.tex_svd[2], -1));
		this.ump45 = new List<ABTest.CWeaponDat>();
		this.ump45.Add(new ABTest.CWeaponDat(0, "ump45_camo_01", "UMP45 KRYPTEK SNAKE", "w_ump45", this.tex_ump45[0], -1));
		this.ump45.Add(new ABTest.CWeaponDat(1, "ump45_camo_02", "UMP45 DIGITAL DESERT", "w_ump45", this.tex_ump45[1], -1));
		this.ump45.Add(new ABTest.CWeaponDat(2, "ump45_camo_03", "UMP45 JUNGLE SQUARE", "w_ump45", this.tex_ump45[2], -1));
		this.ump45.Add(new ABTest.CWeaponDat(3, "ump45_pops_01", "UMP45 SKULL ORNAMENT", "w_ump45", this.tex_ump45[3], -1));
		this.ump45.Add(new ABTest.CWeaponDat(4, "ump45_pops_02", "UMP45 RAINBOW ROMB", "w_ump45", this.tex_ump45[4], -1));
		this.ResetFile();
		base.StartCoroutine(this.StartMainTest());
	}

	private void WeaponShow(Dictionary<string, GameObject> weaponDict, string name, Texture2D tex)
	{
		foreach (KeyValuePair<string, GameObject> current in weaponDict)
		{
			if (current.Key == name)
			{
				current.Value.SetActive(true);
				current.Value.GetComponent<MeshRenderer>().material.mainTexture = tex;
				foreach (Transform transform in current.Value.transform)
				{
					transform.GetComponent<MeshRenderer>().material.mainTexture = tex;
				}
			}
			else
			{
				current.Value.SetActive(false);
			}
		}
	}

	[DebuggerHidden]
	private IEnumerator StartMainTest()
	{
		ABTest.<StartMainTest>c__Iterator0 <StartMainTest>c__Iterator = new ABTest.<StartMainTest>c__Iterator0();
		<StartMainTest>c__Iterator.<>f__this = this;
		return <StartMainTest>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator StartABTest(List<ABTest.CWeaponDat> weaponList)
	{
		ABTest.<StartABTest>c__Iterator1 <StartABTest>c__Iterator = new ABTest.<StartABTest>c__Iterator1();
		<StartABTest>c__Iterator.weaponList = weaponList;
		<StartABTest>c__Iterator.<$>weaponList = weaponList;
		<StartABTest>c__Iterator.<>f__this = this;
		return <StartABTest>c__Iterator;
	}

	private int GetAB(List<ABTest.CWeaponDat> weaponlist)
	{
		for (int i = 0; i < weaponlist.Count; i++)
		{
			if (weaponlist[i].points < 0)
			{
				if (this.lastWeaponId < i)
				{
					this.lastWeaponId = i;
					return i;
				}
			}
		}
		return this.lastWeaponId;
	}

	[DebuggerHidden]
	private IEnumerator GetSelectAB()
	{
		ABTest.<GetSelectAB>c__Iterator2 <GetSelectAB>c__Iterator = new ABTest.<GetSelectAB>c__Iterator2();
		<GetSelectAB>c__Iterator.<>f__this = this;
		return <GetSelectAB>c__Iterator;
	}

	public void SelectA()
	{
		this.variant = 0;
	}

	public void SelectB()
	{
		this.variant = 1;
	}

	private void ResetFile()
	{
		StreamWriter streamWriter = new StreamWriter("result.csv", false);
		streamWriter.Close();
	}

	private void AppendFile(string data)
	{
		StreamWriter streamWriter = new StreamWriter("result.csv", true);
		streamWriter.WriteLine(data);
		streamWriter.Close();
		MonoBehaviour.print(data);
	}

	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.Alpha1))
		{
			this.variant = 0;
		}
		if (Input.GetKeyUp(KeyCode.Alpha2))
		{
			this.variant = 1;
		}
	}
}
