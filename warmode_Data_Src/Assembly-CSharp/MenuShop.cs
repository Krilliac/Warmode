using System;
using UnityEngine;

public class MenuShop : MonoBehaviour
{
	public class CShopData
	{
		public Texture2D icon;

		public string name;

		public string iconname;

		public string name2;

		public int wid;

		public int section;

		public int cost;

		public int level;

		public static int[] catcount = new int[8];

		public CShopData(int wid, string iconname, string name, int section, string prefabname, int cost, int level = 0)
		{
			this.iconname = iconname;
			this.icon = TEX.GetTextureByName(iconname);
			if (this.icon == null)
			{
				this.icon = TEX.GetTextureByName("red");
			}
			this.name = name;
			this.name2 = prefabname;
			this.wid = wid;
			this.section = section;
			this.cost = cost;
			this.level = level;
			MenuShop.CShopData.catcount[section]++;
		}

		~CShopData()
		{
		}
	}

	private static bool show = false;

	public static bool inbuy = false;

	private static Texture2D apply = null;

	private static Texture2D view = null;

	private static Texture2D tGold = null;

	private static float showtime = 0f;

	public static MenuShop.CShopData[] shopdata = new MenuShop.CShopData[1024];

	private static int shopdataloaded = 0;

	private static bool generated = false;

	private static Texture2D tBlack;

	private static Texture2D tWhite;

	private static Texture2D tGray;

	private static Texture2D tOrange;

	private static Texture2D tGreen;

	private static Rect rBack;

	private static Rect rBackHeader;

	private static Rect rBackBody;

	private static Rect rBuy;

	private static Rect rView;

	private static int currCat = 0;

	public static MenuShop.CShopData currData = null;

	private static Vector2 scroll = Vector2.zero;

	private static int hcount = 0;

