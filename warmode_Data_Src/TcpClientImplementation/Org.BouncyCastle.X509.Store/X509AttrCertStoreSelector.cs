using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.Utilities.Date;
using Org.BouncyCastle.X509.Extension;
using System;
using System.Collections;

namespace Org.BouncyCastle.X509.Store
{
	public class X509AttrCertStoreSelector : IX509Selector, ICloneable
	{
		private IX509AttributeCertificate attributeCert;

		private DateTimeObject attributeCertificateValid;

		private AttributeCertificateHolder holder;

		private AttributeCertificateIssuer issuer;

		private BigInteger serialNumber;

		private ISet targetNames = new HashSet();

		private ISet targetGroups = new HashSet();

		public IX509AttributeCertificate AttributeCert
		{
			get
			{
				return this.attributeCert;
			}
			set
			{
				this.attributeCert = value;
			}
		}

		[Obsolete("Use AttributeCertificateValid instead")]
		public DateTimeObject AttribueCertificateValid
		{
			get
			{
				return this.attributeCertificateValid;
			}
			set
			{
				this.attributeCertificateValid = value;
			}
		}

		public DateTimeObject AttributeCertificateValid
		{
			get
			{
				return this.attributeCertificateValid;
			}
			set
			{
				this.attributeCertificateValid = value;
			}
		}

		public AttributeCertificateHolder Holder
		{
			get
			{
				return this.holder;
			}
			set
			{
				this.holder = value;
			}
		}

		public AttributeCertificateIssuer Issuer
		{
			get
			{
				return this.issuer;
			}
			set
			{
				this.issuer = value;
			}
		}

		public BigInteger SerialNumber
		{
			get
			{
				return this.serialNumber;
			}
			set
			{
				this.serialNumber = value;
			}
		}

		public X509AttrCertStoreSelector()
		{
		}

		private X509AttrCertStoreSelector(X509AttrCertStoreSelector o)
		{
			this.attributeCert = o.attributeCert;
			this.attributeCertificateValid = o.attributeCertificateValid;
			this.holder = o.holder;
			this.issuer = o.issuer;
			this.serialNumber = o.serialNumber;
			this.targetGroups = new HashSet(o.targetGroups);
			this.targetNames = new HashSet(o.targetNames);
		}

		public bool Match(object obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			IX509AttributeCertificate iX509AttributeCertificate = obj as IX509AttributeCertificate;
			if (iX509AttributeCertificate == null)
			{
				return false;
			}
			if (this.attributeCert != null && !this.attributeCert.Equals(iX509AttributeCertificate))
			{
				return false;
			}
			if (this.serialNumber != null && !iX509AttributeCertificate.SerialNumber.Equals(this.serialNumber))
			{
				return false;
			}
			if (this.holder != null && !iX509AttributeCertificate.Holder.Equals(this.holder))
			{
				return false;
			}
			if (this.issuer != null && !iX509AttributeCertificate.Issuer.Equals(this.issuer))
			{
				return false;
			}
			if (this.attributeCertificateValid != null && !iX509AttributeCertificate.IsValid(this.attributeCertificateValid.Value))
			{
				return false;
			}
			if (this.targetNames.Count > 0 || this.targetGroups.Count > 0)
			{
				Asn1OctetString extensionValue = iX509AttributeCertificate.GetExtensionValue(X509Extensions.TargetInformation);
				if (extensionValue != null)
				{
					TargetInformation instance;
					try
					{
						instance = TargetInformation.GetInstance(X509ExtensionUtilities.FromExtensionValue(extensionValue));
					}
					catch (Exception)
					{
						bool result = false;
						return result;
					}
					Targets[] targetsObjects = instance.GetTargetsObjects();
					if (this.targetNames.Count > 0)
					{
						bool flag = false;
						int num = 0;
						while (num < targetsObjects.Length && !flag)
						{
							Target[] targets = targetsObjects[num].GetTargets();
							for (int i = 0; i < targets.Length; i++)
							{
								GeneralName targetName = targets[i].TargetName;
								if (targetName != null && this.targetNames.Contains(targetName))
								{
									flag = true;
									break;
								}
							}
							num++;
						}
						if (!flag)
						{
							return false;
						}
					}
					if (this.targetGroups.Count <= 0)
					{
						return true;
					}
					bool flag2 = false;
					int num2 = 0;
					while (num2 < targetsObjects.Length && !flag2)
					{
						Target[] targets2 = targetsObjects[num2].GetTargets();
						for (int j = 0; j < targets2.Length; j++)
						{
							GeneralName targetGroup = targets2[j].TargetGroup;
							if (targetGroup != null && this.targetGroups.Contains(targetGroup))
							{
								flag2 = true;
								break;
							}
						}
						num2++;
					}
					if (!flag2)
					{
						return false;
					}
					return true;
				}
			}
			return true;
		}

		public object Clone()
		{
			return new X509AttrCertStoreSelector(this);
		}

		public void AddTargetName(GeneralName name)
		{
			this.targetNames.Add(name);
		}

		public void AddTargetName(byte[] name)
		{
			this.AddTargetName(GeneralName.GetInstance(Asn1Object.FromByteArray(name)));
		}

		public void SetTargetNames(IEnumerable names)
		{
			this.targetNames = this.ExtractGeneralNames(names);
		}

		public IEnumerable GetTargetNames()
		{
			return new EnumerableProxy(this.targetNames);
		}

		public void AddTargetGroup(GeneralName group)
		{
			this.targetGroups.Add(group);
		}

		public void AddTargetGroup(byte[] name)
		{
			this.AddTargetGroup(GeneralName.GetInstance(Asn1Object.FromByteArray(name)));
		}

		public void SetTargetGroups(IEnumerable names)
		{
			this.targetGroups = this.ExtractGeneralNames(names);
		}

		public IEnumerable GetTargetGroups()
		{
			return new EnumerableProxy(this.targetGroups);
		}

		private ISet ExtractGeneralNames(IEnumerable names)
		{
			ISet set = new HashSet();
			if (names != null)
			{
				foreach (object current in names)
				{
					if (current is GeneralName)
					{
						set.Add(current);
					}
					else
					{
						set.Add(GeneralName.GetInstance(Asn1Object.FromByteArray((byte[])current)));
					}
				}
			}
			return set;
		}
	}
}
