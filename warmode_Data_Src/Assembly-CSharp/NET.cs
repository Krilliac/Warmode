using System;
using System.Text;
using UnityEngine;

public class NET : MonoBehaviour
{
	private static byte[] sendbuffer = new byte[1025];

	private static int writepos = 0;

	private static byte[] readbuffer;

	private static int readlen;

	private static int readpos;

	private static bool readerror;

	public static void BEGIN_WRITE()
	{
		NET.writepos = 0;
	}

	public static void WRITE_BYTE(byte bvalue)
	{
		NET.sendbuffer[NET.writepos] = bvalue;
		NET.writepos++;
	}

	private static void WRITE_SHORT(short svalue)
	{
		byte[] array = NET.EncodeShort(svalue);
		NET.sendbuffer[NET.writepos] = array[0];
		NET.sendbuffer[NET.writepos + 1] = array[1];
		NET.writepos += 2;
	}

	public static void WRITE_FLOAT(float fvalue)
	{
		byte[] array = NET.EncodeFloat(fvalue);
		NET.sendbuffer[NET.writepos] = array[0];
		NET.sendbuffer[NET.writepos + 1] = array[1];
		NET.sendbuffer[NET.writepos + 2] = array[2];
		NET.sendbuffer[NET.writepos + 3] = array[3];
		NET.writepos += 4;
	}

	private static void WRITE_LONG(int ivalue)
	{
		byte[] array = NET.EncodeInteger(ivalue);
		NET.sendbuffer[NET.writepos] = array[0];
		NET.sendbuffer[NET.writepos + 1] = array[1];
		NET.sendbuffer[NET.writepos + 2] = array[2];
		NET.sendbuffer[NET.writepos + 3] = array[3];
		NET.writepos += 4;
	}

	public static int WRITE_LEN()
	{
		return NET.writepos;
	}

	public static void END_WRITE()
	{
		short svalue = (short)NET.writepos;
		NET.writepos = 2;
		NET.WRITE_SHORT(svalue);
		NET.writepos = (int)svalue;
	}

	public static byte[] WRITE_DATA()
	{
		return NET.sendbuffer;
	}

	public static byte[] EncodeShort(short inShort)
	{
		return BitConverter.GetBytes(inShort);
	}

	public static byte[] EncodeInteger(int inInt)
	{
		return BitConverter.GetBytes(inInt);
	}

	public static byte[] EncodeFloat(float inFloat)
	{
		return BitConverter.GetBytes(inFloat);
	}

	public static void BEGIN_READ(byte[] inBytes, int len, int startpos)
	{
		NET.readbuffer = inBytes;
		NET.readlen = len;
		NET.readpos = startpos;
		NET.readerror = false;
	}

	public static int READ_BYTE()
	{
		if (NET.readpos + 1 > NET.readlen)
		{
			NET.readerror = true;
			return 0;
		}
		int result = (int)NET.readbuffer[NET.readpos];
		NET.readpos++;
		return result;
	}

	public static int READ_SHORT()
	{
		if (NET.readpos + 2 > NET.readlen)
		{
			NET.readerror = true;
			return 0;
		}
		int result = NET.DecodeShort(NET.readbuffer, NET.readpos);
		NET.readpos += 2;
		return result;
	}

	public static int READ_LONG()
	{
		if (NET.readpos + 4 > NET.readlen)
		{
			NET.readerror = true;
			return 0;
		}
		int result = NET.DecodeInteger(NET.readbuffer, NET.readpos);
		NET.readpos += 4;
		return result;
	}

	public static float READ_FLOAT()
	{
		if (NET.readpos + 4 > NET.readlen)
		{
			NET.readerror = true;
			return 0f;
		}
		float result = NET.DecodeSingle(NET.readbuffer, NET.readpos);
		NET.readpos += 4;
		return result;
	}

	public static string READ_STRING()
	{
		int num = 0;
		int index = NET.readpos;
		while (NET.readpos < NET.readlen)
		{
			if (NET.readbuffer[NET.readpos] == 0)
			{
				break;
			}
			num++;
			NET.readpos++;
		}
		NET.readpos++;
		if (num == 0)
		{
			return string.Empty;
		}
		return Encoding.UTF8.GetString(NET.readbuffer, index, num);
	}

	public static bool READ_ERROR()
	{
		return NET.readerror;
	}

	public static int DecodeShort(byte[] inBytes, int pos)
	{
		return (int)BitConverter.ToUInt16(inBytes, pos);
	}

	public static int DecodeInteger(byte[] inBytes, int pos)
	{
		return BitConverter.ToInt32(inBytes, pos);
	}

	public static float DecodeSingle(byte[] inBytes, int pos)
	{
		return BitConverter.ToSingle(inBytes, pos);
	}
}
