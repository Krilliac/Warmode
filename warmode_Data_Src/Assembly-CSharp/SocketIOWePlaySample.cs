using BestHTTP.SocketIO;
using BestHTTP.SocketIO.Events;
using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class SocketIOWePlaySample : MonoBehaviour
{
	private enum States
	{
		Connecting,
		WaitForNick,
		Joined
	}

	private const float ratio = 1.5f;

	private string[] controls = new string[]
	{
		"left",
		"right",
		"a",
		"b",
		"up",
		"down",
		"select",
		"start"
	};

	private int MaxMessages = 50;

	private SocketIOWePlaySample.States State;

	private Socket Socket;

	private string Nick = string.Empty;

	private string messageToSend = string.Empty;

	private int connections;

	private List<string> messages = new List<string>();

	private Vector2 scrollPos;

	private Texture2D FrameTexture;

	private void Start()
	{
		SocketOptions socketOptions = new SocketOptions();
		socketOptions.AutoConnect = false;
		SocketManager socketManager = new SocketManager(new Uri("http://io.weplay.io/socket.io/"), socketOptions);
		this.Socket = socketManager.Socket;
		this.Socket.On(SocketIOEventTypes.Connect, new SocketIOCallback(this.OnConnected));
		this.Socket.On("joined", new SocketIOCallback(this.OnJoined));
		this.Socket.On("connections", new SocketIOCallback(this.OnConnections));
		this.Socket.On("join", new SocketIOCallback(this.OnJoin));
		this.Socket.On("move", new SocketIOCallback(this.OnMove));
		this.Socket.On("message", new SocketIOCallback(this.OnMessage));
		this.Socket.On("reload", new SocketIOCallback(this.OnReload));
		this.Socket.On("frame", new SocketIOCallback(this.OnFrame), false);
		this.Socket.On(SocketIOEventTypes.Error, new SocketIOCallback(this.OnError));
		socketManager.Open();
		this.State = SocketIOWePlaySample.States.Connecting;
	}

	private void OnDestroy()
	{
		this.Socket.Manager.Close();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			SampleSelector.SelectedSample.DestroyUnityObject();
		}
	}

	private void OnGUI()
	{
		switch (this.State)
		{
		case SocketIOWePlaySample.States.Connecting:
			GUIHelper.DrawArea(GUIHelper.ClientArea, true, delegate
			{
				GUILayout.BeginVertical(new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				GUIHelper.DrawCenteredText("Connecting to the server...");
				GUILayout.FlexibleSpace();
				GUILayout.EndVertical();
			});
			break;
		case SocketIOWePlaySample.States.WaitForNick:
			GUIHelper.DrawArea(GUIHelper.ClientArea, true, delegate
			{
				this.DrawLoginScreen();
			});
			break;
		case SocketIOWePlaySample.States.Joined:
			GUIHelper.DrawArea(GUIHelper.ClientArea, true, delegate
			{
				if (this.FrameTexture != null)
				{
					GUILayout.Box(this.FrameTexture, new GUILayoutOption[0]);
				}
				this.DrawControls();
				this.DrawChat(true);
			});
			break;
		}
	}

	private void DrawLoginScreen()
	{
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		GUILayout.FlexibleSpace();
		GUIHelper.DrawCenteredText("What's your nickname?");
		this.Nick = GUILayout.TextField(this.Nick, new GUILayoutOption[0]);
		if (GUILayout.Button("Join", new GUILayoutOption[0]))
		{
			this.Join();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
	}

	private void DrawControls()
	{
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("Controls:", new GUILayoutOption[0]);
		for (int i = 0; i < this.controls.Length; i++)
		{
			if (GUILayout.Button(this.controls[i], new GUILayoutOption[0]))
			{
				this.Socket.Emit("move", new object[]
				{
					this.controls[i]
				});
			}
		}
		GUILayout.Label(" Connections: " + this.connections, new GUILayoutOption[0]);
		GUILayout.EndHorizontal();
	}

	private void DrawChat(bool withInput = true)
	{
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		this.scrollPos = GUILayout.BeginScrollView(this.scrollPos, false, false, new GUILayoutOption[0]);
		for (int i = 0; i < this.messages.Count; i++)
		{
			GUILayout.Label(this.messages[i], new GUILayoutOption[]
			{
				GUILayout.MinWidth((float)Screen.width)
			});
		}
		GUILayout.EndScrollView();
		if (withInput)
		{
			GUILayout.Label("Your message: ", new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			this.messageToSend = GUILayout.TextField(this.messageToSend, new GUILayoutOption[0]);
			if (GUILayout.Button("Send", new GUILayoutOption[]
			{
				GUILayout.MaxWidth(100f)
			}))
			{
				this.SendMessage();
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.EndVertical();
	}

	private void AddMessage(string msg)
	{
		this.messages.Insert(0, msg);
		if (this.messages.Count > this.MaxMessages)
		{
			this.messages.RemoveRange(this.MaxMessages, this.messages.Count - this.MaxMessages);
		}
	}

	private void SendMessage()
	{
		if (string.IsNullOrEmpty(this.messageToSend))
		{
			return;
		}
		this.Socket.Emit("message", new object[]
		{
			this.messageToSend
		});
		this.AddMessage(string.Format("{0}: {1}", this.Nick, this.messageToSend));
		this.messageToSend = string.Empty;
	}

	private void Join()
	{
		PlayerPrefs.SetString("Nick", this.Nick);
		this.Socket.Emit("join", new object[]
		{
			this.Nick
		});
	}

	private void Reload()
	{
		this.FrameTexture = null;
		if (this.Socket != null)
		{
			this.Socket.Manager.Close();
			this.Socket = null;
			this.Start();
		}
	}

	private void OnConnected(Socket socket, Packet packet, params object[] args)
	{
		if (PlayerPrefs.HasKey("Nick"))
		{
			this.Nick = PlayerPrefs.GetString("Nick", "NickName");
			this.Join();
		}
		else
		{
			this.State = SocketIOWePlaySample.States.WaitForNick;
		}
		this.AddMessage("connected");
	}

	private void OnJoined(Socket socket, Packet packet, params object[] args)
	{
		this.State = SocketIOWePlaySample.States.Joined;
	}

	private void OnReload(Socket socket, Packet packet, params object[] args)
	{
		this.Reload();
	}

	private void OnMessage(Socket socket, Packet packet, params object[] args)
	{
		if (args.Length == 1)
		{
			this.AddMessage(args[0] as string);
		}
		else
		{
			this.AddMessage(string.Format("{0}: {1}", args[1], args[0]));
		}
	}

	private void OnMove(Socket socket, Packet packet, params object[] args)
	{
		this.AddMessage(string.Format("{0} pressed {1}", args[1], args[0]));
	}

	private void OnJoin(Socket socket, Packet packet, params object[] args)
	{
		string arg = (args.Length <= 1) ? string.Empty : string.Format("({0})", args[1]);
		this.AddMessage(string.Format("{0} joined {1}", args[0], arg));
	}

	private void OnConnections(Socket socket, Packet packet, params object[] args)
	{
		this.connections = Convert.ToInt32(args[0]);
	}

	private void OnFrame(Socket socket, Packet packet, params object[] args)
	{
		if (this.State != SocketIOWePlaySample.States.Joined)
		{
			return;
		}
		if (this.FrameTexture == null)
		{
			this.FrameTexture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
			this.FrameTexture.filterMode = FilterMode.Point;
		}
		byte[] data = packet.Attachments[0];
		this.FrameTexture.LoadImage(data);
	}

	private void OnError(Socket socket, Packet packet, params object[] args)
	{
		this.AddMessage(string.Format("--ERROR - {0}", args[0].ToString()));
	}
}
