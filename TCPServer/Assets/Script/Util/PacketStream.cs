using System;
using System.Text;

namespace UNITY_TCPCLIENT
{
    /// <summary>
    /// 2015/12/05 Hyeon
    /// PacketStream
    /// 패킷스트림
    /// </summary>
    public class PacketStream
    {
        private byte[] mBuffer;
        private int mPosition;
        private int mSize;

        public byte[] Buffer
        {
            get { return mBuffer; }
        }

        public int Position
        {
            get { return mPosition; }
            set { mPosition = value; }
        }

        public int Size
        {
            get { return mSize; }
        }

        public PacketStream(byte[] buffer, int bytes)
        {
            mBuffer = buffer;
            mSize = bytes;
            mPosition = 0;
        }

        public PacketStream(int bytes)
        {
            mBuffer = new byte[bytes];
            mSize = 0;
            mPosition = 0;
        }

        public Int16 Int16()
        {
            Int16 val = BitConverter.ToInt16(mBuffer, mPosition);
            mPosition += sizeof(Int16);
            return val;
        }

        public UInt16 UInt16()
        {
            UInt16 val = BitConverter.ToUInt16(mBuffer, mPosition);
            mPosition += sizeof(UInt16);
            return val;
        }

        public Int32 Int32()
        {
            Int32 val = BitConverter.ToInt32(mBuffer, mPosition);
            mPosition += sizeof(Int32);
            return val;
        }

        public UInt32 UInt32()
        {
            UInt32 val = BitConverter.ToUInt32(mBuffer, mPosition);
            mPosition += sizeof(UInt32);
            return val;
        }

        public Int64 Int64()
        {
            Int64 val = BitConverter.ToInt64(mBuffer, mPosition);
            mPosition += sizeof(Int64);
            return val;
        }

        public UInt64 UInt64()
        {
            UInt64 val = BitConverter.ToUInt64(mBuffer, mPosition);
            mPosition += sizeof(UInt64);
            return val;
        }

        public Double Double()
        {
            Double val = BitConverter.ToDouble(mBuffer, mPosition);
            mPosition += sizeof(Double);
            return val;
        }

        public string String(int len)
        {
            string val = System.Text.Encoding.UTF8.GetString(mBuffer, mPosition, len);
            mPosition += len;
            return val;
        }

        public void Push(Int16 val)
        {
            byte[] temp_buffer = BitConverter.GetBytes(val);
            temp_buffer.CopyTo(mBuffer, mPosition);
            mPosition += temp_buffer.Length;
            mSize += temp_buffer.Length;
        }

        public void Push(UInt16 val)
        {
            byte[] temp_buffer = BitConverter.GetBytes(val);
            temp_buffer.CopyTo(mBuffer, mPosition);
            mPosition += temp_buffer.Length;
            mSize += temp_buffer.Length;
        }

        public void Push(Int32 val)
        {
            byte[] temp_buffer = BitConverter.GetBytes(val);
            temp_buffer.CopyTo(mBuffer, mPosition);
            mPosition += temp_buffer.Length;
            mSize += temp_buffer.Length;
        }

        public void Push(UInt32 val)
        {
            byte[] temp_buffer = BitConverter.GetBytes(val);
            temp_buffer.CopyTo(mBuffer, mPosition);
            mPosition += temp_buffer.Length;
            mSize += temp_buffer.Length;
        }

        public void Push(Int64 val)
        {
            byte[] temp_buffer = BitConverter.GetBytes(val);
            temp_buffer.CopyTo(mBuffer, mPosition);
            mPosition += temp_buffer.Length;
            mSize += temp_buffer.Length;
        }

        public void Push(UInt64 val)
        {
            byte[] temp_buffer = BitConverter.GetBytes(val);
            temp_buffer.CopyTo(mBuffer, mPosition);
            mPosition += temp_buffer.Length;
            mSize += temp_buffer.Length;
        }

        public void Push(Double val)
        {
            byte[] temp_buffer = BitConverter.GetBytes(val);
            temp_buffer.CopyTo(mBuffer, mPosition);
            mPosition += temp_buffer.Length;
            mSize += temp_buffer.Length;
        }

        public void Push(string data)
        {
            byte[] temp_buffer = Encoding.UTF8.GetBytes(data);
            temp_buffer.CopyTo(mBuffer, mPosition);
            mPosition += temp_buffer.Length;
            mSize += temp_buffer.Length;
        }

        public void Push(byte[] data, int bytes)
        {
            System.Buffer.BlockCopy(data, 0, mBuffer, mPosition, bytes);
            mPosition += bytes;
            mSize += bytes;
        }
    }
}

