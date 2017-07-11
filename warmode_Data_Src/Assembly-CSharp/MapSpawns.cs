using System;
using System.IO;
using UnityEngine;

[ExecuteInEditMode]
public class MapSpawns : MonoBehaviour
{
	public bool makeMapConfig;

	public void GetSpawns()
	{
		MonoBehaviour.print("Starting export spawns");
		string path = "spawns.txt";
		FileStream fileStream = new FileStream(path, FileMode.Create);
		StreamWriter streamWriter = new StreamWriter(fileStream);
		Spawn[] array = UnityEngine.Object.FindObjectsOfType(typeof(Spawn)) as Spawn[];
		Spawn[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			Spawn spawn = array2[i];
			if (spawn.Team == 0)
			{
				Transform transform = spawn.gameObject.transform;
				string str = string.Concat(new string[]
				{
					"team",
					spawn.Team.ToString(),
					" spawn ",
					transform.position.x.ToString(),
					" ",
					transform.position.y.ToString(),
					" ",
					transform.position.z.ToString(),
					" ",
					transform.eulerAngles.y.ToString()
				});
				streamWriter.Write(str + "\n");
			}
		}
		if (array.Length > 0)
		{
			streamWriter.Write("\n");
		}
		Spawn[] array3 = array;
		for (int j = 0; j < array3.Length; j++)
		{
			Spawn spawn2 = array3[j];
			if (spawn2.Team == 1)
			{
				Transform transform2 = spawn2.gameObject.transform;
				string str2 = string.Concat(new string[]
				{
					"team",
					spawn2.Team.ToString(),
					" spawn ",
					transform2.position.x.ToString(),
					" ",
					transform2.position.y.ToString(),
					" ",
					transform2.position.z.ToString(),
					" ",
					transform2.eulerAngles.y.ToString()
				});
				streamWriter.Write(str2 + "\n");
			}
		}
		if (array.Length > 0)
		{
			streamWriter.Write("\n");
		}
		Sector[] array4 = UnityEngine.Object.FindObjectsOfType(typeof(Sector)) as Sector[];
		Plant[] array5 = UnityEngine.Object.FindObjectsOfType(typeof(Plant)) as Plant[];
		Sector[] array6 = array4;
		for (int k = 0; k < array6.Length; k++)
		{
			Sector sector = array6[k];
			if (sector.SectorAB == 0)
			{
				Transform transform3 = sector.gameObject.transform;
				string str3 = string.Concat(new string[]
				{
					"c4a sector ",
					transform3.position.x.ToString(),
					" ",
					transform3.position.y.ToString(),
					" ",
					transform3.position.z.ToString(),
					" ",
					(transform3.localScale.x / 2f).ToString(),
					" ",
					(transform3.localScale.y / 2f).ToString(),
					" ",
					(transform3.localScale.z / 2f).ToString(),
					" "
				});
				streamWriter.Write(str3 + "\n");
			}
		}
		if (array5.Length > 0)
		{
			streamWriter.Write("\n");
		}
		Plant[] array7 = array5;
		for (int l = 0; l < array7.Length; l++)
		{
			Plant plant = array7[l];
			if (plant.PointAB == 0)
			{
				Transform transform4 = plant.gameObject.transform;
				string str4 = string.Concat(new string[]
				{
					"c4a plant ",
					transform4.position.x.ToString(),
					" ",
					transform4.position.y.ToString(),
					" ",
					transform4.position.z.ToString(),
					" ",
					(transform4.localScale.x / 2f).ToString(),
					" ",
					(transform4.localScale.y / 2f).ToString(),
					" ",
					(transform4.localScale.z / 2f).ToString(),
					" "
				});
				streamWriter.Write(str4 + "\n");
			}
		}
		if (array5.Length > 0)
		{
			streamWriter.Write("\n");
		}
		Sector[] array8 = array4;
		for (int m = 0; m < array8.Length; m++)
		{
			Sector sector2 = array8[m];
			if (sector2.SectorAB == 1)
			{
				Transform transform5 = sector2.gameObject.transform;
				string str5 = string.Concat(new string[]
				{
					"c4b sector ",
					transform5.position.x.ToString(),
					" ",
					transform5.position.y.ToString(),
					" ",
					transform5.position.z.ToString(),
					" ",
					(transform5.localScale.x / 2f).ToString(),
					" ",
					(transform5.localScale.y / 2f).ToString(),
					" ",
					(transform5.localScale.z / 2f).ToString(),
					" "
				});
				streamWriter.Write(str5 + "\n");
			}
		}
		if (array5.Length > 0)
		{
			streamWriter.Write("\n");
		}
		Plant[] array9 = array5;
		for (int n = 0; n < array9.Length; n++)
		{
			Plant plant2 = array9[n];
			if (plant2.PointAB == 1)
			{
				Transform transform6 = plant2.gameObject.transform;
				string str6 = string.Concat(new string[]
				{
					"c4b plant ",
					transform6.position.x.ToString(),
					" ",
					transform6.position.y.ToString(),
					" ",
					transform6.position.z.ToString(),
					" ",
					(transform6.localScale.x / 2f).ToString(),
					" ",
					(transform6.localScale.y / 2f).ToString(),
					" ",
					(transform6.localScale.z / 2f).ToString(),
					" "
				});
				streamWriter.Write(str6 + "\n");
			}
		}
		streamWriter.Close();
		fileStream.Close();
	}

	private void Update()
	{
		if (this.makeMapConfig)
		{
			this.makeMapConfig = false;
			this.GetSpawns();
			Debug.Log("Export end.");
		}
	}
}
