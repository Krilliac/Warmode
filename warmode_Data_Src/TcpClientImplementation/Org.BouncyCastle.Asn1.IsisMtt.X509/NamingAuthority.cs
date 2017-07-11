using Org.BouncyCastle.Asn1.X500;
using System;
using System.Collections;

namespace Org.BouncyCastle.Asn1.IsisMtt.X509
{
	public class NamingAuthority : Asn1Encodable
	{
		public static readonly DerObjectIdentifier IdIsisMttATNamingAuthoritiesRechtWirtschaftSteuern = new DerObjectIdentifier(IsisMttObjectIdentifiers.IdIsisMttATNamingAuthorities + ".1");

		private readonly DerObjectIdentifier namingAuthorityID;

		private readonly string namingAuthorityUrl;

		private readonly DirectoryString namingAuthorityText;

		public virtual DerObjectIdentifier NamingAuthorityID
		{
			get
			{
				return this.namingAuthorityID;
			}
		}

		public virtual DirectoryString NamingAuthorityText
		{
			get
			{
				return this.namingAuthorityText;
			}
		}

		public virtual string NamingAuthorityUrl
		{
			get
			{
				return this.namingAuthorityUrl;
			}
		}

		public static NamingAuthority GetInstance(object obj)
		{
			if (obj == null || obj is NamingAuthority)
			{
				return (NamingAuthority)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new NamingAuthority((Asn1Sequence)obj);
			}
			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		public static NamingAuthority GetInstance(Asn1TaggedObject obj, bool isExplicit)
		{
			return NamingAuthority.GetInstance(Asn1Sequence.GetInstance(obj, isExplicit));
		}

		private NamingAuthority(Asn1Sequence seq)
		{
			if (seq.Count > 3)
			{
				throw new ArgumentException("Bad sequence size: " + seq.Count);
			}
			IEnumerator enumerator = seq.GetEnumerator();
			if (enumerator.MoveNext())
			{
				Asn1Encodable asn1Encodable = (Asn1Encodable)enumerator.Current;
				if (asn1Encodable is DerObjectIdentifier)
				{
					this.namingAuthorityID = (DerObjectIdentifier)asn1Encodable;
				}
				else if (asn1Encodable is DerIA5String)
				{
					this.namingAuthorityUrl = DerIA5String.GetInstance(asn1Encodable).GetString();
				}
				else
				{
					if (!(asn1Encodable is IAsn1String))
					{
						throw new ArgumentException("Bad object encountered: " + asn1Encodable.GetType().Name);
					}
					this.namingAuthorityText = DirectoryString.GetInstance(asn1Encodable);
				}
			}
			if (enumerator.MoveNext())
			{
				Asn1Encodable asn1Encodable2 = (Asn1Encodable)enumerator.Current;
				if (asn1Encodable2 is DerIA5String)
				{
					this.namingAuthorityUrl = DerIA5String.GetInstance(asn1Encodable2).GetString();
				}
				else
				{
					if (!(asn1Encodable2 is IAsn1String))
					{
						throw new ArgumentException("Bad object encountered: " + asn1Encodable2.GetType().Name);
					}
					this.namingAuthorityText = DirectoryString.GetInstance(asn1Encodable2);
				}
			}
			if (!enumerator.MoveNext())
			{
				return;
			}
			Asn1Encodable asn1Encodable3 = (Asn1Encodable)enumerator.Current;
			if (asn1Encodable3 is IAsn1String)
			{
				this.namingAuthorityText = DirectoryString.GetInstance(asn1Encodable3);
				return;
			}
			throw new ArgumentException("Bad object encountered: " + asn1Encodable3.GetType().Name);
		}

		public NamingAuthority(DerObjectIdentifier namingAuthorityID, string namingAuthorityUrl, DirectoryString namingAuthorityText)
		{
			this.namingAuthorityID = namingAuthorityID;
			this.namingAuthorityUrl = namingAuthorityUrl;
			this.namingAuthorityText = namingAuthorityText;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			if (this.namingAuthorityID != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					this.namingAuthorityID
				});
			}
			if (this.namingAuthorityUrl != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerIA5String(this.namingAuthorityUrl, true)
				});
			}
			if (this.namingAuthorityText != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					this.namingAuthorityText
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
