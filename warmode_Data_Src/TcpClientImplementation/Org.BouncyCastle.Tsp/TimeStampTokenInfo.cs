using Org.BouncyCastle.Asn1.Tsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Tsp
{
	public class TimeStampTokenInfo
	{
		private TstInfo tstInfo;

		private DateTime genTime;

		public bool IsOrdered
		{
			get
			{
				return this.tstInfo.Ordering.IsTrue;
			}
		}

		public Accuracy Accuracy
		{
			get
			{
				return this.tstInfo.Accuracy;
			}
		}

		public DateTime GenTime
		{
			get
			{
				return this.genTime;
			}
		}

		public GenTimeAccuracy GenTimeAccuracy
		{
			get
			{
				if (this.Accuracy != null)
				{
					return new GenTimeAccuracy(this.Accuracy);
				}
				return null;
			}
		}

		public string Policy
		{
			get
			{
				return this.tstInfo.Policy.Id;
			}
		}

		public BigInteger SerialNumber
		{
			get
			{
				return this.tstInfo.SerialNumber.Value;
			}
		}

		public GeneralName Tsa
		{
			get
			{
				return this.tstInfo.Tsa;
			}
		}

		public BigInteger Nonce
		{
			get
			{
				if (this.tstInfo.Nonce != null)
				{
					return this.tstInfo.Nonce.Value;
				}
				return null;
			}
		}

		public AlgorithmIdentifier HashAlgorithm
		{
			get
			{
				return this.tstInfo.MessageImprint.HashAlgorithm;
			}
		}

		public string MessageImprintAlgOid
		{
			get
			{
				return this.tstInfo.MessageImprint.HashAlgorithm.ObjectID.Id;
			}
		}

		public TstInfo TstInfo
		{
			get
			{
				return this.tstInfo;
			}
		}

		public TimeStampTokenInfo(TstInfo tstInfo)
		{
			this.tstInfo = tstInfo;
			try
			{
				this.genTime = tstInfo.GenTime.ToDateTime();
			}
			catch (Exception ex)
			{
				throw new TspException("unable to parse genTime field: " + ex.Message);
			}
		}

		public byte[] GetMessageImprintDigest()
		{
			return this.tstInfo.MessageImprint.GetHashedMessage();
		}

		public byte[] GetEncoded()
		{
			return this.tstInfo.GetEncoded();
		}
	}
}