	public static void Init()
	{
		if (MenuShop.shopdataloaded <= 1)
		{
			MenuShop.shopdata[1] = new MenuShop.CShopData(1, "pistol_glock17", "GLOCK 17", 0, "glock17", 0, 0);
			MenuShop.shopdata[2] = new MenuShop.CShopData(2, "pistol_beretta", "BERETTA", 0, "beretta", 0, 0);
			MenuShop.shopdata[3] = new MenuShop.CShopData(3, "pistol_colt_ssp", "COLT SSP", 0, "colt", 0, 0);
			MenuShop.shopdata[4] = new MenuShop.CShopData(4, "pistol_deagle", "DESERT EAGLE", 0, "deagle", 0, 0);
			MenuShop.shopdata[5] = new MenuShop.CShopData(5, "pistol_remington", "REMINGTON", 0, "remington", 0, 0);
			MenuShop.shopdata[6] = new MenuShop.CShopData(6, "rifle_ak47", "АК-47", 0, "ak47", 0, 0);
			MenuShop.shopdata[7] = new MenuShop.CShopData(7, "rifle_aks74u", "АКС-74У", 0, "aks74u", 0, 0);
			MenuShop.shopdata[8] = new MenuShop.CShopData(8, "rifle_asval", "ASVAL", 0, "asval", 0, 0);
			MenuShop.shopdata[9] = new MenuShop.CShopData(9, "shotgun_m4", "BENELLI M4", 0, "bm4", 0, 0);
			MenuShop.shopdata[10] = new MenuShop.CShopData(10, "rifle_famas", "FAMAS", 0, "famas", 0, 0);
			MenuShop.shopdata[11] = new MenuShop.CShopData(11, "rifle_m4", "M4A1", 0, "m4a1", 0, 0);
			MenuShop.shopdata[12] = new MenuShop.CShopData(12, "smg_mp5", "MP5", 0, "mp5", 0, 0);
			MenuShop.shopdata[13] = new MenuShop.CShopData(13, "smg_mp7", "MP7", 0, "mp7", 0, 0);
			MenuShop.shopdata[14] = new MenuShop.CShopData(14, "smg_p90", "P90", 0, "p90", 0, 0);
			MenuShop.shopdata[15] = new MenuShop.CShopData(15, "rifle_qbz95", "QBZ-95", 0, "qbz95", 0, 0);
			MenuShop.shopdata[16] = new MenuShop.CShopData(16, "shotgun_spas12", "SPAS-12", 0, "spas12", 0, 0);
			MenuShop.shopdata[17] = new MenuShop.CShopData(17, "smg_ump45", "UMP45", 0, "ump45", 0, 0);
			MenuShop.shopdata[18] = new MenuShop.CShopData(18, "rifle_aug", "AUG", 0, "aug", 0, 0);
			MenuShop.shopdata[19] = new MenuShop.CShopData(19, "sniper_svd", "СВД", 0, "svd", 0, 0);
			MenuShop.shopdata[20] = new MenuShop.CShopData(20, "sniper_m110", "M110", 0, "m110", 0, 0);
			MenuShop.shopdata[21] = new MenuShop.CShopData(21, "sniper_m24", "M24", 0, "m24", 0, 0);
			MenuShop.shopdata[22] = new MenuShop.CShopData(22, "sniper_awp", "AWP", 0, "awp", 0, 0);
			MenuShop.shopdata[23] = new MenuShop.CShopData(23, "sniper_m90", "M90", 0, "m90", 0, 0);
			MenuShop.shopdata[24] = new MenuShop.CShopData(24, "mg_pk_pecheneg", "PKP", 0, "pkp", 0, 0);
			MenuShop.shopdata[25] = new MenuShop.CShopData(25, "mg_m249", "M249", 0, "m249", 0, 0);
			MenuShop.shopdata[128] = new MenuShop.CShopData(128, "warbadge_hor0", "BADGE BACK #1", 1, string.Empty, 99, 0);
			MenuShop.shopdata[129] = new MenuShop.CShopData(129, "warbadge_hor1", "BADGE BACK #2", 1, string.Empty, 99, 0);
			MenuShop.shopdata[130] = new MenuShop.CShopData(130, "warbadge_hor2", "BADGE BACK #3", 1, string.Empty, 99, 0);
			MenuShop.shopdata[131] = new MenuShop.CShopData(131, "warbadge_hor3", "BADGE BACK #4", 1, string.Empty, 99, 0);
			MenuShop.shopdata[132] = new MenuShop.CShopData(132, "warbadge_hor4", "BADGE BACK #5", 1, string.Empty, 99, 0);
			MenuShop.shopdata[133] = new MenuShop.CShopData(133, "warbadge_hor5", "BADGE BACK #6", 1, string.Empty, 99, 0);
			MenuShop.shopdata[134] = new MenuShop.CShopData(134, "warbadge_hor6", "BADGE BACK #7", 1, string.Empty, 99, 0);
			MenuShop.shopdata[135] = new MenuShop.CShopData(135, "warbadge_hor7", "BADGE BACK #8", 1, string.Empty, 99, 0);
			MenuShop.shopdata[136] = new MenuShop.CShopData(136, "warbadge_icon0", "BADGE ICON #1", 2, string.Empty, 99, 0);
			MenuShop.shopdata[137] = new MenuShop.CShopData(137, "warbadge_icon1", "BADGE ICON #2", 2, string.Empty, 99, 0);
			MenuShop.shopdata[138] = new MenuShop.CShopData(138, "warbadge_icon2", "BADGE ICON #3", 2, string.Empty, 99, 0);
			MenuShop.shopdata[139] = new MenuShop.CShopData(139, "warbadge_icon3", "BADGE ICON #4", 2, string.Empty, 99, 0);
			MenuShop.shopdata[140] = new MenuShop.CShopData(140, "warbadge_icon4", "BADGE ICON #5", 2, string.Empty, 99, 0);
			MenuShop.shopdata[141] = new MenuShop.CShopData(141, "warbadge_icon5", "BADGE ICON #6", 2, string.Empty, 99, 0);
			MenuShop.shopdata[142] = new MenuShop.CShopData(142, "warbadge_icon6", "BADGE ICON #7", 2, string.Empty, 99, 0);
			MenuShop.shopdata[143] = new MenuShop.CShopData(143, "warbadge_icon7", "BADGE ICON #8", 2, string.Empty, 99, 0);
			MenuShop.shopdata[144] = new MenuShop.CShopData(144, "warbadge_hor8", "BADGE BACK #9", 1, string.Empty, 99, 0);
			MenuShop.shopdata[145] = new MenuShop.CShopData(145, "warbadge_hor9", "BADGE BACK #10", 1, string.Empty, 99, 0);
			MenuShop.shopdata[146] = new MenuShop.CShopData(146, "warbadge_hor10", "BADGE BACK #11", 1, string.Empty, 99, 0);
			MenuShop.shopdata[147] = new MenuShop.CShopData(147, "warbadge_icon8", "BADGE ICON #9", 2, string.Empty, 99, 0);
			MenuShop.shopdata[148] = new MenuShop.CShopData(148, "warbadge_icon9", "BADGE ICON #10", 2, string.Empty, 99, 0);
			MenuShop.shopdata[149] = new MenuShop.CShopData(149, "warbadge_icon10", "BADGE ICON #11", 2, string.Empty, 99, 0);
			MenuShop.shopdata[150] = new MenuShop.CShopData(150, "warbadge_hor11", "BADGE BACK #12", 1, string.Empty, 99, 0);
			MenuShop.shopdata[151] = new MenuShop.CShopData(151, "warbadge_hor12", "BADGE BACK #13", 1, string.Empty, 99, 0);
			MenuShop.shopdata[152] = new MenuShop.CShopData(152, "warbadge_hor13", "BADGE BACK #14", 1, string.Empty, 99, 0);
			MenuShop.shopdata[153] = new MenuShop.CShopData(153, "warbadge_hor14", "BADGE BACK #15", 1, string.Empty, 99, 0);
			MenuShop.shopdata[154] = new MenuShop.CShopData(154, "warbadge_icon11", "BADGE ICON #12", 2, string.Empty, 99, 0);
			MenuShop.shopdata[155] = new MenuShop.CShopData(155, "warbadge_icon12", "BADGE ICON #13", 2, string.Empty, 99, 0);
			MenuShop.shopdata[156] = new MenuShop.CShopData(156, "warbadge_icon13", "BADGE ICON #14", 2, string.Empty, 99, 0);
			MenuShop.shopdata[157] = new MenuShop.CShopData(157, "warbadge_icon14", "BADGE ICON #15", 2, string.Empty, 99, 0);
			MenuShop.shopdata[384] = new MenuShop.CShopData(384, "merc_001", "MERC MASK #1", 3, string.Empty, 99, 0);
			MenuShop.shopdata[385] = new MenuShop.CShopData(385, "merc_002", "MERC MASK #2", 3, string.Empty, 99, 0);
			MenuShop.shopdata[386] = new MenuShop.CShopData(386, "merc_003", "MERC MASK #3", 3, string.Empty, 99, 0);
			MenuShop.shopdata[387] = new MenuShop.CShopData(387, "merc_004", "MERC MASK #4", 3, string.Empty, 99, 0);
			MenuShop.shopdata[388] = new MenuShop.CShopData(388, "warcorp_001", "WARCORP MASK #1", 4, string.Empty, 99, 0);
			MenuShop.shopdata[389] = new MenuShop.CShopData(389, "warcorp_002", "WARCORP MASK #2", 4, string.Empty, 99, 0);
			MenuShop.shopdata[390] = new MenuShop.CShopData(390, "warcorp_003", "WARCORP MASK #3", 4, string.Empty, 99, 0);
			MenuShop.shopdata[391] = new MenuShop.CShopData(391, "merc_005", "MERC MASK #5", 3, string.Empty, 99, 0);
			MenuShop.shopdata[392] = new MenuShop.CShopData(392, "merc_006", "MERC MASK #6", 3, string.Empty, 99, 0);
			MenuShop.shopdata[393] = new MenuShop.CShopData(393, "merc_007", "MERC MASK #7", 3, string.Empty, 99, 0);
			MenuShop.shopdata[394] = new MenuShop.CShopData(394, "merc_008", "MERC MASK #8", 3, string.Empty, 99, 0);
			MenuShop.shopdata[395] = new MenuShop.CShopData(395, "merc_009", "MERC MASK #9", 3, string.Empty, 99, 0);
			MenuShop.shopdata[396] = new MenuShop.CShopData(396, "merc_010", "MERC MASK #10", 3, string.Empty, 99, 0);
			MenuShop.shopdata[397] = new MenuShop.CShopData(397, "merc_011", "MERC MASK #11", 3, string.Empty, 99, 0);
			MenuShop.shopdata[398] = new MenuShop.CShopData(398, "merc_012", "MERC MASK #12", 3, string.Empty, 99, 0);
			MenuShop.shopdata[399] = new MenuShop.CShopData(399, "merc_013", "MERC MASK #13", 3, string.Empty, 99, 0);
			MenuShop.shopdata[400] = new MenuShop.CShopData(400, "merc_014", "MERC MASK #14", 3, string.Empty, 99, 0);
			MenuShop.shopdata[401] = new MenuShop.CShopData(401, "warcorp_004", "WARCORP MASK #4", 4, string.Empty, 99, 0);
			MenuShop.shopdata[402] = new MenuShop.CShopData(402, "warcorp_005", "WARCORP MASK #5", 4, string.Empty, 99, 0);
			MenuShop.shopdata[403] = new MenuShop.CShopData(403, "warcorp_006", "WARCORP MASK #6", 4, string.Empty, 99, 0);
			MenuShop.shopdata[404] = new MenuShop.CShopData(404, "warcorp_007", "WARCORP MASK #7", 4, string.Empty, 99, 0);
			MenuShop.shopdata[405] = new MenuShop.CShopData(405, "warcorp_008", "WARCORP MASK #8", 4, string.Empty, 99, 0);
			MenuShop.shopdata[406] = new MenuShop.CShopData(406, "warcorp_009", "WARCORP MASK #9", 4, string.Empty, 99, 0);
			MenuShop.shopdata[407] = new MenuShop.CShopData(407, "warcorp_010", "WARCORP MASK #10", 4, string.Empty, 99, 0);
			MenuShop.shopdata[408] = new MenuShop.CShopData(408, "warcorp_011", "WARCORP MASK #11", 4, string.Empty, 99, 0);
			MenuShop.shopdata[409] = new MenuShop.CShopData(409, "warcorp_012", "WARCORP MASK #12", 4, string.Empty, 99, 0);
			MenuShop.shopdata[410] = new MenuShop.CShopData(410, "warcorp_013", "WARCORP MASK #13", 4, string.Empty, 99, 0);
			MenuShop.shopdata[412] = new MenuShop.CShopData(412, "warcorp_014", "WARCORP MASK #14", 4, string.Empty, 99, 0);
			MenuShop.shopdata[512] = new MenuShop.CShopData(512, "ak47_art_colorista", "AK47 COLORISTA", 5, "ak47", 99, 1);
			MenuShop.shopdata[513] = new MenuShop.CShopData(513, "ak47_art_hexagon", "AK47 HEXAGON", 5, "ak47", 250, 6);
			MenuShop.shopdata[514] = new MenuShop.CShopData(514, "ak47_art_romb", "AK47 RAINBOW ROMB", 5, "ak47", 145, 3);
			MenuShop.shopdata[515] = new MenuShop.CShopData(515, "ak47_art_rust", "AK47 ORANGE RUST", 5, "ak47", 150, 3);
			MenuShop.shopdata[516] = new MenuShop.CShopData(516, "ak47_art_splat", "AK47 COLORFUL SPLAT", 5, "ak47", 140, 3);
			MenuShop.shopdata[517] = new MenuShop.CShopData(517, "ak47_camo_digitaldesert", "AK47 DIGITAL DESERT", 5, "ak47", 170, 4);
			MenuShop.shopdata[518] = new MenuShop.CShopData(518, "ak47_camo_multicam", "AK47 MODERN MULTICAM", 5, "ak47", 145, 3);
			MenuShop.shopdata[519] = new MenuShop.CShopData(519, "ak47_color_od", "AK47 OD", 5, "ak47", 120, 2);
			MenuShop.shopdata[520] = new MenuShop.CShopData(520, "ak47_color_tan", "AK47 TAN", 5, "ak47", 130, 2);
			MenuShop.shopdata[521] = new MenuShop.CShopData(521, "ak47_color_white", "AK47 WHITE", 5, "ak47", 99, 1);
			MenuShop.shopdata[522] = new MenuShop.CShopData(522, "asval_art_colorfulsplat", "ASVAL COLORFUL SPLAT", 5, "asval", 160, 3);
			MenuShop.shopdata[523] = new MenuShop.CShopData(523, "asval_art_greenmonster", "ASVAL GREEN MONSTER", 5, "asval", 230, 6);
			MenuShop.shopdata[524] = new MenuShop.CShopData(524, "asval_art_hexagon", "ASVAL HEXAGON", 5, "asval", 175, 5);
			MenuShop.shopdata[525] = new MenuShop.CShopData(525, "asval_art_orangerust", "ASVAL ORANGE RUST", 5, "asval", 165, 4);
			MenuShop.shopdata[526] = new MenuShop.CShopData(526, "asval_art_rainbowromb", "ASVAL RAINBOW ROMB", 5, "asval", 140, 2);
			MenuShop.shopdata[527] = new MenuShop.CShopData(527, "asval_camo_desertdigital", "ASVAL DIGITAL DESERT", 5, "asval", 160, 3);
			MenuShop.shopdata[528] = new MenuShop.CShopData(528, "asval_camo_krypteksnake", "ASVAL KRYPTEK SNAKE", 5, "asval", 120, 1);
			MenuShop.shopdata[529] = new MenuShop.CShopData(529, "asval_camo_modernmulticam", "ASVAL MODERN MULTICAM", 5, "asval", 120, 1);
			MenuShop.shopdata[530] = new MenuShop.CShopData(530, "awp_art_hexagon", "AWP HEXAGON", 5, "awp", 450, 7);
			MenuShop.shopdata[531] = new MenuShop.CShopData(531, "awp_art_romb", "AWP RAINBOW ROMB", 5, "awp", 330, 4);
			MenuShop.shopdata[532] = new MenuShop.CShopData(532, "awp_art_rust", "AWP ORANGE RUST", 5, "awp", 350, 5);
			MenuShop.shopdata[533] = new MenuShop.CShopData(533, "awp_art_splat", "AWP COLORFUL SPLAT", 5, "awp", 265, 2);
			MenuShop.shopdata[534] = new MenuShop.CShopData(534, "awp_camo_desertdigital", "AWP DIGITAL DESERT", 5, "awp", 400, 6);
			MenuShop.shopdata[535] = new MenuShop.CShopData(535, "awp_camo_kryptek", "AWP KRYPTEK SNAKE", 5, "awp", 350, 5);
			MenuShop.shopdata[536] = new MenuShop.CShopData(536, "awp_camo_multicam", "AWP MODERN MULTICAM", 5, "awp", 320, 4);
			MenuShop.shopdata[537] = new MenuShop.CShopData(537, "awp_color_cyan", "AWP CYAN", 5, "awp", 200, 1);
			MenuShop.shopdata[538] = new MenuShop.CShopData(538, "awp_color_pink", "AWP PINK", 5, "awp", 220, 1);
			MenuShop.shopdata[539] = new MenuShop.CShopData(539, "awp_color_tan", "AWP TAN", 5, "awp", 300, 3);
			MenuShop.shopdata[540] = new MenuShop.CShopData(540, "awp_color_white", "AWP WHITE", 5, "awp", 250, 2);
			MenuShop.shopdata[541] = new MenuShop.CShopData(541, "glock17_camo_badboy", "GLOCK17 BADBOY", 5, "glock17", 59, 2);
			MenuShop.shopdata[542] = new MenuShop.CShopData(542, "glock17_color_od", "GLOCK17 OD", 5, "glock17", 49, 1);
			MenuShop.shopdata[543] = new MenuShop.CShopData(543, "glock17_color_tan", "GLOCK17 TAN", 5, "glock17", 79, 4);
			MenuShop.shopdata[544] = new MenuShop.CShopData(544, "glock17_camo_carbon", "GLOCK17 CARBON", 5, "glock17", 89, 4);
			MenuShop.shopdata[545] = new MenuShop.CShopData(545, "glock17_camo_digital", "GLOCK17 DIGITAL DESERT", 5, "glock17", 110, 5);
			MenuShop.shopdata[546] = new MenuShop.CShopData(546, "glock17_art_moneyback", "GLOCK17 MONEYBACK", 5, "glock17", 69, 3);
			MenuShop.shopdata[547] = new MenuShop.CShopData(547, "glock17_art_skullornament", "GLOCK17 SKULL ORNAMENT", 5, "glock17", 119, 5);
			MenuShop.shopdata[555] = new MenuShop.CShopData(555, "ak47_art_pinkiecat", "AK47 PINKIE CAT", 5, "ak47", 180, 5);
			MenuShop.shopdata[556] = new MenuShop.CShopData(556, "ak47_camo_junglesquare", "AK47 JUNGLE SQUARE", 5, "ak47", 170, 4);
			MenuShop.shopdata[562] = new MenuShop.CShopData(562, "aks74u_camo_01", "АКС-74У FOREST", 5, "aks74u", 70, 3);
			MenuShop.shopdata[563] = new MenuShop.CShopData(563, "aks74u_camo_02", "АКС-74У JUNGLE SQUARE", 5, "aks74u", 60, 2);
			MenuShop.shopdata[564] = new MenuShop.CShopData(564, "aks74u_pops_01", "АКС-74У BLACK", 5, "aks74u", 49, 1);
			MenuShop.shopdata[565] = new MenuShop.CShopData(565, "aks74u_pops_02", "АКС-74У SKULL ORNAMENT", 5, "aks74u", 100, 5);
			MenuShop.shopdata[571] = new MenuShop.CShopData(571, "aug_camo_01", "AUG HEXAGON", 5, "aug", 145, 3);
			MenuShop.shopdata[572] = new MenuShop.CShopData(572, "aug_camo_02", "AUG DIGITAL DESERT", 5, "aug", 150, 5);
			MenuShop.shopdata[573] = new MenuShop.CShopData(573, "aug_pop_01", "AUG BLACK", 5, "aug", 99, 1);
			MenuShop.shopdata[579] = new MenuShop.CShopData(579, "awp_color_od", "AWP OD", 5, "awp", 300, 3);
			MenuShop.shopdata[585] = new MenuShop.CShopData(585, "beretta_camo_01", "BERETTA HEXAGON", 5, "beretta", 99, 4);
			MenuShop.shopdata[586] = new MenuShop.CShopData(586, "beretta_pops_01", "BERETTA TAN", 5, "beretta", 60, 3);
			MenuShop.shopdata[587] = new MenuShop.CShopData(587, "beretta_pops_02", "BERETTA MONEYBACK", 5, "beretta", 49, 1);
			MenuShop.shopdata[593] = new MenuShop.CShopData(593, "bm4_camo_01", "BENELLI M4 DIGITAL DESERT", 5, "bm4", 180, 5);
			MenuShop.shopdata[594] = new MenuShop.CShopData(594, "bm4_camo_02", "BENELLI M4 KRYPTEK SNAKE", 5, "bm4", 149, 1);
			MenuShop.shopdata[595] = new MenuShop.CShopData(595, "bm4_pop_01", "BENELLI M4 MONEYBACK", 5, "bm4", 160, 3);
			MenuShop.shopdata[601] = new MenuShop.CShopData(601, "colt_camo_01", "COLT SSP JUNGLE SQUARE", 5, "colt", 99, 4);
			MenuShop.shopdata[602] = new MenuShop.CShopData(602, "colt_pop_01", "COLT SSP TAN", 5, "colt", 49, 1);
			MenuShop.shopdata[603] = new MenuShop.CShopData(603, "colt_pop_02", "COLT SSP BLACK", 5, "colt", 60, 2);
			MenuShop.shopdata[609] = new MenuShop.CShopData(609, "deagle_color_black", "DESERT EAGLE BLACK", 5, "deagle", 99, 1);
			MenuShop.shopdata[610] = new MenuShop.CShopData(610, "deagle_color_blacktan", "DESERT EAGLE TAN", 5, "deagle", 140, 4);
			MenuShop.shopdata[611] = new MenuShop.CShopData(611, "deagle_camo_01", "DESERT EAGLE JUNGLE SQUARE", 5, "deagle", 150, 5);
			MenuShop.shopdata[612] = new MenuShop.CShopData(612, "deagle_camo_02", "DESERT EAGLE HEXAGON", 5, "deagle", 120, 3);
			MenuShop.shopdata[618] = new MenuShop.CShopData(618, "famas_camo_01", "FAMAS DIGITAL DESERT", 5, "famas", 149, 5);
			MenuShop.shopdata[619] = new MenuShop.CShopData(619, "famas_camo_02", "FAMAS KRYPTEK SNAKE", 5, "famas", 120, 4);
			MenuShop.shopdata[620] = new MenuShop.CShopData(620, "famas_pop_01", "FAMAS MONEYBACK", 5, "famas", 99, 1);
			MenuShop.shopdata[621] = new MenuShop.CShopData(621, "famas_pop_02", "FAMAS COLORISTA", 5, "famas", 110, 3);
			MenuShop.shopdata[627] = new MenuShop.CShopData(627, "m4a1_camo_01", "M4A1 DIGITAL DESERT", 5, "m4a1", 145, 5);
			MenuShop.shopdata[628] = new MenuShop.CShopData(628, "m4a1_camo_02", "M4A1 MODERN MULTICAM", 5, "m4a1", 124, 4);
			MenuShop.shopdata[629] = new MenuShop.CShopData(629, "m4a1_pop_01", "M4A1 COLORISTA", 5, "m4a1", 99, 1);
			MenuShop.shopdata[630] = new MenuShop.CShopData(630, "m4a1_pop_02", "M4A1 MONEYBACK", 5, "m4a1", 120, 3);
			MenuShop.shopdata[636] = new MenuShop.CShopData(636, "m24_camo_01", "M24 JUNGLE SQUARE", 5, "m24", 169, 3);
			MenuShop.shopdata[637] = new MenuShop.CShopData(637, "m24_camo_02", "M24 DIGITAL DESERT", 5, "m24", 180, 5);
			MenuShop.shopdata[638] = new MenuShop.CShopData(638, "m24_pop_01", "M24 MONEYBACK", 5, "m24", 159, 1);
			MenuShop.shopdata[644] = new MenuShop.CShopData(644, "m90_camo_01", "M90 DIGITAL DESERT", 5, "m90", 350, 7);
			MenuShop.shopdata[645] = new MenuShop.CShopData(645, "m90_camo_02", "M90 JUNGLE SQUARE", 5, "m90", 329, 4);
			MenuShop.shopdata[646] = new MenuShop.CShopData(646, "m90_pop_01", "M90 SKULL ORNAMENT", 5, "m90", 339, 6);
			MenuShop.shopdata[647] = new MenuShop.CShopData(647, "m90_pop_02", "M90 RAINBOW ROMB", 5, "m90", 299, 2);
			MenuShop.shopdata[653] = new MenuShop.CShopData(653, "m110_camo_01", "M110 DIGITAL DESERT", 5, "m110", 149, 1);
			MenuShop.shopdata[654] = new MenuShop.CShopData(654, "m110_camo_02", "M110 HEXAGON", 5, "m110", 169, 5);
			MenuShop.shopdata[655] = new MenuShop.CShopData(655, "m110_pop_01", "M110 SKULL ORNAMENT", 5, "m110", 159, 3);
			MenuShop.shopdata[661] = new MenuShop.CShopData(661, "m249_camo_01", "M249 HEXAGON", 5, "m249", 269, 7);
			MenuShop.shopdata[662] = new MenuShop.CShopData(662, "m249_camo_02", "M249 KRYPTEK SNAKE", 5, "m249", 249, 4);
			MenuShop.shopdata[663] = new MenuShop.CShopData(663, "m249_pop_01", "M249 MONEYBACK", 5, "m249", 199, 2);
			MenuShop.shopdata[669] = new MenuShop.CShopData(669, "mp5_camo_01", "MP5 KRYPTEK SNAKE", 5, "mp5", 99, 4);
			MenuShop.shopdata[670] = new MenuShop.CShopData(670, "mp5_camo_02", "MP5 MODERN MULTICAM", 5, "mp5", 89, 3);
			MenuShop.shopdata[671] = new MenuShop.CShopData(671, "mp5_pop_01", "MP5 HEXAGON", 5, "mp5", 110, 5);
			MenuShop.shopdata[672] = new MenuShop.CShopData(672, "mp5_pop_02", "MP5 MONEYBACK", 5, "mp5", 79, 1);
			MenuShop.shopdata[678] = new MenuShop.CShopData(678, "mp7_camo_01", "MP7 DIGITAL DESERT", 5, "mp7", 89, 3);
			MenuShop.shopdata[679] = new MenuShop.CShopData(679, "mp7_camo_02", "MP7 HEXAGON", 5, "mp7", 109, 5);
			MenuShop.shopdata[680] = new MenuShop.CShopData(680, "mp7_pop_01", "MP7 RAINBOW ROMB", 5, "mp7", 69, 1);
			MenuShop.shopdata[681] = new MenuShop.CShopData(681, "mp7_pop_02", "MP7 SKULL ORNAMENT", 5, "mp7", 99, 4);
			MenuShop.shopdata[687] = new MenuShop.CShopData(687, "p90_camo_01", "P90 HEXAGON", 5, "p90", 109, 4);
			MenuShop.shopdata[688] = new MenuShop.CShopData(688, "p90_camo_02", "P90 JUNGLE SQUARE", 5, "p90", 129, 5);
			MenuShop.shopdata[689] = new MenuShop.CShopData(689, "p90_pop_01", "P90 MONEYBACK", 5, "p90", 99, 3);
			MenuShop.shopdata[690] = new MenuShop.CShopData(690, "p90_pop_02", "P90 COLORISTA", 5, "p90", 79, 1);
			MenuShop.shopdata[696] = new MenuShop.CShopData(696, "pkp_camo_01", "PKP DIGITAL DESERT", 5, "pkp", 269, 7);
			MenuShop.shopdata[697] = new MenuShop.CShopData(697, "pkp_pop_01", "PKP TAN", 5, "pkp", 199, 2);
			MenuShop.shopdata[698] = new MenuShop.CShopData(698, "pkp_pop_02", "PKP RAINBOW ROMB", 5, "pkp", 249, 4);
			MenuShop.shopdata[704] = new MenuShop.CShopData(704, "qbz95_camo_01", "QBZ-95 DIGITAL DESERT", 5, "qbz95", 139, 5);
			MenuShop.shopdata[705] = new MenuShop.CShopData(705, "qbz95_camo_02", "QBZ-95 JUNGLE SQUARE", 5, "qbz95", 129, 4);
			MenuShop.shopdata[706] = new MenuShop.CShopData(706, "qbz95_pop_01", "QBZ-95 SKULL ORNAMENT", 5, "qbz95", 99, 1);
			MenuShop.shopdata[707] = new MenuShop.CShopData(707, "qbz95_pop_02", "QBZ-95 RAINBOW ROMB", 5, "qbz95", 99, 3);
			MenuShop.shopdata[713] = new MenuShop.CShopData(713, "remington_camo_01", "REMINGTON KRYPTEK SNAKE", 5, "remington", 59, 2);
			MenuShop.shopdata[714] = new MenuShop.CShopData(714, "remington_camo_02", "REMINGTON DIGITAL DESERT", 5, "remington", 89, 4);
			MenuShop.shopdata[715] = new MenuShop.CShopData(715, "remington_pop_01", "REMINGTON SKULL ORNAMENT", 5, "remington", 69, 3);
			MenuShop.shopdata[716] = new MenuShop.CShopData(716, "remington_pop_02", "REMINGTON ORANGE RUST", 5, "remington", 49, 1);
			MenuShop.shopdata[724] = new MenuShop.CShopData(724, "spas12_camo_01", "SPAS-12 DIGITAL DESERT", 5, "spas12", 159, 3);
			MenuShop.shopdata[725] = new MenuShop.CShopData(725, "spas12_camo_02", "SPAS-12 JUNGLE SQUARE", 5, "spas12", 149, 1);
			MenuShop.shopdata[726] = new MenuShop.CShopData(726, "spas12_pop_01", "SPAS-12 SKULL ORNAMENT", 5, "spas12", 169, 5);
			MenuShop.shopdata[732] = new MenuShop.CShopData(732, "svd_camo_01", "СВД KRYPTEK SNAKE", 5, "svd", 199, 5);
			MenuShop.shopdata[733] = new MenuShop.CShopData(733, "svd_camo_02", "СВД JUNGLE SQUARE", 5, "svd", 179, 3);
			MenuShop.shopdata[734] = new MenuShop.CShopData(734, "svd_pop_01", "СВД BLACK", 5, "svd", 169, 1);
			MenuShop.shopdata[740] = new MenuShop.CShopData(740, "ump45_camo_01", "UMP45 KRYPTEK SNAKE", 5, "ump45", 75, 4);
			MenuShop.shopdata[741] = new MenuShop.CShopData(741, "ump45_camo_02", "UMP45 DIGITAL DESERT", 5, "ump45", 99, 5);
			MenuShop.shopdata[742] = new MenuShop.CShopData(742, "ump45_camo_03", "UMP45 JUNGLE SQUARE", 5, "ump45", 69, 3);
			MenuShop.shopdata[743] = new MenuShop.CShopData(743, "ump45_pops_01", "UMP45 SKULL ORNAMENT", 5, "ump45", 59, 2);
			MenuShop.shopdata[744] = new MenuShop.CShopData(744, "ump45_pops_02", "UMP45 RAINBOW ROMB", 5, "ump45", 49, 1);
			MenuShop.shopdataloaded++;
		}
	}

