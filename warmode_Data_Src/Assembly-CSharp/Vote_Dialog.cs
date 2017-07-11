using System;
using UnityEngine;
using UnityEngine.UI;

public class Vote_Dialog : MonoBehaviour
{
	public static Vote_Dialog cs;

	public GameObject voteDialog;

	private bool show;

	public Text text_kickPlayer;

	public Text text_starterPlayer;

	public Text text_voteYes;

	public Text text_voteNo;

	public GameObject answerVisual;

	public Text text_voteAnswer;

	private int voteYes;

	private int voteNo;

	private bool canAnswer;

	public void PostAwake()
	{
		Vote_Dialog.cs = this;
		this.SetActive(false);
	}

	public void SetActive(bool val)
	{
		this.voteDialog.SetActive(val);
		this.show = val;
	}

	public void SendVoteStart(byte kickId)
	{
		Client.cs.send_votestart(kickId);
	}

	public void StartVote(int kickId, int starterId)
	{
		this.SetActive(true);
		this.text_kickPlayer.text = "Kick " + PlayerControll.Player[kickId].Name;
		this.text_starterPlayer.text = "Start by " + PlayerControll.Player[starterId].Name;
		this.voteYes = 0;
		this.voteNo = 0;
		if (Client.ID == starterId)
		{
			this.SetYourAnswer(1);
		}
		else
		{
			this.SetYourAnswer(0);
		}
		this.IncAnswer(1);
	}

	public void IncAnswer(int answer)
	{
		if (answer == 1)
		{
			this.voteYes++;
		}
		else if (answer == 2)
		{
			this.voteNo++;
		}
		this.text_voteYes.text = "YES " + ((!this.canAnswer) ? " - " : "[F1] - ") + this.voteYes.ToString();
		this.text_voteNo.text = "NO " + ((!this.canAnswer) ? " - " : "[F2] - ") + this.voteNo.ToString();
	}

	public void EndVote()
	{
		this.SetActive(false);
	}

	public void SetYourAnswer(byte answer)
	{
		if (answer == 0)
		{
			this.answerVisual.SetActive(false);
			this.canAnswer = true;
			return;
		}
		this.text_voteAnswer.text = "Your answer: " + ((answer != 1) ? "NO" : "YES");
		this.answerVisual.SetActive(true);
		this.canAnswer = false;
	}

	private void Update()
	{
		if (!this.show)
		{
			return;
		}
		if (!this.canAnswer)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.F1))
		{
			Client.cs.send_vote(1);
			this.SetYourAnswer(1);
		}
		if (Input.GetKeyDown(KeyCode.F2))
		{
			Client.cs.send_vote(2);
			this.SetYourAnswer(2);
		}
	}
}
