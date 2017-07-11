using System;
using UnityEngine;

public class UIManagerGame : MonoBehaviour
{
	public static UIManagerGame cs;

	private void Awake()
	{
		UIManagerGame.cs = this;
		base.gameObject.GetComponent<Vote_Dialog>().PostAwake();
		base.gameObject.GetComponent<global::Ping>().PostAwake();
		Vote_Dialog.cs.SetActive(false);
	}

	public void SetActive_VoteDialog(bool val)
	{
	}

	public void SetActive_VotePlayerList(bool val)
	{
	}
}
