using Org.BouncyCastle.Utilities;
using System;
using System.Text;

namespace Org.BouncyCastle.Asn1.X509
{
	public class GeneralNames : Asn1Encodable
	{
		private readonly GeneralName[] names;

		public static GeneralNames GetInstance(object obj)
		{
			if (obj == null || obj is GeneralNames)
			{
				return (GeneralNames)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new GeneralNames((Asn1Sequence)obj);
			}
			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		public static GeneralNames GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return GeneralNames.GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public GeneralNames(GeneralName name)
		{
			this.names = new GeneralName[]
			{
				name
			};
		}

		public GeneralNames(GeneralName[] names)
		{
			this.names = (GeneralName[])names.Clone();
		}

		private GeneralNames(Asn1Sequence seq)
		{
			this.names = new GeneralName[seq.Count];
			for (int num = 0; num != seq.Count; num++)
			{
				this.names[num] = GeneralName.GetInstance(seq[num]);
			}
		}

		public GeneralName[] GetNames()
		{
			return (GeneralName[])this.names.Clone();
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerSequence(this.names);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			string newLine = Platform.NewLine;
			stringBuilder.Append("GeneralNames:");
			stringBuilder.Append(newLine);
			GeneralName[] array = this.names;
			for (int i = 0; i < array.Length; i++)
			{
				GeneralName value = array[i];
				stringBuilder.Append("    ");
				stringBuilder.Append(value);
				stringBuilder.Append(newLine);
			}
			return stringBuilder.ToString();
		}
	}
}
