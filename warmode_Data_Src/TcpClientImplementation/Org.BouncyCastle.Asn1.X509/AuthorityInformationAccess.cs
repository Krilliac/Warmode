using Org.BouncyCastle.Utilities;
using System;
using System.Text;

namespace Org.BouncyCastle.Asn1.X509
{
	public class AuthorityInformationAccess : Asn1Encodable
	{
		private readonly AccessDescription[] descriptions;

		public static AuthorityInformationAccess GetInstance(object obj)
		{
			if (obj is AuthorityInformationAccess)
			{
				return (AuthorityInformationAccess)obj;
			}
			if (obj == null)
			{
				return null;
			}
			return new AuthorityInformationAccess(Asn1Sequence.GetInstance(obj));
		}

		private AuthorityInformationAccess(Asn1Sequence seq)
		{
			if (seq.Count < 1)
			{
				throw new ArgumentException("sequence may not be empty");
			}
			this.descriptions = new AccessDescription[seq.Count];
			for (int i = 0; i < seq.Count; i++)
			{
				this.descriptions[i] = AccessDescription.GetInstance(seq[i]);
			}
		}

		public AuthorityInformationAccess(AccessDescription description)
		{
			this.descriptions = new AccessDescription[]
			{
				description
			};
		}

		public AuthorityInformationAccess(DerObjectIdentifier oid, GeneralName location) : this(new AccessDescription(oid, location))
		{
		}

		public AccessDescription[] GetAccessDescriptions()
		{
			return (AccessDescription[])this.descriptions.Clone();
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerSequence(this.descriptions);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			string newLine = Platform.NewLine;
			stringBuilder.Append("AuthorityInformationAccess:");
			stringBuilder.Append(newLine);
			AccessDescription[] array = this.descriptions;
			for (int i = 0; i < array.Length; i++)
			{
				AccessDescription value = array[i];
				stringBuilder.Append("    ");
				stringBuilder.Append(value);
				stringBuilder.Append(newLine);
			}
			return stringBuilder.ToString();
		}
	}
}
