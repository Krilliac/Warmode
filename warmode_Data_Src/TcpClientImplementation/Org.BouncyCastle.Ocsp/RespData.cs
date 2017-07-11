using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.X509;
using System;

namespace Org.BouncyCastle.Ocsp
{
	public class RespData : X509ExtensionBase
	{
		internal readonly ResponseData data;

		public int Version
		{
			get
			{
				return this.data.Version.Value.IntValue + 1;
			}
		}

		public DateTime ProducedAt
		{
			get
			{
				return this.data.ProducedAt.ToDateTime();
			}
		}

		public X509Extensions ResponseExtensions
		{
			get
			{
				return this.data.ResponseExtensions;
			}
		}

		public RespData(ResponseData data)
		{
			this.data = data;
		}

		public RespID GetResponderId()
		{
			return new RespID(this.data.ResponderID);
		}

		public SingleResp[] GetResponses()
		{
			Asn1Sequence responses = this.data.Responses;
			SingleResp[] array = new SingleResp[responses.Count];
			for (int num = 0; num != array.Length; num++)
			{
				array[num] = new SingleResp(SingleResponse.GetInstance(responses[num]));
			}
			return array;
		}

		protected override X509Extensions GetX509Extensions()
		{
			return this.ResponseExtensions;
		}
	}
}
