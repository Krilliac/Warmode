using BestHTTP.SocketIO;
using BestHTTP.SocketIO.Events;
using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class SocketIOChatSample : MonoBehaviour
{
	private enum ChatStates
	{
		Login,
		Chat
	}

	private readonly TimeSpan TYPING_TIMER_LENGTH = TimeSpan.FromMilliseconds(700.0);

	private SocketManager Manager;

	private SocketIOChatSample.ChatStates State;

	private string userName = string.Empty;

	private string message = string.Empty;

	private string chatLog = string.Empty;

	private Vector2 scrollPos;

	private bool typing;

	private DateTime lastTypingTime = DateTime.MinValue;

	private List<string> typingUsers = new List<string>();

	private void Start()
	{
		this.State = SocketIOChatSample.ChatStates.Login;
		SocketOptions socketOptions = new SocketOptions();
		socketOptions.AutoConnect = false;
		this.Manager = new SocketManager(new Uri("http://chat.socket.io/socket.io/"), socketOptions);
		this.Manager.Socket.On("login", new SocketIOCallback(this.OnLogin));
		this.Manager.Socket.On("new message", new SocketIOCallback(this.OnNewMessage));
		this.Manager.Socket.On("user joined", new SocketIOCallback(this.OnUserJoined));
		this.Manager.Socket.On("user left", new SocketIOCallback(this.OnUserLeft));
		this.Manager.Socket.On("typing", new SocketIOCallback(this.OnTyping));
		this.Manager.Socket.On("stop typing", new SocketIOCallback(this.OnStopTyping));
		this.Manager.Socket.On(SocketIOEventTypes.Error, delegate(Socket socket, Packet packet, object[] args)
		{
			Debug.LogError(string.Format("Error: {0}", args[0].ToString()));
		});
		this.Manager.Open();
	}

	private void OnDestroy()
	{
		this.Manager.Close();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			SampleSelector.SelectedSample.DestroyUnityObject();
		}
		if (this.typing)
		{
			DateTime utcNow = DateTime.UtcNow;
			TimeSpan t = utcNow - this.lastTypingTime;
			if (t >= this.TYPING_TIMER_LENGTH)
			{
				this.Manager.Socket.Emit("stop typing", new object[0]);
				this.typing = false;
			}
		}
	}

	private void OnGUI()
	{
		SocketIOChatSample.ChatStates state = this.State;
		if (state != SocketIOChatSample.ChatStates.Login)
		{
			if (state == SocketIOChatSample.ChatStates.Chat)
			{
				this.DrawChatScreen();
			}
		}
		else
		{
			this.DrawLoginScreen();
		}
	}

	private void DrawLoginScreen()
	{
		GUIHelper.DrawArea(GUIHelper.ClientArea, true, delegate
		{
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUIHelper.DrawCenteredText("What's your nickname?");
			this.userName = GUILayout.TextField(this.userName, new GUILayoutOption[0]);
			if (GUILayout.Button("Join", new GUILayoutOption[0]))
			{
				this.SetUserName();
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
		});
	}

	private void DrawChatScreen()
	{
		GUIHelper.DrawArea(GUIHelper.ClientArea, true, delegate
		{
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			this.scrollPos = GUILayout.BeginScrollView(this.scrollPos, new GUILayoutOption[0]);
			GUILayout.Label(this.chatLog, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true),
				GUILayout.ExpandHeight(true)
			});
			GUILayout.EndScrollView();
			string text = string.Empty;
			if (this.typingUsers.Count > 0)
			{
				text += string.Format("{0}", this.typingUsers[0]);
				for (int i = 1; i < this.typingUsers.Count; i++)
				{
					text += string.Format(", {0}", this.typingUsers[i]);
				}
				if (this.typingUsers.Count == 1)
				{
					text += " is typing!";
				}
				else
				{
					text += " are typing!";
				}
			}
			GUILayout.Label(text, new GUILayoutOption[0]);
			GUILayout.Label("Type here:", new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			this.message = GUILayout.TextField(this.message, new GUILayoutOption[0]);
			if (GUILayout.Button("Send", new GUILayoutOption[]
			{
				GUILayout.MaxWidth(100f)
			}))
			{
				this.SendMessage();
			}
			GUILayout.EndHorizontal();
			if (GUI.changed)
			{
				this.UpdateTyping();
			}
			GUILayout.EndVertical();
		});
	}

	private void SetUserName()
	{
		if (string.IsNullOrEmpty(this.userName))
		{
			return;
		}
		this.State = SocketIOChatSample.ChatStates.Chat;
		this.Manager.Socket.Emit("add user", new object[]
		{
			this.userName
		});
	}

	private void SendMessage()
	{
		if (string.IsNullOrEmpty(this.message))
		{
			return;
		}
		this.Manager.Socket.Emit("new message", new object[]
		{
			this.message
		});
		this.chatLog += string.Format("{0}: {1}\n", this.userName, this.message);
		this.message = string.Empty;
	}

	private void UpdateTyping()
	{
		if (!this.typing)
		{
			this.typing = true;
			this.Manager.Socket.Emit("typing", new object[0]);
		}
		this.lastTypingTime = DateTime.UtcNow;
	}

	private void addParticipantsMessage(Dictionary<string, object> data)
	{
		int num = Convert.ToInt32(data["numUsers"]);
		if (num == 1)
		{
			this.chatLog += "there's 1 participant\n";
		}
		else
		{
			string text = this.chatLog;
			this.chatLog = string.Concat(new object[]
			{
				text,
				"there are ",
				num,
				" participants\n"
			});
		}
	}

	private void addChatMessage(Dictionary<string, object> data)
	{
		string arg = data["username"] as string;
		string arg2 = data["message"] as string;
		this.chatLog += string.Format("{0}: {1}\n", arg, arg2);
	}

	private void AddChatTyping(Dictionary<string, object> data)
	{
		string item = data["username"] as string;
		this.typingUsers.Add(item);
	}

	private void RemoveChatTyping(Dictionary<string, object> data)
	{
		string username = data["username"] as string;
		int num = this.typingUsers.FindIndex((string name) => name.Equals(username));
		if (num != -1)
		{
			this.typingUsers.RemoveAt(num);
		}
	}

	private void OnLogin(Socket socket, Packet packet, params object[] args)
	{
		this.chatLog = "Welcome to Socket.IO Chat — \n";
		this.addParticipantsMessage(args[0] as Dictionary<string, object>);
	}

	private void OnNewMessage(Socket socket, Packet packet, params object[] args)
	{
		this.addChatMessage(args[0] as Dictionary<string, object>);
	}

	private void OnUserJoined(Socket socket, Packet packet, params object[] args)
	{
		Dictionary<string, object> dictionary = args[0] as Dictionary<string, object>;
		string arg = dictionary["username"] as string;
		this.chatLog += string.Format("{0} joined\n", arg);
		this.addParticipantsMessage(dictionary);
	}

	private void OnUserLeft(Socket socket, Packet packet, params object[] args)
	{
		Dictionary<string, object> dictionary = args[0] as Dictionary<string, object>;
		string arg = dictionary["username"] as string;
		this.chatLog += string.Format("{0} left\n", arg);
		this.addParticipantsMessage(dictionary);
	}

	private void OnTyping(Socket socket, Packet packet, params object[] args)
	{
		this.AddChatTyping(args[0] as Dictionary<string, object>);
	}

	private void OnStopTyping(Socket socket, Packet packet, params object[] args)
	{
		this.RemoveChatTyping(args[0] as Dictionary<string, object>);
	}
}
