using BestHTTP.Caching;
using BestHTTP.Cookies;
using BestHTTP.Decompression.Zlib;
using BestHTTP.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace BestHTTP
{
	public class HTTPResponse : IDisposable
	{
		internal const byte CR = 13;

		internal const byte LF = 10;

		public const int MinBufferSize = 4096;

		protected string dataAsText;

		protected Texture2D texture;

		internal HTTPRequest baseRequest;

		protected Stream Stream;

		protected List<byte[]> streamedFragments;

		protected object SyncRoot = new object();

		protected byte[] fragmentBuffer;

		protected int fragmentBufferDataLength;

		protected Stream cacheStream;

		protected int allFragmentSize;

		public int VersionMajor
		{
			get;
			protected set;
		}

		public int VersionMinor
		{
			get;
			protected set;
		}

		public int StatusCode
		{
			get;
			protected set;
		}

		public bool IsSuccess
		{
			get
			{
				return (this.StatusCode >= 200 && this.StatusCode < 300) || this.StatusCode == 304;
			}
		}

		public string Message
		{
			get;
			protected set;
		}

		public bool IsStreamed
		{
			get;
			protected set;
		}

		public bool IsStreamingFinished
		{
			get;
			internal set;
		}

		public bool IsFromCache
		{
			get;
			internal set;
		}

		public Dictionary<string, List<string>> Headers
		{
			get;
			protected set;
		}

		public byte[] Data
		{
			get;
			internal set;
		}

		public bool IsUpgraded
		{
			get;
			protected set;
		}

		public List<Cookie> Cookies
		{
			get;
			internal set;
		}

		public string DataAsText
		{
			get
			{
				if (this.Data == null)
				{
					return string.Empty;
				}
				if (!string.IsNullOrEmpty(this.dataAsText))
				{
					return this.dataAsText;
				}
				return this.dataAsText = Encoding.UTF8.GetString(this.Data, 0, this.Data.Length);
			}
		}

		public Texture2D DataAsTexture2D
		{
			get
			{
				if (this.Data == null)
				{
					return null;
				}
				if (this.texture != null)
				{
					return this.texture;
				}
				this.texture = new Texture2D(0, 0);
				this.texture.LoadImage(this.Data);
				return this.texture;
			}
		}

		internal HTTPResponse(HTTPRequest request, Stream stream, bool isStreamed, bool isFromCache)
		{
			this.baseRequest = request;
			this.Stream = stream;
			this.IsStreamed = isStreamed;
			this.IsFromCache = isFromCache;
		}

		internal virtual bool Receive(int forceReadRawContentLength = -1, bool readPayloadData = true)
		{
			string text = string.Empty;
			try
			{
				text = HTTPResponse.ReadTo(this.Stream, 32);
			}
			catch
			{
				if (!this.baseRequest.DisableRetry)
				{
					bool result = false;
					return result;
				}
				throw;
			}
			if (!this.baseRequest.DisableRetry && string.IsNullOrEmpty(text))
			{
				return false;
			}
			string[] array = text.Split(new char[]
			{
				'/',
				'.'
			});
			this.VersionMajor = int.Parse(array[1]);
			this.VersionMinor = int.Parse(array[2]);
			string text2 = HTTPResponse.NoTrimReadTo(this.Stream, 32, 10);
			int statusCode;
			if (this.baseRequest.DisableRetry)
			{
				statusCode = int.Parse(text2);
			}
			else if (!int.TryParse(text2, out statusCode))
			{
				return false;
			}
			this.StatusCode = statusCode;
			if (text2.Length > 0 && (byte)text2[text2.Length - 1] != 10 && (byte)text2[text2.Length - 1] != 13)
			{
				this.Message = HTTPResponse.ReadTo(this.Stream, 10);
			}
			else
			{
				this.Message = string.Empty;
			}
			this.ReadHeaders(this.Stream);
			this.IsUpgraded = (this.StatusCode == 101 && (this.HasHeaderWithValue("connection", "upgrade") || this.HasHeader("upgrade")));
			if (!readPayloadData)
			{
				return true;
			}
			if (forceReadRawContentLength != -1)
			{
				this.IsFromCache = true;
				this.ReadRaw(this.Stream, forceReadRawContentLength);
				return true;
			}
			if ((this.StatusCode >= 100 && this.StatusCode < 200) || this.StatusCode == 204 || this.StatusCode == 304 || this.baseRequest.MethodType == HTTPMethods.Head)
			{
				return true;
			}
			if (this.HasHeaderWithValue("transfer-encoding", "chunked"))
			{
				this.ReadChunked(this.Stream);
			}
			else
			{
				List<string> headerValues = this.GetHeaderValues("content-length");
				List<string> headerValues2 = this.GetHeaderValues("content-range");
				if (headerValues != null && headerValues2 == null)
				{
					this.ReadRaw(this.Stream, int.Parse(headerValues[0]));
				}
				else if (headerValues2 != null)
				{
					HTTPRange range = this.GetRange();
					this.ReadRaw(this.Stream, range.LastBytePos - range.FirstBytePos + 1);
				}
				else
				{
					this.ReadUnknownSize(this.Stream);
				}
			}
			return true;
		}

		protected void ReadHeaders(Stream stream)
		{
			string text = HTTPResponse.ReadTo(stream, 58, 10).Trim();
			while (text != string.Empty)
			{
				string value = HTTPResponse.ReadTo(stream, 10);
				this.AddHeader(text, value);
				text = HTTPResponse.ReadTo(stream, 58, 10);
			}
		}

		protected void AddHeader(string name, string value)
		{
			name = name.ToLower();
			if (this.Headers == null)
			{
				this.Headers = new Dictionary<string, List<string>>();
			}
			List<string> list;
			if (!this.Headers.TryGetValue(name, out list))
			{
				this.Headers.Add(name, list = new List<string>(1));
			}
			list.Add(value);
		}

		public List<string> GetHeaderValues(string name)
		{
			if (this.Headers == null)
			{
				return null;
			}
			name = name.ToLower();
			List<string> list;
			if (!this.Headers.TryGetValue(name, out list) || list.Count == 0)
			{
				return null;
			}
			return list;
		}

		public string GetFirstHeaderValue(string name)
		{
			if (this.Headers == null)
			{
				return null;
			}
			name = name.ToLower();
			List<string> list;
			if (!this.Headers.TryGetValue(name, out list) || list.Count == 0)
			{
				return null;
			}
			return list[0];
		}

		public bool HasHeaderWithValue(string headerName, string value)
		{
			List<string> headerValues = this.GetHeaderValues(headerName);
			if (headerValues == null)
			{
				return false;
			}
			for (int i = 0; i < headerValues.Count; i++)
			{
				if (string.Compare(headerValues[i], value, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return true;
				}
			}
			return false;
		}

		public bool HasHeader(string headerName)
		{
			return this.GetHeaderValues(headerName) != null;
		}

		public HTTPRange GetRange()
		{
			List<string> headerValues = this.GetHeaderValues("content-range");
			if (headerValues == null)
			{
				return null;
			}
			string[] array = headerValues[0].Split(new char[]
			{
				' ',
				'-',
				'/'
			}, StringSplitOptions.RemoveEmptyEntries);
			if (array[1] == "*")
			{
				return new HTTPRange(int.Parse(array[2]));
			}
			return new HTTPRange(int.Parse(array[1]), int.Parse(array[2]), (!(array[3] != "*")) ? -1 : int.Parse(array[3]));
		}

		public static string ReadTo(Stream stream, byte blocker)
		{
			string result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				int num = stream.ReadByte();
				while (num != (int)blocker && num != -1)
				{
					memoryStream.WriteByte((byte)num);
					num = stream.ReadByte();
				}
				result = memoryStream.ToArray().AsciiToString().Trim();
			}
			return result;
		}

		public static string ReadTo(Stream stream, byte blocker1, byte blocker2)
		{
			string result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				int num = stream.ReadByte();
				while (num != (int)blocker1 && num != (int)blocker2 && num != -1)
				{
					memoryStream.WriteByte((byte)num);
					num = stream.ReadByte();
				}
				result = memoryStream.ToArray().AsciiToString().Trim();
			}
			return result;
		}

		public static string NoTrimReadTo(Stream stream, byte blocker1, byte blocker2)
		{
			string result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				int num = stream.ReadByte();
				while (num != (int)blocker1 && num != (int)blocker2 && num != -1)
				{
					memoryStream.WriteByte((byte)num);
					num = stream.ReadByte();
				}
				result = memoryStream.ToArray().AsciiToString();
			}
			return result;
		}

		protected int ReadChunkLength(Stream stream)
		{
			return int.Parse(HTTPResponse.ReadTo(stream, 10).Split(new char[]
			{
				';'
			})[0], NumberStyles.AllowHexSpecifier);
		}

		protected void ReadChunked(Stream stream)
		{
			this.BeginReceiveStreamFragments();
			using (MemoryStream memoryStream = new MemoryStream())
			{
				int num = this.ReadChunkLength(stream);
				byte[] array = new byte[num];
				int num2 = 0;
				this.baseRequest.DownloadLength = num;
				this.baseRequest.DownloadProgressChanged = (this.IsSuccess || this.IsFromCache);
				while (num != 0)
				{
					if (array.Length < num)
					{
						Array.Resize<byte>(ref array, num);
					}
					int num3 = 0;
					this.WaitWhileHasFragments();
					do
					{
						int num4 = stream.Read(array, num3, num - num3);
						if (num4 == 0)
						{
							goto Block_5;
						}
						num3 += num4;
					}
					while (num3 < num);
					if (this.baseRequest.UseStreaming)
					{
						this.FeedStreamFragment(array, 0, num3);
					}
					else
					{
						memoryStream.Write(array, 0, num3);
					}
					HTTPResponse.ReadTo(stream, 10);
					num2 += num3;
					num = this.ReadChunkLength(stream);
					this.baseRequest.DownloadLength += num;
					this.baseRequest.Downloaded = num2;
					this.baseRequest.DownloadProgressChanged = (this.IsSuccess || this.IsFromCache);
					continue;
					Block_5:
					throw new Exception("The remote server closed the connection unexpectedly!");
				}
				if (this.baseRequest.UseStreaming)
				{
					this.FlushRemainingFragmentBuffer();
				}
				this.ReadHeaders(stream);
				if (!this.baseRequest.UseStreaming)
				{
					this.Data = this.DecodeStream(memoryStream);
				}
			}
		}

		internal void ReadRaw(Stream stream, int contentLength)
		{
			this.BeginReceiveStreamFragments();
			this.baseRequest.DownloadLength = contentLength;
			this.baseRequest.DownloadProgressChanged = (this.IsSuccess || this.IsFromCache);
			using (MemoryStream memoryStream = new MemoryStream((!this.baseRequest.UseStreaming) ? contentLength : 0))
			{
				byte[] array = new byte[Math.Max(this.baseRequest.StreamFragmentSize, 4096)];
				while (contentLength > 0)
				{
					int num = 0;
					this.WaitWhileHasFragments();
					do
					{
						int num2 = stream.Read(array, num, Math.Min(contentLength, array.Length - num));
						if (num2 == 0)
						{
							goto Block_5;
						}
						num += num2;
						contentLength -= num2;
						this.baseRequest.Downloaded += num2;
						this.baseRequest.DownloadProgressChanged = (this.IsSuccess || this.IsFromCache);
					}
					while (num < array.Length && contentLength > 0);
					if (this.baseRequest.UseStreaming)
					{
						this.FeedStreamFragment(array, 0, num);
						continue;
					}
					memoryStream.Write(array, 0, num);
					continue;
					Block_5:
					throw new Exception("The remote server closed the connection unexpectedly!");
				}
				if (this.baseRequest.UseStreaming)
				{
					this.FlushRemainingFragmentBuffer();
				}
				if (!this.baseRequest.UseStreaming)
				{
					this.Data = this.DecodeStream(memoryStream);
				}
			}
		}

		private void ReadUnknownSize(Stream stream)
		{
			NetworkStream networkStream = stream as NetworkStream;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				byte[] array = new byte[Math.Max(this.baseRequest.StreamFragmentSize, 4096)];
				int num2;
				do
				{
					int num = 0;
					do
					{
						num2 = 0;
						if (networkStream != null)
						{
							int num3 = num;
							while (num3 < array.Length && networkStream.DataAvailable)
							{
								int num4 = stream.ReadByte();
								if (num4 < 0)
								{
									break;
								}
								array[num3] = (byte)num4;
								num2++;
								num3++;
							}
						}
						else
						{
							num2 = stream.Read(array, num, array.Length - num);
						}
						num += num2;
						this.baseRequest.Downloaded += num2;
						this.baseRequest.DownloadLength = this.baseRequest.Downloaded;
						this.baseRequest.DownloadProgressChanged = (this.IsSuccess || this.IsFromCache);
					}
					while (num < array.Length && num2 > 0);
					if (this.baseRequest.UseStreaming)
					{
						this.FeedStreamFragment(array, 0, num);
					}
					else
					{
						memoryStream.Write(array, 0, num);
					}
				}
				while (num2 > 0);
				if (this.baseRequest.UseStreaming)
				{
					this.FlushRemainingFragmentBuffer();
				}
				if (!this.baseRequest.UseStreaming)
				{
					this.Data = this.DecodeStream(memoryStream);
				}
			}
		}

		protected byte[] DecodeStream(Stream streamToDecode)
		{
			streamToDecode.Seek(0L, SeekOrigin.Begin);
			List<string> list = (!this.IsFromCache) ? this.GetHeaderValues("content-encoding") : null;
			Stream stream;
			if (list == null)
			{
				stream = streamToDecode;
			}
			else
			{
				string text = list[0];
				if (text != null)
				{
					if (HTTPResponse.<>f__switch$map5 == null)
					{
						HTTPResponse.<>f__switch$map5 = new Dictionary<string, int>(2)
						{
							{
								"gzip",
								0
							},
							{
								"deflate",
								1
							}
						};
					}
					int num;
					if (HTTPResponse.<>f__switch$map5.TryGetValue(text, out num))
					{
						if (num == 0)
						{
							stream = new GZipStream(streamToDecode, CompressionMode.Decompress);
							goto IL_C1;
						}
						if (num == 1)
						{
							stream = new DeflateStream(streamToDecode, CompressionMode.Decompress);
							goto IL_C1;
						}
					}
				}
				stream = streamToDecode;
			}
			IL_C1:
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream((int)streamToDecode.Length))
			{
				byte[] array = new byte[1024];
				int count;
				while ((count = stream.Read(array, 0, array.Length)) > 0)
				{
					memoryStream.Write(array, 0, count);
				}
				result = memoryStream.ToArray();
			}
			return result;
		}

		protected void BeginReceiveStreamFragments()
		{
			if (!this.baseRequest.DisableCache && this.baseRequest.UseStreaming && !this.IsFromCache && HTTPCacheService.IsCacheble(this.baseRequest.CurrentUri, this.baseRequest.MethodType, this))
			{
				this.cacheStream = HTTPCacheService.PrepareStreamed(this.baseRequest.CurrentUri, this);
			}
			this.allFragmentSize = 0;
		}

		protected void FeedStreamFragment(byte[] buffer, int pos, int length)
		{
			if (this.fragmentBuffer == null)
			{
				this.fragmentBuffer = new byte[this.baseRequest.StreamFragmentSize];
				this.fragmentBufferDataLength = 0;
			}
			if (this.fragmentBufferDataLength + length <= this.baseRequest.StreamFragmentSize)
			{
				Array.Copy(buffer, pos, this.fragmentBuffer, this.fragmentBufferDataLength, length);
				this.fragmentBufferDataLength += length;
				if (this.fragmentBufferDataLength == this.baseRequest.StreamFragmentSize)
				{
					this.AddStreamedFragment(this.fragmentBuffer);
					this.fragmentBuffer = null;
					this.fragmentBufferDataLength = 0;
				}
			}
			else
			{
				int num = this.baseRequest.StreamFragmentSize - this.fragmentBufferDataLength;
				this.FeedStreamFragment(buffer, pos, num);
				this.FeedStreamFragment(buffer, pos + num, length - num);
			}
		}

		protected void FlushRemainingFragmentBuffer()
		{
			if (this.fragmentBuffer != null)
			{
				Array.Resize<byte>(ref this.fragmentBuffer, this.fragmentBufferDataLength);
				this.AddStreamedFragment(this.fragmentBuffer);
				this.fragmentBuffer = null;
				this.fragmentBufferDataLength = 0;
			}
			if (this.cacheStream != null)
			{
				this.cacheStream.Dispose();
				this.cacheStream = null;
				HTTPCacheService.SetBodyLength(this.baseRequest.CurrentUri, this.allFragmentSize);
			}
		}

		protected void AddStreamedFragment(byte[] buffer)
		{
			object syncRoot = this.SyncRoot;
			lock (syncRoot)
			{
				if (this.streamedFragments == null)
				{
					this.streamedFragments = new List<byte[]>();
				}
				this.streamedFragments.Add(buffer);
				if (this.cacheStream != null)
				{
					this.cacheStream.Write(buffer, 0, buffer.Length);
					this.allFragmentSize += buffer.Length;
				}
			}
		}

		protected void WaitWhileHasFragments()
		{
		}

		public List<byte[]> GetStreamedFragments()
		{
			object syncRoot = this.SyncRoot;
			List<byte[]> result;
			lock (syncRoot)
			{
				if (this.streamedFragments == null || this.streamedFragments.Count == 0)
				{
					result = null;
				}
				else
				{
					List<byte[]> list = new List<byte[]>(this.streamedFragments);
					this.streamedFragments.Clear();
					result = list;
				}
			}
			return result;
		}

		internal bool HasStreamedFragments()
		{
			object syncRoot = this.SyncRoot;
			bool result;
			lock (syncRoot)
			{
				result = (this.streamedFragments != null && this.streamedFragments.Count > 0);
			}
			return result;
		}

		internal void FinishStreaming()
		{
			this.IsStreamingFinished = true;
			this.Dispose();
		}

		public void Dispose()
		{
			if (this.cacheStream != null)
			{
				this.cacheStream.Dispose();
				this.cacheStream = null;
			}
		}
	}
}
