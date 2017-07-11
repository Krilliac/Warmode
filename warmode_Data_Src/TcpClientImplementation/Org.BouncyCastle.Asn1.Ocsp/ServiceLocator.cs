using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Ocsp
{
	public class ServiceLocator : Asn1Encodable
	{
		private readonly X509Name issuer;

		private readonly Asn1Object locator;

		public X509Name Issuer
		{
			get
			{
				return this.issuer;
			}
		}

		public Asn1Object Locator
		{
			get
			{
				return this.locator;
			}
		}

		public static ServiceLocator GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return ServiceLocator.GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static ServiceLocator GetInstance(object obj)
		{
			if (obj == null || obj is ServiceLocator)
			{
				return (ServiceLocator)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new ServiceLocator((Asn1Sequence)obj);
			}
			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		public ServiceLocator(X509Name issuer) : this(issuer, null)
		{
		}

		public ServiceLocator(X509Name issuer, Asn1Object locator)
		{
			if (issuer == null)
			{
				throw new ArgumentNullException("issuer");
			}
			this.issuer = issuer;
			this.locator = locator;
		}

		private ServiceLocator(Asn1Sequence seq)
		{
			this.issuer = X509Name.GetInstance(seq[0]);
			if (seq.Count > 1)
			{
				this.locator = seq[1].ToAsn1Object();
			}
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.issuer
			});
			if (this.locator != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					this.locator
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
