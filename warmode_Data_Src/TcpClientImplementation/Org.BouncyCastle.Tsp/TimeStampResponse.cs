using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cmp;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.Tsp;
using Org.BouncyCastle.Utilities;
using System;
using System.IO;
using System.Text;

namespace Org.BouncyCastle.Tsp
{
	public class TimeStampResponse
	{
		private TimeStampResp resp;

		private TimeStampToken timeStampToken;

		public int Status
		{
			get
			{
				return this.resp.Status.Status.IntValue;
			}
		}

		public TimeStampToken TimeStampToken
		{
			get
			{
				return this.timeStampToken;
			}
		}

		public TimeStampResponse(TimeStampResp resp)
		{
			this.resp = resp;
			if (resp.TimeStampToken != null)
			{
				this.timeStampToken = new TimeStampToken(resp.TimeStampToken);
			}
		}

		public TimeStampResponse(byte[] resp) : this(TimeStampResponse.readTimeStampResp(new Asn1InputStream(resp)))
		{
		}

		public TimeStampResponse(Stream input) : this(TimeStampResponse.readTimeStampResp(new Asn1InputStream(input)))
		{
		}

		private static TimeStampResp readTimeStampResp(Asn1InputStream input)
		{
			TimeStampResp instance;
			try
			{
				instance = TimeStampResp.GetInstance(input.ReadObject());
			}
			catch (ArgumentException ex)
			{
				throw new TspException("malformed timestamp response: " + ex, ex);
			}
			catch (InvalidCastException ex2)
			{
				throw new TspException("malformed timestamp response: " + ex2, ex2);
			}
			return instance;
		}

		public string GetStatusString()
		{
			if (this.resp.Status.StatusString == null)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			PkiFreeText statusString = this.resp.Status.StatusString;
			for (int num = 0; num != statusString.Count; num++)
			{
				stringBuilder.Append(statusString[num].GetString());
			}
			return stringBuilder.ToString();
		}

		public PkiFailureInfo GetFailInfo()
		{
			if (this.resp.Status.FailInfo == null)
			{
				return null;
			}
			return new PkiFailureInfo(this.resp.Status.FailInfo);
		}

		public void Validate(TimeStampRequest request)
		{
			TimeStampToken timeStampToken = this.TimeStampToken;
			if (timeStampToken != null)
			{
				TimeStampTokenInfo timeStampInfo = timeStampToken.TimeStampInfo;
				if (request.Nonce != null && !request.Nonce.Equals(timeStampInfo.Nonce))
				{
					throw new TspValidationException("response contains wrong nonce value.");
				}
				if (this.Status != 0 && this.Status != 1)
				{
					throw new TspValidationException("time stamp token found in failed request.");
				}
				if (!Arrays.ConstantTimeAreEqual(request.GetMessageImprintDigest(), timeStampInfo.GetMessageImprintDigest()))
				{
					throw new TspValidationException("response for different message imprint digest.");
				}
				if (!timeStampInfo.MessageImprintAlgOid.Equals(request.MessageImprintAlgOid))
				{
					throw new TspValidationException("response for different message imprint algorithm.");
				}
				Org.BouncyCastle.Asn1.Cms.Attribute attribute = timeStampToken.SignedAttributes[PkcsObjectIdentifiers.IdAASigningCertificate];
				Org.BouncyCastle.Asn1.Cms.Attribute attribute2 = timeStampToken.SignedAttributes[PkcsObjectIdentifiers.IdAASigningCertificateV2];
				if (attribute == null && attribute2 == null)
				{
					throw new TspValidationException("no signing certificate attribute present.");
				}
				if (attribute != null)
				{
				}
				if (request.ReqPolicy != null && !request.ReqPolicy.Equals(timeStampInfo.Policy))
				{
					throw new TspValidationException("TSA policy wrong for request.");
				}
			}
			else if (this.Status == 0 || this.Status == 1)
			{
				throw new TspValidationException("no time stamp token found and one expected.");
			}
		}

		public byte[] GetEncoded()
		{
			return this.resp.GetEncoded();
		}
	}
}
