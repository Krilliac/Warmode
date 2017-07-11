using Org.BouncyCastle.Asn1.X500;
using System;

namespace Org.BouncyCastle.Asn1.X509.SigI
{
	public class NameOrPseudonym : Asn1Encodable, IAsn1Choice
	{
		private readonly DirectoryString pseudonym;

		private readonly DirectoryString surname;

		private readonly Asn1Sequence givenName;

		public DirectoryString Pseudonym
		{
			get
			{
				return this.pseudonym;
			}
		}

		public DirectoryString Surname
		{
			get
			{
				return this.surname;
			}
		}

		public static NameOrPseudonym GetInstance(object obj)
		{
			if (obj == null || obj is NameOrPseudonym)
			{
				return (NameOrPseudonym)obj;
			}
			if (obj is IAsn1String)
			{
				return new NameOrPseudonym(DirectoryString.GetInstance(obj));
			}
			if (obj is Asn1Sequence)
			{
				return new NameOrPseudonym((Asn1Sequence)obj);
			}
			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		public NameOrPseudonym(DirectoryString pseudonym)
		{
			this.pseudonym = pseudonym;
		}

		private NameOrPseudonym(Asn1Sequence seq)
		{
			if (seq.Count != 2)
			{
				throw new ArgumentException("Bad sequence size: " + seq.Count);
			}
			if (!(seq[0] is IAsn1String))
			{
				throw new ArgumentException("Bad object encountered: " + seq[0].GetType().Name);
			}
			this.surname = DirectoryString.GetInstance(seq[0]);
			this.givenName = Asn1Sequence.GetInstance(seq[1]);
		}

		public NameOrPseudonym(string pseudonym) : this(new DirectoryString(pseudonym))
		{
		}

		public NameOrPseudonym(DirectoryString surname, Asn1Sequence givenName)
		{
			this.surname = surname;
			this.givenName = givenName;
		}

		public DirectoryString[] GetGivenName()
		{
			DirectoryString[] array = new DirectoryString[this.givenName.Count];
			int num = 0;
			foreach (object current in this.givenName)
			{
				array[num++] = DirectoryString.GetInstance(current);
			}
			return array;
		}

		public override Asn1Object ToAsn1Object()
		{
			if (this.pseudonym != null)
			{
				return this.pseudonym.ToAsn1Object();
			}
			return new DerSequence(new Asn1Encodable[]
			{
				this.surname,
				this.givenName
			});
		}
	}
}
