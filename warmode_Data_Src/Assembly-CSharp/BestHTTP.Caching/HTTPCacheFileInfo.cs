using BestHTTP.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace BestHTTP.Caching
{
	internal class HTTPCacheFileInfo : IComparable<HTTPCacheFileInfo>
	{
		internal Uri Uri
		{
			get;
			set;
		}

		internal DateTime LastAccess
		{
			get;
			set;
		}

		internal int BodyLength
		{
			get;
			set;
		}

		private string ETag
		{
			get;
			set;
		}

		private string LastModified
		{
			get;
			set;
		}

		private DateTime Expires
		{
			get;
			set;
		}

		private long Age
		{
			get;
			set;
		}

		private long MaxAge
		{
			get;
			set;
		}

		private DateTime Date
		{
			get;
			set;
		}

		private bool MustRevalidate
		{
			get;
			set;
		}

		private DateTime Received
		{
			get;
			set;
		}

		private string ConstructedPath
		{
			get;
			set;
		}

		internal HTTPCacheFileInfo(Uri uri) : this(uri, DateTime.UtcNow, -1)
		{
		}

		internal HTTPCacheFileInfo(Uri uri, DateTime lastAcces, int bodyLength)
		{
			this.Uri = uri;
			this.LastAccess = lastAcces;
			this.BodyLength = bodyLength;
			this.MaxAge = -1L;
		}

		internal HTTPCacheFileInfo(Uri uri, BinaryReader reader, int version)
		{
			this.Uri = uri;
			this.LastAccess = DateTime.FromBinary(reader.ReadInt64());
			this.BodyLength = reader.ReadInt32();
			if (version == 1)
			{
				this.ETag = reader.ReadString();
				this.LastModified = reader.ReadString();
				this.Expires = DateTime.FromBinary(reader.ReadInt64());
				this.Age = reader.ReadInt64();
				this.MaxAge = reader.ReadInt64();
				this.Date = DateTime.FromBinary(reader.ReadInt64());
				this.MustRevalidate = reader.ReadBoolean();
				this.Received = DateTime.FromBinary(reader.ReadInt64());
			}
		}

		internal void SaveTo(BinaryWriter writer)
		{
			writer.Write(this.LastAccess.ToBinary());
			writer.Write(this.BodyLength);
			writer.Write(this.ETag);
			writer.Write(this.LastModified);
			writer.Write(this.Expires.ToBinary());
			writer.Write(this.Age);
			writer.Write(this.MaxAge);
			writer.Write(this.Date.ToBinary());
			writer.Write(this.MustRevalidate);
			writer.Write(this.Received.ToBinary());
		}

		internal string GetFileName()
		{
			return Convert.ToBase64String(this.Uri.ToString().GetASCIIBytes()).Replace('/', '-');
		}

		internal string GetPath()
		{
			if (this.ConstructedPath != null)
			{
				return this.ConstructedPath;
			}
			string text = Path.Combine(HTTPCacheService.CacheFolder, this.GetFileName());
			this.ConstructedPath = text;
			return text;
		}

		internal bool IsExists()
		{
			return File.Exists(this.GetPath());
		}

		internal void Delete()
		{
			string path = this.GetPath();
			try
			{
				File.Delete(path);
			}
			catch
			{
			}
			finally
			{
				this.Reset();
			}
		}

		private void Reset()
		{
			this.BodyLength = -1;
			this.ETag = string.Empty;
			this.Expires = DateTime.FromBinary(0L);
			this.LastModified = string.Empty;
			this.Age = 0L;
			this.MaxAge = -1L;
			this.Date = DateTime.FromBinary(0L);
			this.MustRevalidate = false;
			this.Received = DateTime.FromBinary(0L);
		}

		private void SetUpCachingValues(HTTPResponse response)
		{
			this.ETag = response.GetFirstHeaderValue("ETag").ToStrOrEmpty();
			this.Expires = response.GetFirstHeaderValue("Expires").ToDateTime(DateTime.FromBinary(0L));
			this.LastModified = response.GetFirstHeaderValue("Last-Modified").ToStrOrEmpty();
			this.Age = response.GetFirstHeaderValue("Age").ToInt64(0L);
			this.Date = response.GetFirstHeaderValue("Date").ToDateTime(DateTime.FromBinary(0L));
			string firstHeaderValue = response.GetFirstHeaderValue("cache-control");
			if (!string.IsNullOrEmpty(firstHeaderValue))
			{
				string[] array = firstHeaderValue.FindOption("Max-Age");
				double num;
				if (array != null && double.TryParse(array[1], out num))
				{
					this.MaxAge = (long)((int)num);
				}
				this.MustRevalidate = firstHeaderValue.ToLower().Contains("must-revalidate");
			}
			this.Received = DateTime.UtcNow;
		}

		internal bool WillExpireInTheFuture()
		{
			if (!this.IsExists())
			{
				return false;
			}
			if (this.MustRevalidate)
			{
				return false;
			}
			if (this.MaxAge != -1L)
			{
				long val = Math.Max(0L, (long)(this.Received - this.Date).TotalSeconds);
				long num = Math.Max(val, this.Age);
				long num2 = (long)(DateTime.UtcNow - this.Date).TotalSeconds;
				long num3 = num + num2;
				return num3 < this.MaxAge;
			}
			return this.Expires > DateTime.UtcNow;
		}

		internal void SetUpRevalidationHeaders(HTTPRequest request)
		{
			if (!this.IsExists())
			{
				return;
			}
			if (!string.IsNullOrEmpty(this.ETag))
			{
				request.AddHeader("If-None-Match", this.ETag);
			}
			if (!string.IsNullOrEmpty(this.LastModified))
			{
				request.AddHeader("If-Modified-Since", this.LastModified);
			}
		}

		internal Stream GetBodyStream(out int length)
		{
			if (!this.IsExists())
			{
				length = 0;
				return null;
			}
			length = this.BodyLength;
			this.LastAccess = DateTime.UtcNow;
			FileStream fileStream = new FileStream(this.GetPath(), FileMode.Open);
			fileStream.Seek((long)(-(long)length), SeekOrigin.End);
			return fileStream;
		}

		internal HTTPResponse ReadResponseTo(HTTPRequest request)
		{
			if (!this.IsExists())
			{
				return null;
			}
			this.LastAccess = DateTime.UtcNow;
			HTTPResponse result;
			using (FileStream fileStream = new FileStream(this.GetPath(), FileMode.Open))
			{
				HTTPResponse hTTPResponse = new HTTPResponse(request, fileStream, request.UseStreaming, true);
				hTTPResponse.Receive(this.BodyLength, true);
				result = hTTPResponse;
			}
			return result;
		}

		internal void Store(HTTPResponse response)
		{
			string path = this.GetPath();
			if (path.Length > HTTPManager.MaxPathLength)
			{
				return;
			}
			if (File.Exists(path))
			{
				this.Delete();
			}
			using (FileStream fileStream = new FileStream(path, FileMode.Create))
			{
				fileStream.WriteLine("HTTP/1.1 {0} {1}", new object[]
				{
					response.StatusCode,
					response.Message
				});
				foreach (KeyValuePair<string, List<string>> current in response.Headers)
				{
					for (int i = 0; i < current.Value.Count; i++)
					{
						fileStream.WriteLine("{0}: {1}", new object[]
						{
							current.Key,
							current.Value[i]
						});
					}
				}
				fileStream.WriteLine();
				fileStream.Write(response.Data, 0, response.Data.Length);
			}
			this.BodyLength = response.Data.Length;
			this.LastAccess = DateTime.UtcNow;
			this.SetUpCachingValues(response);
		}

		internal Stream GetSaveStream(HTTPResponse response)
		{
			this.LastAccess = DateTime.UtcNow;
			string path = this.GetPath();
			if (File.Exists(path))
			{
				this.Delete();
			}
			if (path.Length > HTTPManager.MaxPathLength)
			{
				return null;
			}
			using (FileStream fileStream = new FileStream(path, FileMode.Create))
			{
				fileStream.WriteLine("HTTP/1.1 {0} {1}", new object[]
				{
					response.StatusCode,
					response.Message
				});
				foreach (KeyValuePair<string, List<string>> current in response.Headers)
				{
					for (int i = 0; i < current.Value.Count; i++)
					{
						fileStream.WriteLine("{0}: {1}", new object[]
						{
							current.Key,
							current.Value[i]
						});
					}
				}
				fileStream.WriteLine();
			}
			if (response.IsFromCache && !response.Headers.ContainsKey("content-length"))
			{
				response.Headers.Add("content-length", new List<string>
				{
					this.BodyLength.ToString()
				});
			}
			this.SetUpCachingValues(response);
			return new FileStream(this.GetPath(), FileMode.Append);
		}

		public int CompareTo(HTTPCacheFileInfo other)
		{
			return this.LastAccess.CompareTo(other.LastAccess);
		}
	}
}