	public void LoadEnd()
	{
		MenuShop.currData = null;
		MenuShop.tBlack = TEX.GetTextureByName("black");
		MenuShop.tOrange = TEX.GetTextureByName("orange");
		MenuShop.tGray = TEX.GetTextureByName("gray");
		MenuShop.tWhite = TEX.GetTextureByName("white");
		MenuShop.tGreen = TEX.GetTextureByName("green");
		MenuShop.apply = TEX.GetTextureByName("apply");
		MenuShop.view = TEX.GetTextureByName("search");
		MenuShop.tGold = TEX.GetTextureByName("gold_64");
		MenuShop.Init();
	}

	public static void GenerateCustomIcons()
	{
		if (MenuShop.generated)
		{
			return;
		}
		MenuShop.generated = true;
		for (int i = 512; i < 1024; i++)
		{
			if (MenuShop.shopdata[i] != null)
			{
				string[] array = MenuShop.shopdata[i].iconname.Split(new char[]
				{
					'_'
				});
				if (array.Length == 3)
				{
					GameObject gameObject = ItemPreview.Create("w_" + array[0]);
					if (gameObject == null)
					{
						MonoBehaviour.print("error create preview: w_" + array[0]);
					}
					else
					{
						ItemPreview.SetSkin(gameObject, MenuShop.shopdata[i].iconname);
						MenuShop.shopdata[i].icon = ItemPreview.Get();
						UnityEngine.Object.DestroyImmediate(gameObject);
					}
				}
			}
		}
	}

