using System;

namespace Org.BouncyCastle.Asn1.Cms
{
	public class ScvpReqRes : Asn1Encodable
	{
		private readonly ContentInfo request;

		private readonly ContentInfo response;

		public virtual ContentInfo Request
		{
			get
			{
				return this.request;
			}
		}

		public virtual ContentInfo Response
		{
			get
			{
				return this.response;
			}
		}

		public static ScvpReqRes GetInstance(object obj)
		{
			if (obj is ScvpReqRes)
			{
				return (ScvpReqRes)obj;
			}
			if (obj != null)
			{
				return new ScvpReqRes(Asn1Sequence.GetInstance(obj));
			}
			return null;
		}

		private ScvpReqRes(Asn1Sequence seq)
		{
			if (seq[0] is Asn1TaggedObject)
			{
				this.request = ContentInfo.GetInstance(Asn1TaggedObject.GetInstance(seq[0]), true);
				this.response = ContentInfo.GetInstance(seq[1]);
				return;
			}
			this.request = null;
			this.response = ContentInfo.GetInstance(seq[0]);
		}

		public ScvpReqRes(ContentInfo response) : this(null, response)
		{
		}

		public ScvpReqRes(ContentInfo request, ContentInfo response)
		{
			this.request = request;
			this.response = response;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			if (this.request != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 0, this.request)
				});
			}
			asn1EncodableVector.Add(new Asn1Encodable[]
			{
				this.response
			});
			return new DerSequence(asn1EncodableVector);
		}
	}
}
