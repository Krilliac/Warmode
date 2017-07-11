using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.IO;

namespace Org.BouncyCastle.Ocsp
{
	public class OcspResp
	{
		private OcspResponse resp;

		public int Status
		{
			get
			{
				return this.resp.ResponseStatus.Value.IntValue;
			}
		}

		public OcspResp(OcspResponse resp)
		{
			this.resp = resp;
		}

		public OcspResp(byte[] resp) : this(new Asn1InputStream(resp))
		{
		}

		public OcspResp(Stream inStr) : this(new Asn1InputStream(inStr))
		{
		}

		private OcspResp(Asn1InputStream aIn)
		{
			try
			{
				this.resp = OcspResponse.GetInstance(aIn.ReadObject());
			}
			catch (Exception ex)
			{
				throw new IOException("malformed response: " + ex.Message, ex);
			}
		}

		public object GetResponseObject()
		{
			ResponseBytes responseBytes = this.resp.ResponseBytes;
			if (responseBytes == null)
			{
				return null;
			}
			if (responseBytes.ResponseType.Equals(OcspObjectIdentifiers.PkixOcspBasic))
			{
				try
				{
					return new BasicOcspResp(BasicOcspResponse.GetInstance(Asn1Object.FromByteArray(responseBytes.Response.GetOctets())));
				}
				catch (Exception ex)
				{
					throw new OcspException("problem decoding object: " + ex, ex);
				}
			}
			return responseBytes.Response;
		}

		public byte[] GetEncoded()
		{
			return this.resp.GetEncoded();
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			OcspResp ocspResp = obj as OcspResp;
			return ocspResp != null && this.resp.Equals(ocspResp.resp);
		}

		public override int GetHashCode()
		{
			return this.resp.GetHashCode();
		}
	}
}
