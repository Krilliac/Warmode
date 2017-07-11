using System;
using System.Collections.Generic;
using UnityEngine;

public class SND : MonoBehaviour
{
	private static List<AudioClip> soundlist = new List<AudioClip>();

	private static AudioClip curSound = null;

	public static void Init()
	{
		SND.soundlist.Clear();
	}

	public static AudioClip GetSoundByName(string _name)
	{
		foreach (AudioClip current in SND.soundlist)
		{
			if (current.name == _name)
			{
				AudioClip result = current;
				return result;
			}
		}
		SND.curSound = null;
		string[] array = _name.Split(new char[]
		{
			'/'
		});
		if (array.Length > 1)
		{
			SND.curSound = ContentLoader_.LoadAudio(array[array.Length - 1]);
		}
		else
		{
			SND.curSound = ContentLoader_.LoadAudio(_name);
		}
		if (SND.curSound == null)
		{
			return null;
		}
		SND.soundlist.Add(SND.curSound);
		return SND.curSound;
	}
}