	public static void SaveTex2DToDisk(int wid)
	{
		MonoBehaviour.print(MenuShop.shopdata[wid].name);
		MenuShop.shopdata[wid].icon = null;
	}

	public void PostAwake()
	{
		MenuShop.show = false;
		this.OnResize();
	}

	public static void SetActive(bool val)
	{
		MenuShop.show = val;
		if (MenuShop.show)
		{
			MenuPlayer.ChangePlayer(1, 8, 22);
			MenuPlayer.SetPosition(0.9f, 0.03f, -1f);
			MenuShop.currData = null;
			MenuShop.showtime = Time.time;
			MenuShop.GenerateCustomIcons();
		}
	}

	public void OnResize()
	{
		MenuShop.rBack = new Rect((float)Screen.width / 2f + GUIM.YRES(47f), GUIM.YRES(80f), GUIM.YRES(420f), GUIM.YRES(565f));
		MenuShop.rBackHeader = new Rect(MenuShop.rBack.x, MenuShop.rBack.y, MenuShop.rBack.width, GUIM.YRES(160f));
		MenuShop.rBackBody = new Rect(MenuShop.rBack.x, MenuShop.rBack.y + GUIM.YRES(170f), MenuShop.rBack.width, GUIM.YRES(395f));
		MenuShop.rBuy = new Rect(MenuShop.rBackHeader.x + MenuShop.rBackHeader.width - GUIM.YRES(136f), MenuShop.rBackHeader.y + GUIM.YRES(120f), GUIM.YRES(120f), GUIM.YRES(24f));
		MenuShop.rView = new Rect(MenuShop.rBackHeader.x + 16f, MenuShop.rBackHeader.y + GUIM.YRES(120f), GUIM.YRES(24f), GUIM.YRES(24f));
	}

