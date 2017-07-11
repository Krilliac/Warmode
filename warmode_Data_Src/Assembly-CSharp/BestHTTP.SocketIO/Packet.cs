using BestHTTP.JSON;
using BestHTTP.SocketIO.JsonEncoders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BestHTTP.SocketIO
{
	public sealed class Packet
	{
		private enum PayloadTypes : byte
		{
			Textual,
			Binary
		}

		private const string Placeholder = "_placeholder";

		private List<byte[]> attachments;

		public TransportEventTypes TransportEvent
		{
			get;
			private set;
		}

		public SocketIOEventTypes SocketIOEvent
		{
			get;
			private set;
		}

		public int AttachmentCount
		{
			get;
			private set;
		}

		public int Id
		{
			get;
			private set;
		}

		public string Namespace
		{
			get;
			private set;
		}

		public string Payload
		{
			get;
			private set;
		}

		public string EventName
		{
			get;
			private set;
		}

		public List<byte[]> Attachments
		{
			get
			{
				return this.attachments;
			}
			set
			{
				this.attachments = value;
				this.AttachmentCount = ((this.attachments == null) ? 0 : this.attachments.Count);
			}
		}

		public bool HasAllAttachment
		{
			get
			{
				return this.Attachments != null && this.Attachments.Count == this.AttachmentCount;
			}
		}

		public bool IsDecoded
		{
			get;
			private set;
		}

		public object[] DecodedArgs
		{
			get;
			private set;
		}

		internal Packet()
		{
			this.TransportEvent = TransportEventTypes.Unknown;
			this.SocketIOEvent = SocketIOEventTypes.Unknown;
			this.Payload = string.Empty;
		}

		internal Packet(string from)
		{
			this.Parse(from);
		}

		internal Packet(TransportEventTypes transportEvent, SocketIOEventTypes packetType, string nsp, string payload, int attachment = 0, int id = 0)
		{
			this.TransportEvent = transportEvent;
			this.SocketIOEvent = packetType;
			this.Namespace = nsp;
			this.Payload = payload;
			this.AttachmentCount = attachment;
			this.Id = id;
		}

		public object[] Decode(IJsonEncoder encoder)
		{
			if (this.IsDecoded || encoder == null)
			{
				return this.DecodedArgs;
			}
			this.IsDecoded = true;
			if (string.IsNullOrEmpty(this.Payload))
			{
				return this.DecodedArgs;
			}
			List<object> list = encoder.Decode(this.Payload);
			if (list != null && list.Count > 0)
			{
				list.RemoveAt(0);
				this.DecodedArgs = list.ToArray();
			}
			return this.DecodedArgs;
		}

		public string DecodeEventName()
		{
			if (!string.IsNullOrEmpty(this.EventName))
			{
				return this.EventName;
			}
			if (string.IsNullOrEmpty(this.Payload))
			{
				return string.Empty;
			}
			if (this.Payload[0] != '[')
			{
				return string.Empty;
			}
			int num = 1;
			while (this.Payload.Length > num && this.Payload[num] != '"' && this.Payload[num] != '\'')
			{
				num++;
			}
			if (this.Payload.Length <= num)
			{
				return string.Empty;
			}
			int num2;
			num = (num2 = num + 1);
			while (this.Payload.Length > num && this.Payload[num] != '"' && this.Payload[num] != '\'')
			{
				num++;
			}
			if (this.Payload.Length <= num)
			{
				return string.Empty;
			}
			string text = this.Payload.Substring(num2, num - num2);
			this.EventName = text;
			return text;
		}

		public string RemoveEventName(bool removeArrayMarks)
		{
			if (string.IsNullOrEmpty(this.Payload))
			{
				return string.Empty;
			}
			if (this.Payload[0] != '[')
			{
				return string.Empty;
			}
			int num = 1;
			while (this.Payload.Length > num && this.Payload[num] != '"' && this.Payload[num] != '\'')
			{
				num++;
			}
			if (this.Payload.Length <= num)
			{
				return string.Empty;
			}
			int num2 = num;
			while (this.Payload.Length > num && this.Payload[num] != ',' && this.Payload[num] != ']')
			{
				num++;
			}
			if (this.Payload.Length <= ++num)
			{
				return string.Empty;
			}
			string text = this.Payload.Remove(num2, num - num2);
			if (removeArrayMarks)
			{
				text = text.Substring(1, text.Length - 2);
			}
			return text;
		}

		public bool ReconstructAttachmentAsIndex()
		{
			return this.PlaceholderReplacer(delegate(string json, Dictionary<string, object> obj)
			{
				int num = Convert.ToInt32(obj["num"]);
				this.Payload = this.Payload.Replace(json, num.ToString());
				this.IsDecoded = false;
			});
		}

		public bool ReconstructAttachmentAsBase64()
		{
			return this.HasAllAttachment && this.PlaceholderReplacer(delegate(string json, Dictionary<string, object> obj)
			{
				int index = Convert.ToInt32(obj["num"]);
				this.Payload = this.Payload.Replace(json, string.Format("\"{0}\"", Convert.ToBase64String(this.Attachments[index])));
				this.IsDecoded = false;
			});
		}

		internal void Parse(string from)
		{
			int num = 0;
			this.TransportEvent = (TransportEventTypes)char.GetNumericValue(from, num++);
			if (from.Length > num && char.GetNumericValue(from, num) >= 0.0)
			{
				this.SocketIOEvent = (SocketIOEventTypes)char.GetNumericValue(from, num++);
			}
			else
			{
				this.SocketIOEvent = SocketIOEventTypes.Unknown;
			}
			if (this.SocketIOEvent == SocketIOEventTypes.BinaryEvent || this.SocketIOEvent == SocketIOEventTypes.BinaryAck)
			{
				int num2 = from.IndexOf('-', num);
				if (num2 == -1)
				{
					num2 = from.Length;
				}
				int attachmentCount = 0;
				int.TryParse(from.Substring(num, num2 - num), out attachmentCount);
				this.AttachmentCount = attachmentCount;
				num = num2 + 1;
			}
			if (from.Length > num && from[num] == '/')
			{
				int num3 = from.IndexOf(',', num);
				if (num3 == -1)
				{
					num3 = from.Length;
				}
				this.Namespace = from.Substring(num, num3 - num);
				num = num3 + 1;
			}
			else
			{
				this.Namespace = "/";
			}
			if (from.Length > num && char.GetNumericValue(from[num]) >= 0.0)
			{
				int num4 = num++;
				while (from.Length > num && char.GetNumericValue(from[num]) >= 0.0)
				{
					num++;
				}
				int id = 0;
				int.TryParse(from.Substring(num4, num - num4), out id);
				this.Id = id;
			}
			if (from.Length > num)
			{
				this.Payload = from.Substring(num);
			}
			else
			{
				this.Payload = string.Empty;
			}
		}

		internal string Encode()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.TransportEvent == TransportEventTypes.Unknown && this.AttachmentCount > 0)
			{
				this.TransportEvent = TransportEventTypes.Message;
			}
			if (this.TransportEvent != TransportEventTypes.Unknown)
			{
				stringBuilder.Append(((int)this.TransportEvent).ToString());
			}
			if (this.SocketIOEvent == SocketIOEventTypes.Unknown && this.AttachmentCount > 0)
			{
				this.SocketIOEvent = SocketIOEventTypes.BinaryEvent;
			}
			if (this.SocketIOEvent != SocketIOEventTypes.Unknown)
			{
				stringBuilder.Append(((int)this.SocketIOEvent).ToString());
			}
			if (this.SocketIOEvent == SocketIOEventTypes.BinaryEvent || this.SocketIOEvent == SocketIOEventTypes.BinaryAck)
			{
				stringBuilder.Append(this.AttachmentCount.ToString());
				stringBuilder.Append("-");
			}
			bool flag = false;
			if (this.Namespace != "/")
			{
				stringBuilder.Append(this.Namespace);
				flag = true;
			}
			if (this.Id != 0)
			{
				if (flag)
				{
					stringBuilder.Append(",");
					flag = false;
				}
				stringBuilder.Append(this.Id.ToString());
			}
			if (!string.IsNullOrEmpty(this.Payload))
			{
				if (flag)
				{
					stringBuilder.Append(",");
				}
				stringBuilder.Append(this.Payload);
			}
			return stringBuilder.ToString();
		}

		internal byte[] EncodeBinary()
		{
			if (this.AttachmentCount != 0 || (this.Attachments != null && this.Attachments.Count != 0))
			{
				if (this.Attachments == null)
				{
					throw new ArgumentException("packet.Attachments are null!");
				}
				if (this.AttachmentCount != this.Attachments.Count)
				{
					throw new ArgumentException("packet.AttachmentCount != packet.Attachments.Count. Use the packet.AddAttachment function to add data to a packet!");
				}
			}
			string s = this.Encode();
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			byte[] array = this.EncodeData(bytes, Packet.PayloadTypes.Textual, null);
			if (this.AttachmentCount != 0)
			{
				int num = array.Length;
				List<byte[]> list = new List<byte[]>(this.AttachmentCount);
				int num2 = 0;
				for (int i = 0; i < this.AttachmentCount; i++)
				{
					byte[] array2 = this.EncodeData(this.Attachments[i], Packet.PayloadTypes.Binary, new byte[]
					{
						4
					});
					list.Add(array2);
					num2 += array2.Length;
				}
				Array.Resize<byte>(ref array, array.Length + num2);
				for (int j = 0; j < this.AttachmentCount; j++)
				{
					byte[] array3 = list[j];
					Array.Copy(array3, 0, array, num, array3.Length);
					num += array3.Length;
				}
			}
			return array;
		}

		internal void AddAttachmentFromServer(byte[] data, bool copyFull)
		{
			if (data == null || data.Length == 0)
			{
				return;
			}
			if (this.attachments == null)
			{
				this.attachments = new List<byte[]>(this.AttachmentCount);
			}
			if (copyFull)
			{
				this.Attachments.Add(data);
			}
			else
			{
				byte[] array = new byte[data.Length - 1];
				Array.Copy(data, 1, array, 0, data.Length - 1);
				this.Attachments.Add(array);
			}
		}

		private byte[] EncodeData(byte[] data, Packet.PayloadTypes type, byte[] afterHeaderData)
		{
			int num = (afterHeaderData == null) ? 0 : afterHeaderData.Length;
			string text = (data.Length + num).ToString();
			byte[] array = new byte[text.Length];
			for (int i = 0; i < text.Length; i++)
			{
				array[i] = (byte)char.GetNumericValue(text[i]);
			}
			byte[] array2 = new byte[data.Length + array.Length + 2 + num];
			array2[0] = (byte)type;
			for (int j = 0; j < array.Length; j++)
			{
				array2[1 + j] = array[j];
			}
			int num2 = 1 + array.Length;
			array2[num2++] = 255;
			if (afterHeaderData != null && afterHeaderData.Length > 0)
			{
				Array.Copy(afterHeaderData, 0, array2, num2, afterHeaderData.Length);
				num2 += afterHeaderData.Length;
			}
			Array.Copy(data, 0, array2, num2, data.Length);
			return array2;
		}

		private bool PlaceholderReplacer(Action<string, Dictionary<string, object>> onFound)
		{
			if (string.IsNullOrEmpty(this.Payload))
			{
				return false;
			}
			for (int i = this.Payload.IndexOf("_placeholder"); i >= 0; i = this.Payload.IndexOf("_placeholder"))
			{
				int num = i;
				while (this.Payload[num] != '{')
				{
					num--;
				}
				int num2 = i;
				while (this.Payload.Length > num2 && this.Payload[num2] != '}')
				{
					num2++;
				}
				if (this.Payload.Length <= num2)
				{
					return false;
				}
				string text = this.Payload.Substring(num, num2 - num + 1);
				bool flag = false;
				Dictionary<string, object> dictionary = Json.Decode(text, ref flag) as Dictionary<string, object>;
				if (!flag)
				{
					return false;
				}
				object obj;
				if (!dictionary.TryGetValue("_placeholder", out obj) || !(bool)obj)
				{
					return false;
				}
				if (!dictionary.TryGetValue("num", out obj))
				{
					return false;
				}
				onFound(text, dictionary);
			}
			return true;
		}

		public override string ToString()
		{
			return this.Payload;
		}

		internal Packet Clone()
		{
			return new Packet(this.TransportEvent, this.SocketIOEvent, this.Namespace, this.Payload, 0, this.Id)
			{
				EventName = this.EventName,
				AttachmentCount = this.AttachmentCount,
				attachments = this.attachments
			};
		}
	}
}
