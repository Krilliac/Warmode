using BestHTTP;
using BestHTTP.WebSocket;
using System;
using UnityEngine;

public class WebSocketSample : MonoBehaviour
{
	private string address = "ws://echo.websocket.org";

	private string msgToSend = "Hello World!";

	private string Text = string.Empty;

	private WebSocket webSocket;

	private Vector2 scrollPos;

	private void OnDestroy()
	{
		if (this.webSocket != null)
		{
			this.webSocket.Close();
		}
	}

	private void OnGUI()
	{
		GUIHelper.DrawArea(GUIHelper.ClientArea, true, delegate
		{
			this.scrollPos = GUILayout.BeginScrollView(this.scrollPos, new GUILayoutOption[0]);
			GUILayout.Label(this.Text, new GUILayoutOption[0]);
			GUILayout.EndScrollView();
			GUILayout.Space(5f);
			GUILayout.FlexibleSpace();
			this.address = GUILayout.TextField(this.address, new GUILayoutOption[0]);
			if (this.webSocket == null && GUILayout.Button("Open Web Socket", new GUILayoutOption[0]))
			{
				this.webSocket = new WebSocket(new Uri(this.address));
				if (HTTPManager.Proxy != null)
				{
					this.webSocket.InternalRequest.Proxy = new HTTPProxy(HTTPManager.Proxy.Address, HTTPManager.Proxy.Credentials, false);
				}
				WebSocket expr_C3 = this.webSocket;
				expr_C3.OnOpen = (OnWebSocketOpenDelegate)Delegate.Combine(expr_C3.OnOpen, new OnWebSocketOpenDelegate(this.OnOpen));
				WebSocket expr_EA = this.webSocket;
				expr_EA.OnMessage = (OnWebSocketMessageDelegate)Delegate.Combine(expr_EA.OnMessage, new OnWebSocketMessageDelegate(this.OnMessageReceived));
				WebSocket expr_111 = this.webSocket;
				expr_111.OnClosed = (OnWebSocketClosedDelegate)Delegate.Combine(expr_111.OnClosed, new OnWebSocketClosedDelegate(this.OnClosed));
				WebSocket expr_138 = this.webSocket;
				expr_138.OnError = (OnWebSocketErrorDelegate)Delegate.Combine(expr_138.OnError, new OnWebSocketErrorDelegate(this.OnError));
				this.webSocket.Open();
				this.Text += "Opening Web Socket...\n";
			}
			if (this.webSocket != null && this.webSocket.IsOpen)
			{
				GUILayout.Space(10f);
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				this.msgToSend = GUILayout.TextField(this.msgToSend, new GUILayoutOption[0]);
				if (GUILayout.Button("Send", new GUILayoutOption[]
				{
					GUILayout.MaxWidth(70f)
				}))
				{
					this.Text += "Sending message...\n";
					this.webSocket.Send(this.msgToSend);
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(10f);
				if (GUILayout.Button("Close", new GUILayoutOption[0]))
				{
					this.webSocket.Close(1000, "Bye!");
				}
			}
		});
	}

	private void OnOpen(WebSocket ws)
	{
		this.Text += string.Format("-WebSocket Open!\n", new object[0]);
	}

	private void OnMessageReceived(WebSocket ws, string message)
	{
		this.Text += string.Format("-Message received: {0}\n", message);
	}

	private void OnClosed(WebSocket ws, ushort code, string message)
	{
		this.Text += string.Format("-WebSocket closed! Code: {0} Message: {1}\n", code, message);
		this.webSocket = null;
	}

	private void OnError(WebSocket ws, Exception ex)
	{
		string str = string.Empty;
		if (ws.InternalRequest.Response != null)
		{
			str = string.Format("Status Code from Server: {0} and Message: {1}", ws.InternalRequest.Response.StatusCode, ws.InternalRequest.Response.Message);
		}
		this.Text += string.Format("-An error occured: {0}\n", (ex == null) ? ("Unknown Error " + str) : ex.Message);
		this.webSocket = null;
	}
}
