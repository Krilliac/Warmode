using System;
using System.Text;

namespace Org.BouncyCastle.Asn1.X509
{
	public class RoleSyntax : Asn1Encodable
	{
		private readonly GeneralNames roleAuthority;

		private readonly GeneralName roleName;

		public GeneralNames RoleAuthority
		{
			get
			{
				return this.roleAuthority;
			}
		}

		public GeneralName RoleName
		{
			get
			{
				return this.roleName;
			}
		}

		public static RoleSyntax GetInstance(object obj)
		{
			if (obj is RoleSyntax)
			{
				return (RoleSyntax)obj;
			}
			if (obj != null)
			{
				return new RoleSyntax(Asn1Sequence.GetInstance(obj));
			}
			return null;
		}

		public RoleSyntax(GeneralNames roleAuthority, GeneralName roleName)
		{
			if (roleName == null || roleName.TagNo != 6 || ((IAsn1String)roleName.Name).GetString().Equals(""))
			{
				throw new ArgumentException("the role name MUST be non empty and MUST use the URI option of GeneralName");
			}
			this.roleAuthority = roleAuthority;
			this.roleName = roleName;
		}

		public RoleSyntax(GeneralName roleName) : this(null, roleName)
		{
		}

		public RoleSyntax(string roleName) : this(new GeneralName(6, (roleName == null) ? "" : roleName))
		{
		}

		private RoleSyntax(Asn1Sequence seq)
		{
			if (seq.Count < 1 || seq.Count > 2)
			{
				throw new ArgumentException("Bad sequence size: " + seq.Count);
			}
			for (int num = 0; num != seq.Count; num++)
			{
				Asn1TaggedObject instance = Asn1TaggedObject.GetInstance(seq[num]);
				switch (instance.TagNo)
				{
				case 0:
					this.roleAuthority = GeneralNames.GetInstance(instance, false);
					break;
				case 1:
					this.roleName = GeneralName.GetInstance(instance, true);
					break;
				default:
					throw new ArgumentException("Unknown tag in RoleSyntax");
				}
			}
		}

		public string GetRoleNameAsString()
		{
			return ((IAsn1String)this.roleName.Name).GetString();
		}

		public string[] GetRoleAuthorityAsString()
		{
			if (this.roleAuthority == null)
			{
				return new string[0];
			}
			GeneralName[] names = this.roleAuthority.GetNames();
			string[] array = new string[names.Length];
			for (int i = 0; i < names.Length; i++)
			{
				Asn1Encodable name = names[i].Name;
				if (name is IAsn1String)
				{
					array[i] = ((IAsn1String)name).GetString();
				}
				else
				{
					array[i] = name.ToString();
				}
			}
			return array;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			if (this.roleAuthority != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(false, 0, this.roleAuthority)
				});
			}
			asn1EncodableVector.Add(new Asn1Encodable[]
			{
				new DerTaggedObject(true, 1, this.roleName)
			});
			return new DerSequence(asn1EncodableVector);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder("Name: " + this.GetRoleNameAsString() + " - Auth: ");
			if (this.roleAuthority == null || this.roleAuthority.GetNames().Length == 0)
			{
				stringBuilder.Append("N/A");
			}
			else
			{
				string[] roleAuthorityAsString = this.GetRoleAuthorityAsString();
				stringBuilder.Append('[').Append(roleAuthorityAsString[0]);
				for (int i = 1; i < roleAuthorityAsString.Length; i++)
				{
					stringBuilder.Append(", ").Append(roleAuthorityAsString[i]);
				}
				stringBuilder.Append(']');
			}
			return stringBuilder.ToString();
		}
	}
}
