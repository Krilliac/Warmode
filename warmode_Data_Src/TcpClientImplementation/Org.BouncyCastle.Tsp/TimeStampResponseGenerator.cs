using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cmp;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.Tsp;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities.Date;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Tsp
{
	public class TimeStampResponseGenerator
	{
		private class FailInfo : DerBitString
		{
			internal FailInfo(int failInfoValue) : base(DerBitString.GetBytes(failInfoValue), DerBitString.GetPadBits(failInfoValue))
			{
			}
		}

		private PkiStatus status;

		private Asn1EncodableVector statusStrings;

		private int failInfo;

		private TimeStampTokenGenerator tokenGenerator;

		private IList acceptedAlgorithms;

		private IList acceptedPolicies;

		private IList acceptedExtensions;

		public TimeStampResponseGenerator(TimeStampTokenGenerator tokenGenerator, IList acceptedAlgorithms) : this(tokenGenerator, acceptedAlgorithms, null, null)
		{
		}

		public TimeStampResponseGenerator(TimeStampTokenGenerator tokenGenerator, IList acceptedAlgorithms, IList acceptedPolicy) : this(tokenGenerator, acceptedAlgorithms, acceptedPolicy, null)
		{
		}

		public TimeStampResponseGenerator(TimeStampTokenGenerator tokenGenerator, IList acceptedAlgorithms, IList acceptedPolicies, IList acceptedExtensions)
		{
			this.tokenGenerator = tokenGenerator;
			this.acceptedAlgorithms = acceptedAlgorithms;
			this.acceptedPolicies = acceptedPolicies;
			this.acceptedExtensions = acceptedExtensions;
			this.statusStrings = new Asn1EncodableVector(new Asn1Encodable[0]);
		}

		private void AddStatusString(string statusString)
		{
			this.statusStrings.Add(new Asn1Encodable[]
			{
				new DerUtf8String(statusString)
			});
		}

		private void SetFailInfoField(int field)
		{
			this.failInfo |= field;
		}

		private PkiStatusInfo GetPkiStatusInfo()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				new DerInteger((int)this.status)
			});
			if (this.statusStrings.Count > 0)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new PkiFreeText(new DerSequence(this.statusStrings))
				});
			}
			if (this.failInfo != 0)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new TimeStampResponseGenerator.FailInfo(this.failInfo)
				});
			}
			return new PkiStatusInfo(new DerSequence(asn1EncodableVector));
		}

		public TimeStampResponse Generate(TimeStampRequest request, BigInteger serialNumber, DateTime genTime)
		{
			return this.Generate(request, serialNumber, new DateTimeObject(genTime));
		}

		public TimeStampResponse Generate(TimeStampRequest request, BigInteger serialNumber, DateTimeObject genTime)
		{
			TimeStampResp resp;
			try
			{
				if (genTime == null)
				{
					throw new TspValidationException("The time source is not available.", 512);
				}
				request.Validate(this.acceptedAlgorithms, this.acceptedPolicies, this.acceptedExtensions);
				this.status = PkiStatus.Granted;
				this.AddStatusString("Operation Okay");
				PkiStatusInfo pkiStatusInfo = this.GetPkiStatusInfo();
				ContentInfo instance;
				try
				{
					TimeStampToken timeStampToken = this.tokenGenerator.Generate(request, serialNumber, genTime.Value);
					byte[] encoded = timeStampToken.ToCmsSignedData().GetEncoded();
					instance = ContentInfo.GetInstance(Asn1Object.FromByteArray(encoded));
				}
				catch (IOException e)
				{
					throw new TspException("Timestamp token received cannot be converted to ContentInfo", e);
				}
				resp = new TimeStampResp(pkiStatusInfo, instance);
			}
			catch (TspValidationException ex)
			{
				this.status = PkiStatus.Rejection;
				this.SetFailInfoField(ex.FailureCode);
				this.AddStatusString(ex.Message);
				PkiStatusInfo pkiStatusInfo2 = this.GetPkiStatusInfo();
				resp = new TimeStampResp(pkiStatusInfo2, null);
			}
			TimeStampResponse result;
			try
			{
				result = new TimeStampResponse(resp);
			}
			catch (IOException e2)
			{
				throw new TspException("created badly formatted response!", e2);
			}
			return result;
		}

		public TimeStampResponse GenerateFailResponse(PkiStatus status, int failInfoField, string statusString)
		{
			this.status = status;
			this.SetFailInfoField(failInfoField);
			if (statusString != null)
			{
				this.AddStatusString(statusString);
			}
			PkiStatusInfo pkiStatusInfo = this.GetPkiStatusInfo();
			TimeStampResp resp = new TimeStampResp(pkiStatusInfo, null);
			TimeStampResponse result;
			try
			{
				result = new TimeStampResponse(resp);
			}
			catch (IOException e)
			{
				throw new TspException("created badly formatted response!", e);
			}
			return result;
		}
	}
}
