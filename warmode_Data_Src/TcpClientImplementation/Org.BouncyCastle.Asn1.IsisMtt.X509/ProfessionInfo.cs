using Org.BouncyCastle.Asn1.X500;
using System;
using System.Collections;

namespace Org.BouncyCastle.Asn1.IsisMtt.X509
{
	public class ProfessionInfo : Asn1Encodable
	{
		public static readonly DerObjectIdentifier Rechtsanwltin = new DerObjectIdentifier(NamingAuthority.IdIsisMttATNamingAuthoritiesRechtWirtschaftSteuern + ".1");

		public static readonly DerObjectIdentifier Rechtsanwalt = new DerObjectIdentifier(NamingAuthority.IdIsisMttATNamingAuthoritiesRechtWirtschaftSteuern + ".2");

		public static readonly DerObjectIdentifier Rechtsbeistand = new DerObjectIdentifier(NamingAuthority.IdIsisMttATNamingAuthoritiesRechtWirtschaftSteuern + ".3");

		public static readonly DerObjectIdentifier Steuerberaterin = new DerObjectIdentifier(NamingAuthority.IdIsisMttATNamingAuthoritiesRechtWirtschaftSteuern + ".4");

		public static readonly DerObjectIdentifier Steuerberater = new DerObjectIdentifier(NamingAuthority.IdIsisMttATNamingAuthoritiesRechtWirtschaftSteuern + ".5");

		public static readonly DerObjectIdentifier Steuerbevollmchtigte = new DerObjectIdentifier(NamingAuthority.IdIsisMttATNamingAuthoritiesRechtWirtschaftSteuern + ".6");

		public static readonly DerObjectIdentifier Steuerbevollmchtigter = new DerObjectIdentifier(NamingAuthority.IdIsisMttATNamingAuthoritiesRechtWirtschaftSteuern + ".7");

		public static readonly DerObjectIdentifier Notarin = new DerObjectIdentifier(NamingAuthority.IdIsisMttATNamingAuthoritiesRechtWirtschaftSteuern + ".8");

		public static readonly DerObjectIdentifier Notar = new DerObjectIdentifier(NamingAuthority.IdIsisMttATNamingAuthoritiesRechtWirtschaftSteuern + ".9");

		public static readonly DerObjectIdentifier Notarvertreterin = new DerObjectIdentifier(NamingAuthority.IdIsisMttATNamingAuthoritiesRechtWirtschaftSteuern + ".10");

		public static readonly DerObjectIdentifier Notarvertreter = new DerObjectIdentifier(NamingAuthority.IdIsisMttATNamingAuthoritiesRechtWirtschaftSteuern + ".11");

		public static readonly DerObjectIdentifier Notariatsverwalterin = new DerObjectIdentifier(NamingAuthority.IdIsisMttATNamingAuthoritiesRechtWirtschaftSteuern + ".12");

		public static readonly DerObjectIdentifier Notariatsverwalter = new DerObjectIdentifier(NamingAuthority.IdIsisMttATNamingAuthoritiesRechtWirtschaftSteuern + ".13");

		public static readonly DerObjectIdentifier Wirtschaftsprferin = new DerObjectIdentifier(NamingAuthority.IdIsisMttATNamingAuthoritiesRechtWirtschaftSteuern + ".14");

		public static readonly DerObjectIdentifier Wirtschaftsprfer = new DerObjectIdentifier(NamingAuthority.IdIsisMttATNamingAuthoritiesRechtWirtschaftSteuern + ".15");

		public static readonly DerObjectIdentifier VereidigteBuchprferin = new DerObjectIdentifier(NamingAuthority.IdIsisMttATNamingAuthoritiesRechtWirtschaftSteuern + ".16");

		public static readonly DerObjectIdentifier VereidigterBuchprfer = new DerObjectIdentifier(NamingAuthority.IdIsisMttATNamingAuthoritiesRechtWirtschaftSteuern + ".17");

		public static readonly DerObjectIdentifier Patentanwltin = new DerObjectIdentifier(NamingAuthority.IdIsisMttATNamingAuthoritiesRechtWirtschaftSteuern + ".18");

		public static readonly DerObjectIdentifier Patentanwalt = new DerObjectIdentifier(NamingAuthority.IdIsisMttATNamingAuthoritiesRechtWirtschaftSteuern + ".19");

		private readonly NamingAuthority namingAuthority;

		private readonly Asn1Sequence professionItems;

		private readonly Asn1Sequence professionOids;

		private readonly string registrationNumber;

		private readonly Asn1OctetString addProfessionInfo;

		public virtual Asn1OctetString AddProfessionInfo
		{
			get
			{
				return this.addProfessionInfo;
			}
		}

		public virtual NamingAuthority NamingAuthority
		{
			get
			{
				return this.namingAuthority;
			}
		}