	public static int CentToGold(int cent)
	{
		return Mathf.CeilToInt((float)cent / 100f * 65f / 7f * 8f);
	}

	public static int DollarToGolos(int cost)
	{
		return Mathf.CeilToInt((float)cost * 65f / 7f);
	}

	public static void Draw()
	{
		if (!MenuShop.show)
		{
			return;
		}
		float num = Time.time - MenuShop.showtime + 0.001f;
		if (num > 0.05f)
		{
			num = 0.05f;
		}
		num *= 20f;
		Matrix4x4 matrix = GUI.matrix;
		Vector3 s = new Vector3(num, num, 1f);
		Vector3 pos = new Vector3(MenuShop.rBack.center.x - MenuShop.rBack.center.x * num, MenuShop.rBack.center.y - MenuShop.rBack.center.y * num, 1f);
		GUI.matrix = Matrix4x4.TRS(pos, Quaternion.identity, s);
		GUIM.DrawBox(MenuShop.rBackHeader, MenuShop.tBlack);
		GUIM.DrawBox(MenuShop.rBackBody, MenuShop.tBlack);
		if (MenuShop.currData != null)
		{
			if (MenuShop.currData.section == 1)
			{
				GUI.DrawTexture(new Rect(MenuShop.rBackHeader.x + GUIM.YRES(8f), MenuShop.rBackHeader.y + GUIM.YRES(8f) + GUIM.YRES(40f), GUIM.YRES(256f), GUIM.YRES(64f)), MenuShop.currData.icon);
			}
			if (MenuShop.currData.section == 2)
			{
				GUI.DrawTexture(new Rect(MenuShop.rBackHeader.x + GUIM.YRES(84f), MenuShop.rBackHeader.y + GUIM.YRES(20f), GUIM.YRES(120f), GUIM.YRES(120f)), MenuShop.currData.icon);
			}
			if (MenuShop.currData.section == 3 || MenuShop.currData.section == 4)
			{
				GUI.DrawTexture(new Rect(MenuShop.rBackHeader.x + GUIM.YRES(64f), MenuShop.rBackHeader.y + GUIM.YRES(8f), GUIM.YRES(160f), GUIM.YRES(160f)), MenuShop.currData.icon);
			}
			if (MenuShop.currData.section == 5)
			{
				GUI.DrawTexture(new Rect(MenuShop.rBackHeader.x + GUIM.YRES(16f), MenuShop.rBackHeader.y - GUIM.YRES(48f), GUIM.YRES(256f), GUIM.YRES(256f)), MenuShop.currData.icon);
				if (GUIM.Button(MenuShop.rView, BaseColor.Blue, string.Empty, TextAnchor.MiddleCenter, BaseColor.White, 0, 0, false))
				{
					Main.HideAll();
					MenuPreview.SetActive(true);
					MenuPreview.Preview(MenuShop.currData);
				}
				GUI.DrawTexture(MenuShop.rView, MenuShop.view);
			}
			if (BaseData.item[MenuShop.currData.wid] == 0)
			{
				BaseColor c = BaseColor.Green;
				if (MenuShop.inbuy)
				{
					c = BaseColor.Gray;
				}
				if (BaseData.iLevel < MenuShop.currData.level)
				{
					c = BaseColor.Red;
				}
				if (GUIM.Button(MenuShop.rBuy, c, Lang.Get("_BUY"), TextAnchor.MiddleCenter, BaseColor.White, 1, 12, true) && BaseData.iLevel >= MenuShop.currData.level)
				{
					if (GameData.gSteam)
					{
						WebHandler.get_buy("&itemid=" + MenuShop.currData.wid.ToString());
					}
					else if (GameData.gSocial)
					{
						WebHandler.set_buy("&itemid=" + MenuShop.currData.wid.ToString());
					}
					MenuShop.inbuy = true;
				}
			}
			else
			{
				GUI.DrawTexture(MenuShop.rBuy, MenuShop.tGray);
				GUIM.DrawText(MenuShop.rBuy, Lang.Get("_ALREADY_HAVE"), TextAnchor.MiddleCenter, BaseColor.White, 1, 12, false);
			}
			GUIM.DrawText(new Rect(MenuShop.rBuy.x, MenuShop.rBuy.y - GUIM.YRES(24f), MenuShop.rBuy.width, MenuShop.rBuy.height), Lang.Get("_COST") + ":", TextAnchor.MiddleLeft, BaseColor.White, 1, 12, false);
			string text = string.Format("{0:C}", (float)MenuShop.currData.cost / 100f);
			if (GameData.gSteam)
			{
				GUIM.DrawText(new Rect(MenuShop.rBuy.x, MenuShop.rBuy.y - GUIM.YRES(24f), MenuShop.rBuy.width, MenuShop.rBuy.height), text, TextAnchor.MiddleRight, BaseColor.White, 1, 12, false);
			}
			else if (GameData.gSocial)
			{
				text = MenuShop.CentToGold(MenuShop.currData.cost).ToString();
				GUIM.DrawText(new Rect(MenuShop.rBuy.x, MenuShop.rBuy.y - GUIM.YRES(24f), MenuShop.rBuy.width - GUIM.YRES(14f), MenuShop.rBuy.height), text, TextAnchor.MiddleRight, BaseColor.White, 1, 12, false);
				GUI.DrawTexture(new Rect(MenuShop.rBuy.x + GUIM.YRES(108f), MenuShop.rBuy.y - GUIM.YRES(17f), GUIM.YRES(12f), GUIM.YRES(12f)), MenuShop.tGold);
			}
			GUIM.DrawText(new Rect(MenuShop.rBuy.x, MenuShop.rBackHeader.y + GUIM.YRES(8f), MenuShop.rBuy.width, MenuShop.rBuy.height), MenuShop.currData.name, TextAnchor.MiddleRight, BaseColor.White, 1, 12, false);
		}
		MenuShop.DrawButtonCategory(0, new Rect(MenuShop.rBackBody.x + GUIM.YRES(4f), MenuShop.rBackBody.y + GUIM.YRES(4f), GUIM.YRES(80f), GUIM.YRES(24f)), Lang.Get("_BADGES"), false);
		MenuShop.DrawButtonCategory(1, new Rect(MenuShop.rBackBody.x + GUIM.YRES(4f) + GUIM.YRES(84f), MenuShop.rBackBody.y + GUIM.YRES(4f), GUIM.YRES(80f), GUIM.YRES(24f)), Lang.Get("_MASKS"), false);
		MenuShop.DrawButtonCategory(2, new Rect(MenuShop.rBackBody.x + GUIM.YRES(4f) + GUIM.YRES(84f) * 2f, MenuShop.rBackBody.y + GUIM.YRES(4f), GUIM.YRES(80f), GUIM.YRES(24f)), Lang.Get("_WEAPONS"), false);
		MenuShop.scroll = GUIM.BeginScrollView(new Rect(MenuShop.rBackBody.x + GUIM.YRES(4f), MenuShop.rBackBody.y + GUIM.YRES(32f), MenuShop.rBackBody.width - GUIM.YRES(8f), MenuShop.rBackBody.height - GUIM.YRES(40f)), MenuShop.scroll, new Rect(0f, 0f, 0f, (float)MenuShop.hcount * GUIM.YRES(100f) - GUIM.YRES(4f)));
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < 1024; i++)
		{
			if (MenuShop.shopdata[i] != null)
			{
				if (MenuShop.currCat != 0 || MenuShop.shopdata[i].section == 1 || MenuShop.shopdata[i].section == 2)
				{
					if (MenuShop.currCat != 1 || MenuShop.shopdata[i].section == 3 || MenuShop.shopdata[i].section == 4)
					{
						if (MenuShop.currCat != 2 || MenuShop.shopdata[i].section == 5)
						{
							MenuShop.DrawItem(new Rect((GUIM.YRES(96f) + GUIM.YRES(4f)) * (float)num2, (GUIM.YRES(96f) + GUIM.YRES(4f)) * (float)num3, GUIM.YRES(96f), GUIM.YRES(96f)), MenuShop.shopdata[i]);
							num2++;
							if (num2 >= 4)
							{
								num2 = 0;
								num3++;
							}
						}
					}
				}
			}
		}
		MenuShop.hcount = num3;
		if (num2 != 0)
		{
			MenuShop.hcount++;
		}
		GUIM.EndScrollView();
		GUI.matrix = matrix;
	}

	private static void DrawButtonCategory(int cat, Rect r, string name, bool block = false)
	{
		bool flag;
		if (block)
		{
			flag = GUIM.Button(r, BaseColor.Orange, name, TextAnchor.MiddleCenter, BaseColor.White, 1, 12, false);
			return;
		}
		if (MenuShop.currCat == cat)
		{
			flag = GUIM.Button(r, BaseColor.White, name, TextAnchor.MiddleCenter, BaseColor.Blue, 1, 12, false);
		}
		else
		{
			flag = GUIM.Button(r, BaseColor.Gray, name, TextAnchor.MiddleCenter, BaseColor.White, 1, 12, false);
		}
		if (flag)
		{
			MenuShop.currCat = cat;
			MenuShop.currData = null;
		}
	}

	public static bool DrawItem(Rect r, MenuShop.CShopData data)
	{
		int num = (int)GUIM.YRES(1f);
		if (num < 1)
		{
			num = 1;
		}
		bool flag = false;
		Vector2 mpos = new Vector2(Input.mousePosition.x, (float)Screen.height - Input.mousePosition.y);
		if (data == MenuShop.currData)
		{
			GUI.DrawTexture(r, MenuShop.tWhite);
		}
		else if (GUIM.Contains(r, mpos))
		{
			GUI.DrawTexture(r, MenuShop.tGray);
			GUI.color = new Color(1f, 1f, 1f, 0.1f);
			GUI.DrawTexture(r, MenuShop.tWhite);
			GUI.color = Color.white;
			flag = true;
		}
		else
		{
			if (BaseData.item[data.wid] == 1)
			{
				GUI.color = new Color(1f, 1f, 1f, 0.05f);
				GUI.DrawTexture(r, MenuShop.tWhite);
				GUI.color = Color.white;
			}
			GUI.DrawTexture(new Rect(r.x, r.y, r.width, (float)num), MenuShop.tGray);
			GUI.DrawTexture(new Rect(r.x, r.y + r.height - (float)num, r.width, (float)num), MenuShop.tGray);
			GUI.DrawTexture(new Rect(r.x, r.y, (float)num, r.height), MenuShop.tGray);
			GUI.DrawTexture(new Rect(r.x + r.width - (float)num, r.y, (float)num, r.height), MenuShop.tGray);
		}
		if (data.level > 0)
		{
			BaseColor fontcolor = BaseColor.White;
			if (BaseData.iLevel < data.level)
			{
				fontcolor = BaseColor.Red;
			}
			GUIM.DrawText(r, "Lv." + data.level.ToString() + " ", TextAnchor.LowerRight, fontcolor, 1, 12, false);
		}
		if (data.section == 1)
		{
			GUI.DrawTexture(new Rect(r.x + GUIM.YRES(4f), r.y + GUIM.YRES(4f) + GUIM.YRES(33f), GUIM.YRES(88f), GUIM.YRES(22f)), data.icon);
		}
		if (data.section == 2)
		{
			GUI.DrawTexture(new Rect(r.x + GUIM.YRES(4f), r.y + GUIM.YRES(4f), GUIM.YRES(88f), GUIM.YRES(88f)), data.icon);
		}
		if (data.section == 3 || data.section == 4)
		{
			GUI.DrawTexture(new Rect(r.x + GUIM.YRES(4f), r.y + GUIM.YRES(4f), GUIM.YRES(88f), GUIM.YRES(88f)), data.icon);
		}
		if (data.section == 5)
		{
			GUI.DrawTexture(new Rect(r.x + GUIM.YRES(4f), r.y + GUIM.YRES(4f), GUIM.YRES(88f), GUIM.YRES(88f)), data.icon);
		}
		if (data.section == 0)
		{
			GUI.DrawTexture(new Rect(r.x + GUIM.YRES(4f), r.y + GUIM.YRES(4f) + GUIM.YRES(22f), GUIM.YRES(88f), GUIM.YRES(44f)), data.icon);
		}
		if (MenuInventory.isActive())
		{
			bool flag2 = false;
			int id = WeaponData.GetId(data.name2);
			if (id > 0 && BaseData.profileWeapon[id] == data.wid)
			{
				flag2 = true;
			}
			if (BaseData.badge_back == data.wid || BaseData.badge_icon == data.wid || BaseData.mask_merc == data.wid || BaseData.mask_warcorp == data.wid || flag2)
			{
				GUI.color = Color.green;
				GUI.DrawTexture(new Rect(r.x + GUIM.YRES(4f), r.y + GUIM.YRES(4f), GUIM.YRES(10f), GUIM.YRES(10f)), MenuShop.apply);
				GUI.color = Color.white;
			}
			else if (flag && data.section > 0)
			{
				GUIM.DrawText(r, Lang.Get("_CLICK_TO_EQUIP"), TextAnchor.LowerCenter, BaseColor.White, 1, 10, false);
			}
		}
		bool result = false;
		if (GUIM.HideButton(r))
		{
			MenuShop.currData = data;
			if (MenuShop.show)
			{
				if (MenuShop.currData.section == 3 || MenuShop.currData.section == 4)
				{
					MenuPlayer.PreviewMask(MenuShop.currData);
					MenuPlayer.SetPosition(0.9f, 0.03f, -1f);
				}
				else if (MenuShop.currData.section == 5)
				{
					MenuPlayer.playermodel = -1;
					if (MenuShop.currData.name2 == "ak47")
					{
						MenuPlayer.ChangePlayer(0, 6, 22);
					}
					else if (MenuShop.currData.name2 == "aks74u")
					{
						MenuPlayer.ChangePlayer(0, 7, 22);
					}
					else if (MenuShop.currData.name2 == "asval")
					{
						MenuPlayer.ChangePlayer(0, 8, 22);
					}
					else if (MenuShop.currData.name2 == "aug")
					{
						MenuPlayer.ChangePlayer(0, 18, 22);
					}
					else if (MenuShop.currData.name2 == "awp")
					{
						MenuPlayer.ChangePlayer(0, 22, 8);
					}
					else if (MenuShop.currData.name2 == "beretta")
					{
						MenuPlayer.ChangePlayer(0, 2, 22);
					}
					else if (MenuShop.currData.name2 == "bm4")
					{
						MenuPlayer.ChangePlayer(0, 9, 22);
					}
					else if (MenuShop.currData.name2 == "colt")
					{
						MenuPlayer.ChangePlayer(0, 3, 8);
					}
					else if (MenuShop.currData.name2 == "deagle")
					{
						MenuPlayer.ChangePlayer(0, 4, 8);
					}
					else if (MenuShop.currData.name2 == "famas")
					{
						MenuPlayer.ChangePlayer(0, 10, 22);
					}
					else if (MenuShop.currData.name2 == "glock17")
					{
						MenuPlayer.ChangePlayer(0, 1, 8);
					}
					else if (MenuShop.currData.name2 == "m4a1")
					{
						MenuPlayer.ChangePlayer(0, 11, 22);
					}
					else if (MenuShop.currData.name2 == "m24")
					{
						MenuPlayer.ChangePlayer(0, 21, 22);
					}
					else if (MenuShop.currData.name2 == "m90")
					{
						MenuPlayer.ChangePlayer(0, 23, 22);
					}
					else if (MenuShop.currData.name2 == "m110")
					{
						MenuPlayer.ChangePlayer(0, 20, 22);
					}
					else if (MenuShop.currData.name2 == "m249")
					{
						MenuPlayer.ChangePlayer(0, 25, 22);
					}
					else if (MenuShop.currData.name2 == "mp5")
					{
						MenuPlayer.ChangePlayer(0, 12, 22);
					}
					else if (MenuShop.currData.name2 == "mp7")
					{
						MenuPlayer.ChangePlayer(0, 13, 22);
					}
					else if (MenuShop.currData.name2 == "p90")
					{
						MenuPlayer.ChangePlayer(0, 14, 22);
					}
					else if (MenuShop.currData.name2 == "pkp")
					{
						MenuPlayer.ChangePlayer(0, 24, 22);
					}
					else if (MenuShop.currData.name2 == "qbz95")
					{
						MenuPlayer.ChangePlayer(0, 15, 22);
					}
					else if (MenuShop.currData.name2 == "remington")
					{
						MenuPlayer.ChangePlayer(0, 5, 22);
					}
					else if (MenuShop.currData.name2 == "spas12")
					{
						MenuPlayer.ChangePlayer(0, 16, 8);
					}
					else if (MenuShop.currData.name2 == "svd")
					{
						MenuPlayer.ChangePlayer(0, 19, 22);
					}
					else if (MenuShop.currData.name2 == "ump45")
					{
						MenuPlayer.ChangePlayer(0, 17, 22);
					}
					MenuPlayer.SetPosition(0.9f, 0.03f, -1f);
				}
			}
			result = true;
		}
		return result;
	}

	private void Update()
	{
		if (!GameData.gSteam)
		{
			return;
		}
		if (!MenuShop.inbuy)
		{
			return;
		}
		Steam.RunCallBacks();
		ulong num = Steam.s_get_tnx_orderid();
		if (num != 0uL)
		{
			MenuShop.inbuy = false;
			WebHandler.get_buyfin("&orderid=" + num.ToString());
		}
	}
}
