using System;
using UnityEngine;

public class ContentLoader2_ : MonoBehaviour
{
	public static float progress;

	private void Update()
	{
		if (ContentLoader_.LoadList.Count == 0)
		{
			return;
		}
		if (ContentLoader_.inDownload)
		{
			return;
		}
		this.Load(ContentLoader_.LoadList[0].name, ContentLoader_.LoadList[0].version);
		ContentLoader_.currBundle = ContentLoader_.LoadList[0];
		ContentLoader_.LoadList.RemoveAt(0);
		if (ContentLoader_.inDownload || ContentLoader_.LoadList.Count > 0)
		{
			float num = 0f;
			if (ContentLoader_.www != null)
			{
				num = ContentLoader_.www.progress;
			}
			ContentLoader2_.progress = 1f;
			if (ContentLoader_.LoadList.Count > 0)
			{
				ContentLoader2_.progress = (float)(ContentLoader_.maxcontentcount - ContentLoader_.LoadList.Count - 1) / (float)ContentLoader_.maxcontentcount + 1f / (float)ContentLoader_.maxcontentcount * num;
			}
		}
	}

	public void Load(string svalue, int version)
	{
		base.StartCoroutine(ContentLoader_.cLoad(svalue, version));
		ContentLoader_.inDownload = true;
	}

	public void LoadEnd()
	{
	}
}
