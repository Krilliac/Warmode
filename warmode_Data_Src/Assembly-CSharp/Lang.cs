using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Lang : MonoBehaviour
{
	public static int langid = 0;

	private static Dictionary<string, string> Lang_EN = new Dictionary<string, string>
	{
		{
			"#LANGNAME",
			"EN"
		},
		{
			"_LANGUAGE",
			"LANGUAGE"
		},
		{
			"_NICKNAME",
			"NICKNAME"
		},
		{
			"_FRIENDS",
			"FRIENDS"
		},
		{
			"_PLAY",
			"PLAY"
		},
		{
			"_SHOP",
			"SHOP"
		},
		{
			"_OPTIONS",
			"OPTIONS"
		},
		{
			"_MUTE",
			"MUTE"
		},
		{
			"_FULLSCREEN_F9",
			"FULLSCREEN F9"
		},
		{
			"_LOADING",
			"LOADING"
		},
		{
			"_STEAM_NOT_LAUNCHED",
			"STEAM NOT LAUNCHED"
		},
		{
			"_STEAM_NOT_LOGGEDON",
			"STEAM NOT LOGGEDON"
		},
		{
			"_AUTHORIZATION",
			"AUTHORIZATION"
		},
		{
			"_LOW",
			"LOW"
		},
		{
			"_MEDIUM",
			"MEDIUM"
		},
		{
			"_HIGH",
			"HIGH"
		},
		{
			"_VERY_HIGH",
			"VERY HIGH"
		},
		{
			"_CUSTOM",
			"CUSTOM"
		},
		{
			"_CANCEL",
			"CANCEL"
		},
		{
			"_SELECT",
			"SELECT"
		},
		{
			"_ON",
			"ON"
		},
		{
			"_OFF",
			"OFF"
		},
		{
			"_SAVE",
			"SAVE"
		},
		{
			"_DISPLAY_RESOLUTION",
			"DISPLAY RESOLUTION"
		},
		{
			"_PRESET",
			"PRESET"
		},
		{
			"_TEXTURES_QUALITY",
			"TEXTURES QUALITY"
		},
		{
			"_SHADOWS_QUALITY",
			"SHADOWS QUALITY"
		},
		{
			"_ANTI_ALIASING",
			"ANTIALIASING"
		},
		{
			"_COLOR_CORRECTION",
			"COLOR CORRECTION"
		},
		{
			"_POSTEFFECTS",
			"POSTEFFECTS"
		},
		{
			"_DOF",
			"DOF"
		},
		{
			"_VIG",
			"VIGNETTING"
		},
		{
			"_TONE",
			"TONE"
		},
		{
			"_SHARPNESS",
			"SHARPNESS"
		},
		{
			"_SSAO",
			"SSAO"
		},
		{
			"_NOISE",
			"NOISE"
		},
		{
			"_BRIGHTNESS",
			"BRIGHTNESS"
		},
		{
			"_GAMMA",
			"GAMMA"
		},
		{
			"_GAME_VOLUME",
			"GAME VOLUME"
		},
		{
			"_MENU_VOLUME",
			"MENU VOLUME"
		},
		{
			"_SENS",
			"MOUSE SENSITIVITY"
		},
		{
			"_ZOOM_SENS",
			"MOUSE ZOOM SENSITIVITY"
		},
		{
			"_ZOOM_LOCK",
			"WEAPON ZOOM (TOGGLE/HOLD BUTTON)"
		},
		{
			"_MODE",
			"MODE"
		},
		{
			"_MAP",
			"MAP"
		},
		{
			"_RATE",
			"RATE"
		},
		{
			"_PLAYERS",
			"PLAYERS"
		},
		{
			"_REFRESH",
			"REFRESH"
		},
		{
			"_CONNECT",
			"CONNECT"
		},
		{
			"_GAMEMODE",
			"GAMEMODE"
		},
		{
			"_DEATHMATCH",
			"DEATHMATCH"
		},
		{
			"_CONFRONTATION",
			"CONFRONTATION"
		},
		{
			"_DETONATION",
			"DETONATION"
		},
		{
			"_ZOMBIEMATCH",
			"ZOMBIEMATCH"
		},
		{
			"_BUY",
			"BUY"
		},
		{
			"_PROFILE",
			"PROFILE"
		},
		{
			"_PLAYER_MENU",
			"PLAYER MENU"
		},
		{
			"_BADGE",
			"BADGE"
		},
		{
			"_BADGES",
			"BADGES"
		},
		{
			"_MASKS",
			"MASKS"
		},
		{
			"_CHANNEL",
			"CHANNEL"
		},
		{
			"_AUDIO",
			"AUDIO"
		},
		{
			"_VIDEO",
			"VIDEO"
		},
		{
			"_GAME",
			"GAME"
		},
		{
			"_PISTOLS",
			"PISTOLS"
		},
		{
			"_SHOTGUNS",
			"SHOTGUNS"
		},
		{
			"_SUBMACHUNE_GUNS",
			"SUBMACHUNE GUNS"
		},
		{
			"_ASSAULT_RIFLES",
			"ASSAULT RIFLES"
		},
		{
			"_SNIPER_RIFLES",
			"SNIPER RIFLES"
		},
		{
			"_HEAVY_RIFLES",
			"HEAVY RIFLES"
		},
		{
			"_AMMUNITION",
			"AMMUNITION"
		},
		{
			"_WEAPON_BUY",
			"BUY WEAPON"
		},
		{
			"_LAST_WEAPONS",
			"LAST WEAPONS"
		},
		{
			"_RESUME",
			"RESUME"
		},
		{
			"_HELP",
			"HELP"
		},
		{
			"_CHOOSE_TEAM",
			"CHOOSE TEAM"
		},
		{
			"_FULLSCREEN",
			"FULLSCREEN"
		},
		{
			"_EXIT_MENU",
			"EXIT MENU"
		},
		{
			"_resume_to_game",
			"resume to game"
		},
		{
			"_tip_control",
			"tip control"
		},
		{
			"_change_team_ingame",
			"change team"
		},
		{
			"_open_game_in_fullscreen",
			"open game in fullscreen"
		},
		{
			"_progress_not_saved",
			"progress not saved"
		},
		{
			"_FREECAM",
			"FREECAM"
		},
		{
			"_RESPAWN",
			"RESPAWN"
		},
		{
			"_YOU_KILLED_BY",
			"YOU KILLED BY"
		},
		{
			"_SPECTATORS",
			"SPECTATORS"
		},
		{
			"_YOU_REWARDS",
			"YOU REWARDS"
		},
		{
			"_WINNER",
			"WINNER"
		},
		{
			"_COST",
			"COST"
		},
		{
			"_ALL",
			"ALL"
		},
		{
			"_STOCK",
			"STOCK"
		},
		{
			"_ACTIVE_WEAPONS",
			"ACTIVE WEAPONS"
		},
		{
			"_OPTIONS_SAVED",
			"OPTIONS SAVED"
		},
		{
			"_CLICK_TO_EQUIP",
			"CLICK TO EQUIP"
		},
		{
			"_ASSIST",
			"ASSIST"
		},
		{
			"_BACK",
			"BACK"
		},
		{
			"_CONTROL",
			"CONTROL"
		},
		{
			"_SPRINT",
			"SPRINT"
		},
		{
			"_JUMP",
			"JUMP"
		},
		{
			"_CROUCH",
			"CROUCH"
		},
		{
			"_RELOAD",
			"RELOAD"
		},
		{
			"_LAST_WEAPON",
			"LAST WEAPON"
		},
		{
			"_FAST_KNIFE",
			"FAST KNIFE"
		},
		{
			"_THROW_GRENADE",
			"THROW GRENADE"
		},
		{
			"_SCOREBOARD",
			"SCOREBOARD"
		},
		{
			"_TEAM_CHAT",
			"TEAM CHAT"
		},
		{
			"_BUY_MENU",
			"BUY MENU"
		},
		{
			"_RESET",
			"RESET"
		},
		{
			"_INVERT_MOUSE",
			"INVERT MOUSE"
		},
		{
			"_VSYNC",
			"VSYNC"
		},
		{
			"_WEAPONS",
			"PAINTINGS"
		},
		{
			"_VOTE",
			"VOTE"
		},
		{
			"_vote_menu",
			"start vote (kick)"
		},
		{
			"_VOTE_KICK",
			"VOTE KICK"
		},
		{
			"_MOUSE_RAW_INPUT",
			"MOUSE RAW INPUT"
		},
		{
			"_CLOSE",
			"CLOSE"
		},
		{
			"_FULLTEAM",
			"TEAM IS FULL"
		},
		{
			"_PREPARE",
			"PREPARE"
		},
		{
			"_MERCS",
			"МERCS"
		},
		{
			"_CORPS",
			"CORPS"
		},
		{
			"_PEOPLE",
			"PEOPLE"
		},
		{
			"_ZOMBIE",
			"ZOMBIE"
		},
		{
			"_МERCS_WIN",
			"МERCS WIN"
		},
		{
			"_WARCORPS_WIN",
			"WARCORPS WIN"
		},
		{
			"_ZOMBIE_WIN",
			"ZOMBIE WIN"
		},
		{
			"_PEOPLE_WIN",
			"PEOPLE WIN"
		},
		{
			"_DRAW",
			"DRAW"
		},
		{
			"_TEAM_NOT_BALANCED",
			"TEAM NOT BALANCED"
		},
		{
			"_LIGHT_GAME_BUY",
			"TEXT BUY MENU"
		},
		{
			"_CREEP",
			"CREEP"
		},
		{
			"_DROP_WEAPON",
			"DROP WEAPON"
		},
		{
			"_DYNAMIC_CROSSHAIR",
			"DYNAMIC CROSSHAIR"
		},
		{
			"_AMMUNITION_BUY",
			"AMMUNITION BUY"
		},
		{
			"_YOU_BANNED!_UNBAN_COST",
			"YOU BANNED! UNBAN COST"
		},
		{
			"_UNBAN",
			"UNBAN"
		},
		{
			"_EXIT",
			"EXIT"
		},
		{
			"_WARMUP_TIME",
			"WARMUP TIME"
		},
		{
			"_WARMUP_ENDED",
			"WARMUP ENDED"
		},
		{
			"_UPDATE_VERSION",
			"PLEASE, UPDATE THE GAME"
		},
		{
			"_ADD_GOLD",
			"ADD GOLD"
		},
		{
			"_BONUS",
			"BONUS"
		},
		{
			"_ALREADY_HAVE",
			"ALREADY HAVE"
		}
	};

	private static Dictionary<string, string> Lang_RU = new Dictionary<string, string>
	{
		{
			"#LANGNAME",
			"RU"
		},
		{
			"_LANGUAGE",
			"ЯЗЫК"
		},
		{
			"_NICKNAME",
			"ПОЗЫВНОЙ"
		},
		{
			"_FRIENDS",
			"ДРУЗЬЯ"
		},
		{
			"_PLAY",
			"ИГРАТЬ"
		},
		{
			"_SHOP",
			"МАГАЗИН"
		},
		{
			"_OPTIONS",
			"ОПЦИИ"
		},
		{
			"_MUTE",
			"ЗАГЛУШИТЬ"
		},
		{
			"_FULLSCREEN_F9",
			"РАЗВЕРНУТЬ F9"
		},
		{
			"_LOADING",
			"ЗАГРУЗКА"
		},
		{
			"_STEAM_NOT_LAUNCHED",
			"STEAM НЕ ЗАПУЩЕН"
		},
		{
			"_STEAM_NOT_LOGGEDON",
			"ВЫ НЕ АВТОРИЗОВАНЫ В STEAM"
		},
		{
			"_AUTHORIZATION",
			"АВТОРИЗАЦИЯ"
		},
		{
			"_LOW",
			"НИЗКОЕ"
		},
		{
			"_MEDIUM",
			"СРЕДНЕЕ"
		},
		{
			"_HIGH",
			"ВЫСОКОЕ"
		},
		{
			"_VERY_HIGH",
			"ОЧЕНЬ ВЫСОКОЕ"
		},
		{
			"_CUSTOM",
			"СВОЕ"
		},
		{
			"_CANCEL",
			"ОТМЕНА"
		},
		{
			"_SELECT",
			"ВЫБОР"
		},
		{
			"_ON",
			"ВКЛ"
		},
		{
			"_OFF",
			"ВЫКЛ"
		},
		{
			"_SAVE",
			"СОХРАНИТЬ"
		},
		{
			"_DISPLAY_RESOLUTION",
			"РАЗРЕШЕНИЕ ЭКРАНА"
		},
		{
			"_PRESET",
			"КАЧЕСТВО ГРАФИКИ"
		},
		{
			"_TEXTURES_QUALITY",
			"КАЧЕСТВО ТЕКСТУР"
		},
		{
			"_SHADOWS_QUALITY",
			"КАЧЕСТВО ТЕНЕЙ"
		},
		{
			"_ANTI_ALIASING",
			"СГЛАЖИВАНИЕ"
		},
		{
			"_COLOR_CORRECTION",
			"ЦВЕТОКОРРЕКЦИЯ"
		},
		{
			"_POSTEFFECTS",
			"ПОСТЭФФЕКТЫ"
		},
		{
			"_DOF",
			"ГЛУБИНА РЕЗКОСТИ"
		},
		{
			"_VIG",
			"ВИНЬЕТИРОВАНИЕ"
		},
		{
			"_TONE",
			"ТОН"
		},
		{
			"_SHARPNESS",
			"РЕЗКОСТЬ"
		},
		{
			"_SSAO",
			"SSAO"
		},
		{
			"_NOISE",
			"ШУМ"
		},
		{
			"_BRIGHTNESS",
			"ЯРКОСТЬ"
		},
		{
			"_GAMMA",
			"ГАММА"
		},
		{
			"_GAME_VOLUME",
			"ГРОМКОСТЬ В ИГРЕ"
		},
		{
			"_MENU_VOLUME",
			"ГРОМКОСТЬ В МЕНЮ"
		},
		{
			"_SENS",
			"ЧУВСТВИТЕЛЬНОСТЬ МЫШИ"
		},
		{
			"_ZOOM_SENS",
			"ЧУВСТВИТЕЛЬНОСТЬ МЫШИ (ПРИЦЕЛИВАНИЕ)"
		},
		{
			"_ZOOM_LOCK",
			"ПРИЦЕЛИВАНИЕ (ПЕРЕКЛЮЧЕНИЕ/УДЕРЖИВАНИЕ)"
		},
		{
			"_MODE",
			"РЕЖИМ"
		},
		{
			"_MAP",
			"КАРТА"
		},
		{
			"_RATE",
			"РЕЙТ"
		},
		{
			"_PLAYERS",
			"ИГРОКИ"
		},
		{
			"_REFRESH",
			"ОБНОВИТЬ"
		},
		{
			"_CONNECT",
			"ПОДКЛЮЧИТЬСЯ"
		},
		{
			"_GAMEMODE",
			"РЕЖИМ ИГРЫ"
		},
		{
			"_DEATHMATCH",
			"СХВАТКА"
		},
		{
			"_CONFRONTATION",
			"ПРОТИВОСТОЯНИЕ"
		},
		{
			"_DETONATION",
			"УНИЧТОЖЕНИЕ"
		},
		{
			"_ZOMBIEMATCH",
			"ЗОМБИМОД"
		},
		{
			"_BUY",
			"КУПИТЬ"
		},
		{
			"_PROFILE",
			"ПРОФИЛЬ"
		},
		{
			"_PLAYER_MENU",
			"МЕНЮ ИГРОКА"
		},
		{
			"_BADGE",
			"БЕЙДЖ"
		},
		{
			"_BADGES",
			"БЕЙДЖИ"
		},
		{
			"_MASKS",
			"МАСКИ"
		},
		{
			"_CHANNEL",
			"КАНАЛ"
		},
		{
			"_AUDIO",
			"АУДИО"
		},
		{
			"_VIDEO",
			"ВИДЕО"
		},
		{
			"_GAME",
			"ИГРА"
		},
		{
			"_PISTOLS",
			"ПИСТОЛЕТЫ"
		},
		{
			"_SHOTGUNS",
			"ДРОБОВИКИ"
		},
		{
			"_SUBMACHUNE_GUNS",
			"ЛЕГКИЕ АВТОМАТЫ"
		},
		{
			"_ASSAULT_RIFLES",
			"ШТУРМОВЫЕ ВИНТОВКИ"
		},
		{
			"_SNIPER_RIFLES",
			"СНАЙПЕРСКИЕ ВИНТОВКИ"
		},
		{
			"_HEAVY_RIFLES",
			"ПУЛЕМЕТЫ"
		},
		{
			"_AMMUNITION",
			"АМУНИЦИЯ"
		},
		{
			"_WEAPON_BUY",
			"ПОКУПКА ОРУЖИЯ"
		},
		{
			"_LAST_WEAPONS",
			"ПОСЛЕДНЕЕ ОРУЖИЕ"
		},
		{
			"_RESUME",
			"ПРОДОЛЖИТЬ"
		},
		{
			"_HELP",
			"ПОМОЩЬ"
		},
		{
			"_CHOOSE_TEAM",
			"ВЫБОР КОМАНДЫ"
		},
		{
			"_FULLSCREEN",
			"ВО ВЕСЬ ЭКРАН"
		},
		{
			"_EXIT_MENU",
			"МЕНЮ ВЫХОДА"
		},
		{
			"_resume_to_game",
			"вернуться в игру"
		},
		{
			"_tip_control",
			"помощь в управлении"
		},
		{
			"_change_team_ingame",
			"смена команды"
		},
		{
			"_open_game_in_fullscreen",
			"полный экран"
		},
		{
			"_progress_not_saved",
			"прогресс текущего раунда не будет сохранен"
		},
		{
			"_FREECAM",
			"СВОБОДНАЯ КАМЕРА"
		},
		{
			"_RESPAWN",
			"ТОЧКА ЗАРОЖДЕНИЯ"
		},
		{
			"_YOU_KILLED_BY",
			"ВЫ УБИТЫ"
		},
		{
			"_SPECTATORS",
			"НАБЛЮДАТЕЛИ"
		},
		{
			"_YOU_REWARDS",
			"ВАША НАГРАДА"
		},
		{
			"_WINNER",
			"ПОБЕДИТЕЛЬ"
		},
		{
			"_COST",
			"СТОИМОСТЬ"
		},
		{
			"_ALL",
			"ВСЕ"
		},
		{
			"_STOCK",
			"СТОК"
		},
		{
			"_ACTIVE_WEAPONS",
			"АКТИВНЫЕ ОРУЖИЯ"
		},
		{
			"_OPTIONS_SAVED",
			"НАСТРОЙКИ СОХРАНЕНЫ"
		},
		{
			"_CLICK_TO_EQUIP",
			"НАЖМИТЕ\nЧТОБЫ ОДЕТЬ"
		},
		{
			"_ASSIST",
			"ПОМОЩЬ"
		},
		{
			"_BACK",
			"НАЗАД"
		},
		{
			"_CONTROL",
			"УПРАВЛЕНИЕ"
		},
		{
			"_SPRINT",
			"СПРИНТ"
		},
		{
			"_JUMP",
			"ПРЫЖОК"
		},
		{
			"_CROUCH",
			"ПРИСЕДАНИЕ"
		},
		{
			"_RELOAD",
			"ПЕРЕЗАРЯДКА"
		},
		{
			"_LAST_WEAPON",
			"ПОСЛЕДНЕЕ ОРУЖИЕ"
		},
		{
			"_FAST_KNIFE",
			"БЫСТРЫЙ НОЖ"
		},
		{
			"_THROW_GRENADE",
			"КИНУТЬ ГРАНАТУ"
		},
		{
			"_SCOREBOARD",
			"ТАБЛИЦА ОЧКОВ"
		},
		{
			"_TEAM_CHAT",
			"КОМАНДНЫЙ ЧАТ"
		},
		{
			"_BUY_MENU",
			"МЕНЮ ПОКУПКИ"
		},
		{
			"_RESET",
			"СБРОСИТЬ"
		},
		{
			"_INVERT_MOUSE",
			"ИНВЕРТИРОВАТЬ МЫШЬ"
		},
		{
			"_VSYNC",
			"VSYNC"
		},
		{
			"_WEAPONS",
			"ПОКРАСКА"
		},
		{
			"_VOTE",
			"ГОЛОСОВАНИЕ"
		},
		{
			"_vote_menu",
			"запустить голосование (исключение)"
		},
		{
			"_VOTE_KICK",
			"ИСКЛЮЧЕНИЕ ИЗ ИГРЫ"
		},
		{
			"_MOUSE_RAW_INPUT",
			"ПРЯМОЕ ПОДКЛЮЧЕНИЕ МЫШИ"
		},
		{
			"_CLOSE",
			"ЗАКРЫТЬ"
		},
		{
			"_FULLTEAM",
			"КОМАНДА ЗАПОЛНЕНА"
		},
		{
			"_PREPARE",
			"ПОДГОТОВКА"
		},
		{
			"_MERCS",
			"МERCS"
		},
		{
			"_CORPS",
			"CORPS"
		},
		{
			"_PEOPLE",
			"ЖИВЫЕ"
		},
		{
			"_ZOMBIE",
			"ЗОМБИ"
		},
		{
			"_МERCS_WIN",
			"МERCS ПОБЕДИЛИ"
		},
		{
			"_WARCORPS_WIN",
			"WARCORPS ПОБЕДИЛИ"
		},
		{
			"_ZOMBIE_WIN",
			"ЗОМБИ ПОБЕДИЛИ"
		},
		{
			"_PEOPLE_WIN",
			"ЖИВЫЕ ПОБЕДИЛИ"
		},
		{
			"_DRAW",
			"НИЧЬЯ"
		},
		{
			"_TEAM_NOT_BALANCED",
			"КОМАНДЫ НЕ СБАЛАНСИРОВАНЫ"
		},
		{
			"_LIGHT_GAME_BUY",
			"ТЕКСТОВОЕ МЕНЮ ПОКУПКИ"
		},
		{
			"_CREEP",
			"КРАСТЬСЯ"
		},
		{
			"_DROP_WEAPON",
			"БРОСИТЬ ОРУЖИЕ"
		},
		{
			"_DYNAMIC_CROSSHAIR",
			"ДИНАМИЧЕСКИЙ ПРИЦЕЛ"
		},
		{
			"_AMMUNITION_BUY",
			"ЗАКУП АМУНИЦИИ"
		},
		{
			"_YOU_BANNED!_UNBAN_COST",
			"ВЫ ЗАБАНЕНЫ! СТОИМОСТЬ РАЗБАНА"
		},
		{
			"_UNBAN",
			"РАЗБАН"
		},
		{
			"_EXIT",
			"ВЫХОД"
		},
		{
			"_WARMUP_TIME",
			"ИДЕТ РАЗМИНКА"
		},
		{
			"_WARMUP_ENDED",
			"РАЗМИНКА ЗАВЕРШЕНА"
		},
		{
			"_UPDATE_VERSION",
			"ПОЖАЛУЙСТА, ОБНОВИТЕ ВЕРСИЮ ИГРЫ"
		},
		{
			"_ADD_GOLD",
			"ПОПОЛНИТЬ"
		},
		{
			"_BONUS",
			"БОНУС"
		},
		{
			"_ALREADY_HAVE",
			"УЖЕ ЕСТЬ"
		}
	};

	private static List<Dictionary<string, string>> lang = new List<Dictionary<string, string>>
	{
		Lang.Lang_EN,
		Lang.Lang_RU
	};

	public static void Init()
	{
		if (PlayerPrefs.HasKey("langid"))
		{
			Lang.langid = PlayerPrefs.GetInt("langid");
		}
	}

	private static void InitLanguage()
	{
	}

	public static void CheckDublicate()
	{
		for (int i = 0; i < Lang.lang[Lang.langid].Count; i++)
		{
			for (int j = 0; j < Lang.lang[Lang.langid].Count; j++)
			{
				if (i != j)
				{
					if (Lang.lang[Lang.langid].Keys.ToList<string>()[i] == Lang.lang[Lang.langid].Keys.ToList<string>()[j])
					{
						Debug.Log("> DUBLICATE STRING " + Lang.lang[Lang.langid].Keys.ToList<string>()[i]);
					}
				}
			}
		}
	}

	public static void ChangeLanguage()
	{
		Lang.langid++;
		if (Lang.langid >= Lang.lang.Count)
		{
			Lang.langid = 0;
		}
		PlayerPrefs.SetInt("langid", Lang.langid);
	}

	public static string Get(string name)
	{
		string empty = string.Empty;
		if (Lang.lang[Lang.langid].TryGetValue(name, out empty))
		{
			return empty;
		}
		return name;
	}

	public static string GetLanguage()
	{
		return Lang.Get("#LANGNAME").ToLower();
	}
}
