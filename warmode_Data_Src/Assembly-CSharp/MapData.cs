using System;
using System.Collections.Generic;
using UnityEngine;

public class MapData : MonoBehaviour
{
	public class CMapData
	{
		public string name;

		public string clearName;

		public Sprite previewSmall;

		public CMapData(string name, string clearName)
		{
			this.name = name;
			this.clearName = clearName;
		}
	}

	public static List<MapData.CMapData> maps = new List<MapData.CMapData>
	{
		new MapData.CMapData("INDUSTRIAL", "02"),
		new MapData.CMapData("HANGARS", "03"),
		new MapData.CMapData("CONSTRUCTION", "05"),
		new MapData.CMapData("CATACOMBS", "09"),
		new MapData.CMapData("DEPOT", "11"),
		new MapData.CMapData("INDUSTRY", "14"),
		new MapData.CMapData("HIGHRISE", "17"),
		new MapData.CMapData("BASEMENT", "18"),
		new MapData.CMapData("CANALS", "19"),
		new MapData.CMapData("QUIET", "22"),
		new MapData.CMapData("ROCKTOWN", "23"),
		new MapData.CMapData("FACTORY", "24"),
		new MapData.CMapData("PRIPYAT", "25"),
		new MapData.CMapData("AIM#1", "26"),
		new MapData.CMapData("AIM#2", "27"),
		new MapData.CMapData("AIM#3", "29"),
		new MapData.CMapData("COLD", "30"),
		new MapData.CMapData("RUST", "31"),
		new MapData.CMapData("UNDERGROUND", "32"),
		new MapData.CMapData("HOUSE", "34")
	};

	public static MapData.CMapData GetMap(string clearName)
	{
		foreach (MapData.CMapData current in MapData.maps)
		{
			if (current.clearName == clearName)
			{
				return current;
			}
		}
		return null;
	}

	public static string GetName(string clearName)
	{
		foreach (MapData.CMapData current in MapData.maps)
		{
			if (current.clearName == clearName)
			{
				return current.name;
			}
		}
		return string.Empty;
	}
}