		public virtual string RegistrationNumber
		{
			get
			{
				return this.registrationNumber;
			}
		}

		public static ProfessionInfo GetInstance(object obj)
		{
			if (obj == null || obj is ProfessionInfo)
			{
				return (ProfessionInfo)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new ProfessionInfo((Asn1Sequence)obj);
			}
			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		private ProfessionInfo(Asn1Sequence seq)
		{
			if (seq.Count > 5)
			{
				throw new ArgumentException("Bad sequence size: " + seq.Count);
			}
			IEnumerator enumerator = seq.GetEnumerator();
			enumerator.MoveNext();
			Asn1Encodable asn1Encodable = (Asn1Encodable)enumerator.Current;
			if (asn1Encodable is Asn1TaggedObject)
			{
				Asn1TaggedObject asn1TaggedObject = (Asn1TaggedObject)asn1Encodable;
				if (asn1TaggedObject.TagNo != 0)
				{
					throw new ArgumentException("Bad tag number: " + asn1TaggedObject.TagNo);
				}
				this.namingAuthority = NamingAuthority.GetInstance(asn1TaggedObject, true);
				enumerator.MoveNext();
				asn1Encodable = (Asn1Encodable)enumerator.Current;
			}
			this.professionItems = Asn1Sequence.GetInstance(asn1Encodable);
			if (enumerator.MoveNext())
			{
				asn1Encodable = (Asn1Encodable)enumerator.Current;
				if (asn1Encodable is Asn1Sequence)
				{
					this.professionOids = Asn1Sequence.GetInstance(asn1Encodable);
				}
				else if (asn1Encodable is DerPrintableString)
				{
					this.registrationNumber = DerPrintableString.GetInstance(asn1Encodable).GetString();
				}
				else
				{
					if (!(asn1Encodable is Asn1OctetString))
					{
						throw new ArgumentException("Bad object encountered: " + asn1Encodable.GetType().Name);
					}
					this.addProfessionInfo = Asn1OctetString.GetInstance(asn1Encodable);
				}
			}
			if (enumerator.MoveNext())
			{
				asn1Encodable = (Asn1Encodable)enumerator.Current;
				if (asn1Encodable is DerPrintableString)
				{
					this.registrationNumber = DerPrintableString.GetInstance(asn1Encodable).GetString();
				}
				else
				{
					if (!(asn1Encodable is DerOctetString))
					{
						throw new ArgumentException("Bad object encountered: " + asn1Encodable.GetType().Name);
					}
					this.addProfessionInfo = (DerOctetString)asn1Encodable;
				}
			}
			if (!enumerator.MoveNext())
			{
				return;
			}
			asn1Encodable = (Asn1Encodable)enumerator.Current;
			if (asn1Encodable is DerOctetString)
			{
				this.addProfessionInfo = (DerOctetString)asn1Encodable;
				return;
			}
			throw new ArgumentException("Bad object encountered: " + asn1Encodable.GetType().Name);
		}

		public ProfessionInfo(NamingAuthority namingAuthority, DirectoryString[] professionItems, DerObjectIdentifier[] professionOids, string registrationNumber, Asn1OctetString addProfessionInfo)
		{
			this.namingAuthority = namingAuthority;
			this.professionItems = new DerSequence(professionItems);
			if (professionOids != null)
			{
				this.professionOids = new DerSequence(professionOids);
			}
			this.registrationNumber = registrationNumber;
			this.addProfessionInfo = addProfessionInfo;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			if (this.namingAuthority != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 0, this.namingAuthority)
				});
			}
			asn1EncodableVector.Add(new Asn1Encodable[]
			{
				this.professionItems
			});
			if (this.professionOids != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					this.professionOids
				});
			}
			if (this.registrationNumber != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerPrintableString(this.registrationNumber, true)
				});
			}
			if (this.addProfessionInfo != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					this.addProfessionInfo
				});
			}
			return new DerSequence(asn1EncodableVector);
		}

		public virtual DirectoryString[] GetProfessionItems()
		{
			DirectoryString[] array = new DirectoryString[this.professionItems.Count];
			for (int i = 0; i < this.professionItems.Count; i++)
			{
				array[i] = DirectoryString.GetInstance(this.professionItems[i]);
			}
			return array;
		}

		public virtual DerObjectIdentifier[] GetProfessionOids()
		{
			if (this.professionOids == null)
			{
				return new DerObjectIdentifier[0];
			}
			DerObjectIdentifier[] array = new DerObjectIdentifier[this.professionOids.Count];
			for (int i = 0; i < this.professionOids.Count; i++)
			{
				array[i] = DerObjectIdentifier.GetInstance(this.professionOids[i]);
			}
			return array;
		}
	}
}
