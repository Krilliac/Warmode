using BestHTTP;
using System;
using System.IO;
using System.Threading;

public sealed class UploadStream : Stream
{
	private MemoryStream ReadBuffer = new MemoryStream();

	private MemoryStream WriteBuffer = new MemoryStream();

	private bool noMoreData;

	private AutoResetEvent ARE = new AutoResetEvent(false);

	private object locker = new object();

	public string Name
	{
		get;
		private set;
	}

	private bool IsReadBufferEmpty
	{
		get
		{
			object obj = this.locker;
			bool result;
			lock (obj)
			{
				result = (this.ReadBuffer.Position == this.ReadBuffer.Length);
			}
			return result;
		}
	}

	public override bool CanRead
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public override bool CanSeek
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public override bool CanWrite
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public override long Length
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public override long Position
	{
		get
		{
			throw new NotImplementedException();
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public UploadStream(string name) : this()
	{
		this.Name = name;
	}

	public UploadStream()
	{
		this.ReadBuffer = new MemoryStream();
		this.WriteBuffer = new MemoryStream();
		this.Name = string.Empty;
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		if (this.noMoreData)
		{
			if (this.ReadBuffer.Position != this.ReadBuffer.Length)
			{
				return this.ReadBuffer.Read(buffer, offset, count);
			}
			if (this.WriteBuffer.Length <= 0L)
			{
				HTTPManager.Logger.Information("UploadStream", string.Format("{0} - Read - End Of Stream", this.Name));
				return -1;
			}
			this.SwitchBuffers();
		}
		if (this.IsReadBufferEmpty)
		{
			this.ARE.WaitOne();
			object obj = this.locker;
			lock (obj)
			{
				if (this.IsReadBufferEmpty && this.WriteBuffer.Length > 0L)
				{
					this.SwitchBuffers();
				}
			}
		}
		int result = -1;
		object obj2 = this.locker;
		lock (obj2)
		{
			result = this.ReadBuffer.Read(buffer, offset, count);
		}
		return result;
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		if (this.noMoreData)
		{
			throw new ArgumentException("noMoreData already set!");
		}
		object obj = this.locker;
		lock (obj)
		{
			this.WriteBuffer.Write(buffer, offset, count);
			this.SwitchBuffers();
		}
		this.ARE.Set();
	}

	public override void Flush()
	{
		this.Finish();
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			HTTPManager.Logger.Information("UploadStream", string.Format("{0} - Dispose", this.Name));
			this.ReadBuffer.Dispose();
			this.ReadBuffer = null;
			this.WriteBuffer.Dispose();
			this.WriteBuffer = null;
			this.ARE.Close();
			this.ARE = null;
		}
		base.Dispose(disposing);
	}

	public void Finish()
	{
		if (this.noMoreData)
		{
			throw new ArgumentException("noMoreData already set!");
		}
		HTTPManager.Logger.Information("UploadStream", string.Format("{0} - Finish", this.Name));
		this.noMoreData = true;
		this.ARE.Set();
	}

	private bool SwitchBuffers()
	{
		object obj = this.locker;
		lock (obj)
		{
			if (this.ReadBuffer.Position == this.ReadBuffer.Length)
			{
				this.WriteBuffer.Seek(0L, SeekOrigin.Begin);
				this.ReadBuffer.SetLength(0L);
				MemoryStream writeBuffer = this.WriteBuffer;
				this.WriteBuffer = this.ReadBuffer;
				this.ReadBuffer = writeBuffer;
				return true;
			}
		}
		return false;
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		throw new NotImplementedException();
	}

	public override void SetLength(long value)
	{
		throw new NotImplementedException();
	}
}
