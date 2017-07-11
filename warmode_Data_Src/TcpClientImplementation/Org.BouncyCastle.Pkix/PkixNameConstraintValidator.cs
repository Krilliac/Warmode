using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections;

namespace Org.BouncyCastle.Pkix
{
	public class PkixNameConstraintValidator
	{
		private ISet excludedSubtreesDN = new HashSet();

		private ISet excludedSubtreesDNS = new HashSet();

		private ISet excludedSubtreesEmail = new HashSet();

		private ISet excludedSubtreesURI = new HashSet();

		private ISet excludedSubtreesIP = new HashSet();

		private ISet permittedSubtreesDN;

		private ISet permittedSubtreesDNS;

		private ISet permittedSubtreesEmail;

		private ISet permittedSubtreesURI;

		private ISet permittedSubtreesIP;

		private static bool WithinDNSubtree(Asn1Sequence dns, Asn1Sequence subtree)
		{
			if (subtree.Count < 1)
			{
				return false;
			}
			if (subtree.Count > dns.Count)
			{
				return false;
			}
			for (int i = subtree.Count - 1; i >= 0; i--)
			{
				if (!subtree[i].Equals(dns[i]))
				{
					return false;
				}
			}
			return true;
		}

		public void CheckPermittedDN(Asn1Sequence dns)
		{
			this.CheckPermittedDN(this.permittedSubtreesDN, dns);
		}

		public void CheckExcludedDN(Asn1Sequence dns)
		{
			this.CheckExcludedDN(this.excludedSubtreesDN, dns);
		}

		private void CheckPermittedDN(ISet permitted, Asn1Sequence dns)
		{
			if (permitted == null)
			{
				return;
			}
			if (permitted.Count == 0 && dns.Count == 0)
			{
				return;
			}
			IEnumerator enumerator = permitted.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Asn1Sequence subtree = (Asn1Sequence)enumerator.Current;
				if (PkixNameConstraintValidator.WithinDNSubtree(dns, subtree))
				{
					return;
				}
			}
			throw new PkixNameConstraintValidatorException("Subject distinguished name is not from a permitted subtree");
		}

		private void CheckExcludedDN(ISet excluded, Asn1Sequence dns)
		{
			if (excluded.IsEmpty)
			{
				return;
			}
			IEnumerator enumerator = excluded.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Asn1Sequence subtree = (Asn1Sequence)enumerator.Current;
				if (PkixNameConstraintValidator.WithinDNSubtree(dns, subtree))
				{
					throw new PkixNameConstraintValidatorException("Subject distinguished name is from an excluded subtree");
				}
			}
		}

		private ISet IntersectDN(ISet permitted, ISet dns)
		{
			ISet set = new HashSet();
			IEnumerator enumerator = dns.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Asn1Sequence instance = Asn1Sequence.GetInstance(((GeneralSubtree)enumerator.Current).Base.Name.ToAsn1Object());
				if (permitted == null)
				{
					if (instance != null)
					{
						set.Add(instance);
					}
				}
				else
				{
					IEnumerator enumerator2 = permitted.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						Asn1Sequence asn1Sequence = (Asn1Sequence)enumerator2.Current;
						if (PkixNameConstraintValidator.WithinDNSubtree(instance, asn1Sequence))
						{
							set.Add(instance);
						}
						else if (PkixNameConstraintValidator.WithinDNSubtree(asn1Sequence, instance))
						{
							set.Add(asn1Sequence);
						}
					}
				}
			}
			return set;
		}

		private ISet UnionDN(ISet excluded, Asn1Sequence dn)
		{
			if (!excluded.IsEmpty)
			{
				ISet set = new HashSet();
				IEnumerator enumerator = excluded.GetEnumerator();
				while (enumerator.MoveNext())
				{
					Asn1Sequence asn1Sequence = (Asn1Sequence)enumerator.Current;
					if (PkixNameConstraintValidator.WithinDNSubtree(dn, asn1Sequence))
					{
						set.Add(asn1Sequence);
					}
					else if (PkixNameConstraintValidator.WithinDNSubtree(asn1Sequence, dn))
					{
						set.Add(dn);
					}
					else
					{
						set.Add(asn1Sequence);
						set.Add(dn);
					}
				}
				return set;
			}
			if (dn == null)
			{
				return excluded;
			}
			excluded.Add(dn);
			return excluded;
		}

		private ISet IntersectEmail(ISet permitted, ISet emails)
		{
			ISet set = new HashSet();
			IEnumerator enumerator = emails.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string text = this.ExtractNameAsString(((GeneralSubtree)enumerator.Current).Base);
				if (permitted == null)
				{
					if (text != null)
					{
						set.Add(text);
					}
				}
				else
				{
					IEnumerator enumerator2 = permitted.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						string email = (string)enumerator2.Current;
						this.intersectEmail(text, email, set);
					}
				}
			}
			return set;
		}

		private ISet UnionEmail(ISet excluded, string email)
		{
			if (!excluded.IsEmpty)
			{
				ISet set = new HashSet();
				IEnumerator enumerator = excluded.GetEnumerator();
				while (enumerator.MoveNext())
				{
					string email2 = (string)enumerator.Current;
					this.unionEmail(email2, email, set);
				}
				return set;
			}
			if (email == null)
			{
				return excluded;
			}
			excluded.Add(email);
			return excluded;
		}

		private ISet IntersectIP(ISet permitted, ISet ips)
		{
			ISet set = new HashSet();
			IEnumerator enumerator = ips.GetEnumerator();
			while (enumerator.MoveNext())
			{
				byte[] octets = Asn1OctetString.GetInstance(((GeneralSubtree)enumerator.Current).Base.Name).GetOctets();
				if (permitted == null)
				{
					if (octets != null)
					{
						set.Add(octets);
					}
				}
				else
				{
					IEnumerator enumerator2 = permitted.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						byte[] ipWithSubmask = (byte[])enumerator2.Current;
						set.AddAll(this.IntersectIPRange(ipWithSubmask, octets));
					}
				}
			}
			return set;
		}

		private ISet UnionIP(ISet excluded, byte[] ip)
		{
			if (!excluded.IsEmpty)
			{
				ISet set = new HashSet();
				IEnumerator enumerator = excluded.GetEnumerator();
				while (enumerator.MoveNext())
				{
					byte[] ipWithSubmask = (byte[])enumerator.Current;
					set.AddAll(this.UnionIPRange(ipWithSubmask, ip));
				}
				return set;
			}
			if (ip == null)
			{
				return excluded;
			}
			excluded.Add(ip);
			return excluded;
		}

		private ISet UnionIPRange(byte[] ipWithSubmask1, byte[] ipWithSubmask2)
		{
			ISet set = new HashSet();
			if (Arrays.AreEqual(ipWithSubmask1, ipWithSubmask2))
			{
				set.Add(ipWithSubmask1);
			}
			else
			{
				set.Add(ipWithSubmask1);
				set.Add(ipWithSubmask2);
			}
			return set;
		}

		private ISet IntersectIPRange(byte[] ipWithSubmask1, byte[] ipWithSubmask2)
		{
			if (ipWithSubmask1.Length != ipWithSubmask2.Length)
			{
				return new HashSet();
			}
			byte[][] array = this.ExtractIPsAndSubnetMasks(ipWithSubmask1, ipWithSubmask2);
			byte[] ip = array[0];
			byte[] array2 = array[1];
			byte[] ip2 = array[2];
			byte[] array3 = array[3];
			byte[][] array4 = this.MinMaxIPs(ip, array2, ip2, array3);
			byte[] ip3 = PkixNameConstraintValidator.Min(array4[1], array4[3]);
			byte[] ip4 = PkixNameConstraintValidator.Max(array4[0], array4[2]);
			if (PkixNameConstraintValidator.CompareTo(ip4, ip3) == 1)
			{
				return new HashSet();
			}
			byte[] ip5 = PkixNameConstraintValidator.Or(array4[0], array4[2]);
			byte[] subnetMask = PkixNameConstraintValidator.Or(array2, array3);
			return new HashSet
			{
				this.IpWithSubnetMask(ip5, subnetMask)
			};
		}

		private byte[] IpWithSubnetMask(byte[] ip, byte[] subnetMask)
		{
			int num = ip.Length;
			byte[] array = new byte[num * 2];
			Array.Copy(ip, 0, array, 0, num);
			Array.Copy(subnetMask, 0, array, num, num);
			return array;
		}

		private byte[][] ExtractIPsAndSubnetMasks(byte[] ipWithSubmask1, byte[] ipWithSubmask2)
		{
			int num = ipWithSubmask1.Length / 2;
			byte[] array = new byte[num];
			byte[] array2 = new byte[num];
			Array.Copy(ipWithSubmask1, 0, array, 0, num);
			Array.Copy(ipWithSubmask1, num, array2, 0, num);
			byte[] array3 = new byte[num];
			byte[] array4 = new byte[num];
			Array.Copy(ipWithSubmask2, 0, array3, 0, num);
			Array.Copy(ipWithSubmask2, num, array4, 0, num);
			return new byte[][]
			{
				array,
				array2,
				array3,
				array4
			};
		}

		private byte[][] MinMaxIPs(byte[] ip1, byte[] subnetmask1, byte[] ip2, byte[] subnetmask2)
		{
			int num = ip1.Length;
			byte[] array = new byte[num];
			byte[] array2 = new byte[num];
			byte[] array3 = new byte[num];
			byte[] array4 = new byte[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = (ip1[i] & subnetmask1[i]);
				array2[i] = ((ip1[i] & subnetmask1[i]) | ~subnetmask1[i]);
				array3[i] = (ip2[i] & subnetmask2[i]);
				array4[i] = ((ip2[i] & subnetmask2[i]) | ~subnetmask2[i]);
			}
			return new byte[][]
			{
				array,
				array2,
				array3,
				array4
			};
		}

		private void CheckPermittedEmail(ISet permitted, string email)
		{
			if (permitted == null)
			{
				return;
			}
			IEnumerator enumerator = permitted.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string constraint = (string)enumerator.Current;
				if (this.EmailIsConstrained(email, constraint))
				{
					return;
				}
			}
			if (email.Length == 0 && permitted.Count == 0)
			{
				return;
			}
			throw new PkixNameConstraintValidatorException("Subject email address is not from a permitted subtree.");
		}

		private void CheckExcludedEmail(ISet excluded, string email)
		{
			if (excluded.IsEmpty)
			{
				return;
			}
			IEnumerator enumerator = excluded.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string constraint = (string)enumerator.Current;
				if (this.EmailIsConstrained(email, constraint))
				{
					throw new PkixNameConstraintValidatorException("Email address is from an excluded subtree.");
				}
			}
		}

		private void CheckPermittedIP(ISet permitted, byte[] ip)
		{
			if (permitted == null)
			{
				return;
			}
			IEnumerator enumerator = permitted.GetEnumerator();
			while (enumerator.MoveNext())
			{
				byte[] constraint = (byte[])enumerator.Current;
				if (this.IsIPConstrained(ip, constraint))
				{
					return;
				}
			}
			if (ip.Length == 0 && permitted.Count == 0)
			{
				return;
			}
			throw new PkixNameConstraintValidatorException("IP is not from a permitted subtree.");
		}

		private void checkExcludedIP(ISet excluded, byte[] ip)
		{
			if (excluded.IsEmpty)
			{
				return;
			}
			IEnumerator enumerator = excluded.GetEnumerator();
			while (enumerator.MoveNext())
			{
				byte[] constraint = (byte[])enumerator.Current;
				if (this.IsIPConstrained(ip, constraint))
				{
					throw new PkixNameConstraintValidatorException("IP is from an excluded subtree.");
				}
			}
		}

		private bool IsIPConstrained(byte[] ip, byte[] constraint)
		{
			int num = ip.Length;
			if (num != constraint.Length / 2)
			{
				return false;
			}
			byte[] array = new byte[num];
			Array.Copy(constraint, num, array, 0, num);
			byte[] array2 = new byte[num];
			byte[] array3 = new byte[num];
			for (int i = 0; i < num; i++)
			{
				array2[i] = (constraint[i] & array[i]);
				array3[i] = (ip[i] & array[i]);
			}
			return Arrays.AreEqual(array2, array3);
		}

		private bool EmailIsConstrained(string email, string constraint)
		{
			string text = email.Substring(email.IndexOf('@') + 1);
			if (constraint.IndexOf('@') != -1)
			{
				if (email.ToUpper().Equals(constraint.ToUpper()))
				{
					return true;
				}
			}
			else if (!constraint[0].Equals('.'))
			{
				if (text.ToUpper().Equals(constraint.ToUpper()))
				{
					return true;
				}
			}
			else if (this.WithinDomain(text, constraint))
			{
				return true;
			}
			return false;
		}

		private bool WithinDomain(string testDomain, string domain)
		{
			string text = domain;
			if (text.StartsWith("."))
			{
				text = text.Substring(1);
			}
			string[] array = text.Split(new char[]
			{
				'.'
			});
			string[] array2 = testDomain.Split(new char[]
			{
				'.'
			});
			if (array2.Length <= array.Length)
			{
				return false;
			}
			int num = array2.Length - array.Length;
			for (int i = -1; i < array.Length; i++)
			{
				if (i == -1)
				{
					if (array2[i + num].Equals(""))
					{
						return false;
					}
				}
				else if (Platform.CompareIgnoreCase(array2[i + num], array[i]) != 0)
				{
					return false;
				}
			}
			return true;
		}

		private void CheckPermittedDNS(ISet permitted, string dns)
		{
			if (permitted == null)
			{
				return;
			}
			IEnumerator enumerator = permitted.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string text = (string)enumerator.Current;
				if (this.WithinDomain(dns, text) || dns.ToUpper().Equals(text.ToUpper()))
				{
					return;
				}
			}
			if (dns.Length == 0 && permitted.Count == 0)
			{
				return;
			}
			throw new PkixNameConstraintValidatorException("DNS is not from a permitted subtree.");
		}

		private void checkExcludedDNS(ISet excluded, string dns)
		{
			if (excluded.IsEmpty)
			{
				return;
			}
			IEnumerator enumerator = excluded.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string text = (string)enumerator.Current;
				if (this.WithinDomain(dns, text) || Platform.CompareIgnoreCase(dns, text) == 0)
				{
					throw new PkixNameConstraintValidatorException("DNS is from an excluded subtree.");
				}
			}
		}

		private void unionEmail(string email1, string email2, ISet union)
		{
			if (email1.IndexOf('@') != -1)
			{
				string text = email1.Substring(email1.IndexOf('@') + 1);
				if (email2.IndexOf('@') != -1)
				{
					if (Platform.CompareIgnoreCase(email1, email2) == 0)
					{
						union.Add(email1);
						return;
					}
					union.Add(email1);
					union.Add(email2);
					return;
				}
				else if (email2.StartsWith("."))
				{
					if (this.WithinDomain(text, email2))
					{
						union.Add(email2);
						return;
					}
					union.Add(email1);
					union.Add(email2);
					return;
				}
				else
				{
					if (Platform.CompareIgnoreCase(text, email2) == 0)
					{
						union.Add(email2);
						return;
					}
					union.Add(email1);
					union.Add(email2);
					return;
				}
			}
			else if (email1.StartsWith("."))
			{
				if (email2.IndexOf('@') != -1)
				{
					string testDomain = email2.Substring(email1.IndexOf('@') + 1);
					if (this.WithinDomain(testDomain, email1))
					{
						union.Add(email1);
						return;
					}
					union.Add(email1);
					union.Add(email2);
					return;
				}
				else if (email2.StartsWith("."))
				{
					if (this.WithinDomain(email1, email2) || Platform.CompareIgnoreCase(email1, email2) == 0)
					{
						union.Add(email2);
						return;
					}
					if (this.WithinDomain(email2, email1))
					{
						union.Add(email1);
						return;
					}
					union.Add(email1);
					union.Add(email2);
					return;
				}
				else
				{
					if (this.WithinDomain(email2, email1))
					{
						union.Add(email1);
						return;
					}
					union.Add(email1);
					union.Add(email2);
					return;
				}
			}
			else if (email2.IndexOf('@') != -1)
			{
				string a = email2.Substring(email1.IndexOf('@') + 1);
				if (Platform.CompareIgnoreCase(a, email1) == 0)
				{
					union.Add(email1);
					return;
				}
				union.Add(email1);
				union.Add(email2);
				return;
			}
			else if (email2.StartsWith("."))
			{
				if (this.WithinDomain(email1, email2))
				{
					union.Add(email2);
					return;
				}
				union.Add(email1);
				union.Add(email2);
				return;
			}
			else
			{
				if (Platform.CompareIgnoreCase(email1, email2) == 0)
				{
					union.Add(email1);
					return;
				}
				union.Add(email1);
				union.Add(email2);
				return;
			}
		}

		private void unionURI(string email1, string email2, ISet union)
		{
			if (email1.IndexOf('@') != -1)
			{
				string text = email1.Substring(email1.IndexOf('@') + 1);
				if (email2.IndexOf('@') != -1)
				{
					if (Platform.CompareIgnoreCase(email1, email2) == 0)
					{
						union.Add(email1);
						return;
					}
					union.Add(email1);
					union.Add(email2);
					return;
				}
				else if (email2.StartsWith("."))
				{
					if (this.WithinDomain(text, email2))
					{
						union.Add(email2);
						return;
					}
					union.Add(email1);
					union.Add(email2);
					return;
				}
				else
				{
					if (Platform.CompareIgnoreCase(text, email2) == 0)
					{
						union.Add(email2);
						return;
					}
					union.Add(email1);
					union.Add(email2);
					return;
				}
			}
			else if (email1.StartsWith("."))
			{
				if (email2.IndexOf('@') != -1)
				{
					string testDomain = email2.Substring(email1.IndexOf('@') + 1);
					if (this.WithinDomain(testDomain, email1))
					{
						union.Add(email1);
						return;
					}
					union.Add(email1);
					union.Add(email2);
					return;
				}
				else if (email2.StartsWith("."))
				{
					if (this.WithinDomain(email1, email2) || Platform.CompareIgnoreCase(email1, email2) == 0)
					{
						union.Add(email2);
						return;
					}
					if (this.WithinDomain(email2, email1))
					{
						union.Add(email1);
						return;
					}
					union.Add(email1);
					union.Add(email2);
					return;
				}
				else
				{
					if (this.WithinDomain(email2, email1))
					{
						union.Add(email1);
						return;
					}
					union.Add(email1);
					union.Add(email2);
					return;
				}
			}
			else if (email2.IndexOf('@') != -1)
			{
				string a = email2.Substring(email1.IndexOf('@') + 1);
				if (Platform.CompareIgnoreCase(a, email1) == 0)
				{
					union.Add(email1);
					return;
				}
				union.Add(email1);
				union.Add(email2);
				return;
			}
			else if (email2.StartsWith("."))
			{
				if (this.WithinDomain(email1, email2))
				{
					union.Add(email2);
					return;
				}
				union.Add(email1);
				union.Add(email2);
				return;
			}
			else
			{
				if (Platform.CompareIgnoreCase(email1, email2) == 0)
				{
					union.Add(email1);
					return;
				}
				union.Add(email1);
				union.Add(email2);
				return;
			}
		}

		private ISet intersectDNS(ISet permitted, ISet dnss)
		{
			ISet set = new HashSet();
			IEnumerator enumerator = dnss.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string text = this.ExtractNameAsString(((GeneralSubtree)enumerator.Current).Base);
				if (permitted == null)
				{
					if (text != null)
					{
						set.Add(text);
					}
				}
				else
				{
					IEnumerator enumerator2 = permitted.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						string text2 = (string)enumerator2.Current;
						if (this.WithinDomain(text2, text))
						{
							set.Add(text2);
						}
						else if (this.WithinDomain(text, text2))
						{
							set.Add(text);
						}
					}
				}
			}
			return set;
		}

		protected ISet unionDNS(ISet excluded, string dns)
		{
			if (!excluded.IsEmpty)
			{
				ISet set = new HashSet();
				IEnumerator enumerator = excluded.GetEnumerator();
				while (enumerator.MoveNext())
				{
					string text = (string)enumerator.Current;
					if (this.WithinDomain(text, dns))
					{
						set.Add(dns);
					}
					else if (this.WithinDomain(dns, text))
					{
						set.Add(text);
					}
					else
					{
						set.Add(text);
						set.Add(dns);
					}
				}
				return set;
			}
			if (dns == null)
			{
				return excluded;
			}
			excluded.Add(dns);
			return excluded;
		}

		private void intersectEmail(string email1, string email2, ISet intersect)
		{
			if (email1.IndexOf('@') != -1)
			{
				string text = email1.Substring(email1.IndexOf('@') + 1);
				if (email2.IndexOf('@') != -1)
				{
					if (Platform.CompareIgnoreCase(email1, email2) == 0)
					{
						intersect.Add(email1);
						return;
					}
				}
				else if (email2.StartsWith("."))
				{
					if (this.WithinDomain(text, email2))
					{
						intersect.Add(email1);
						return;
					}
				}
				else if (Platform.CompareIgnoreCase(text, email2) == 0)
				{
					intersect.Add(email1);
					return;
				}
			}
			else if (email1.StartsWith("."))
			{
				if (email2.IndexOf('@') != -1)
				{
					string testDomain = email2.Substring(email1.IndexOf('@') + 1);
					if (this.WithinDomain(testDomain, email1))
					{
						intersect.Add(email2);
						return;
					}
				}
				else if (email2.StartsWith("."))
				{
					if (this.WithinDomain(email1, email2) || Platform.CompareIgnoreCase(email1, email2) == 0)
					{
						intersect.Add(email1);
						return;
					}
					if (this.WithinDomain(email2, email1))
					{
						intersect.Add(email2);
						return;
					}
				}
				else if (this.WithinDomain(email2, email1))
				{
					intersect.Add(email2);
					return;
				}
			}
			else if (email2.IndexOf('@') != -1)
			{
				string a = email2.Substring(email2.IndexOf('@') + 1);
				if (Platform.CompareIgnoreCase(a, email1) == 0)
				{
					intersect.Add(email2);
					return;
				}
			}
			else if (email2.StartsWith("."))
			{
				if (this.WithinDomain(email1, email2))
				{
					intersect.Add(email1);
					return;
				}
			}
			else if (Platform.CompareIgnoreCase(email1, email2) == 0)
			{
				intersect.Add(email1);
			}
		}

		private void checkExcludedURI(ISet excluded, string uri)
		{
			if (excluded.IsEmpty)
			{
				return;
			}
			IEnumerator enumerator = excluded.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string constraint = (string)enumerator.Current;
				if (this.IsUriConstrained(uri, constraint))
				{
					throw new PkixNameConstraintValidatorException("URI is from an excluded subtree.");
				}
			}
		}

		private ISet intersectURI(ISet permitted, ISet uris)
		{
			ISet set = new HashSet();
			IEnumerator enumerator = uris.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string text = this.ExtractNameAsString(((GeneralSubtree)enumerator.Current).Base);
				if (permitted == null)
				{
					if (text != null)
					{
						set.Add(text);
					}
				}
				else
				{
					IEnumerator enumerator2 = permitted.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						string email = (string)enumerator2.Current;
						this.intersectURI(email, text, set);
					}
				}
			}
			return set;
		}

		private ISet unionURI(ISet excluded, string uri)
		{
			if (!excluded.IsEmpty)
			{
				ISet set = new HashSet();
				IEnumerator enumerator = excluded.GetEnumerator();
				while (enumerator.MoveNext())
				{
					string email = (string)enumerator.Current;
					this.unionURI(email, uri, set);
				}
				return set;
			}
			if (uri == null)
			{
				return excluded;
			}
			excluded.Add(uri);
			return excluded;
		}

		private void intersectURI(string email1, string email2, ISet intersect)
		{
			if (email1.IndexOf('@') != -1)
			{
				string text = email1.Substring(email1.IndexOf('@') + 1);
				if (email2.IndexOf('@') != -1)
				{
					if (Platform.CompareIgnoreCase(email1, email2) == 0)
					{
						intersect.Add(email1);
						return;
					}
				}
				else if (email2.StartsWith("."))
				{
					if (this.WithinDomain(text, email2))
					{
						intersect.Add(email1);
						return;
					}
				}
				else if (Platform.CompareIgnoreCase(text, email2) == 0)
				{
					intersect.Add(email1);
					return;
				}
			}
			else if (email1.StartsWith("."))
			{
				if (email2.IndexOf('@') != -1)
				{
					string testDomain = email2.Substring(email1.IndexOf('@') + 1);
					if (this.WithinDomain(testDomain, email1))
					{
						intersect.Add(email2);
						return;
					}
				}
				else if (email2.StartsWith("."))
				{
					if (this.WithinDomain(email1, email2) || Platform.CompareIgnoreCase(email1, email2) == 0)
					{
						intersect.Add(email1);
						return;
					}
					if (this.WithinDomain(email2, email1))
					{
						intersect.Add(email2);
						return;
					}
				}
				else if (this.WithinDomain(email2, email1))
				{
					intersect.Add(email2);
					return;
				}
			}
			else if (email2.IndexOf('@') != -1)
			{
				string a = email2.Substring(email2.IndexOf('@') + 1);
				if (Platform.CompareIgnoreCase(a, email1) == 0)
				{
					intersect.Add(email2);
					return;
				}
			}
			else if (email2.StartsWith("."))
			{
				if (this.WithinDomain(email1, email2))
				{
					intersect.Add(email1);
					return;
				}
			}
			else if (Platform.CompareIgnoreCase(email1, email2) == 0)
			{
				intersect.Add(email1);
			}
		}

		private void CheckPermittedURI(ISet permitted, string uri)
		{
			if (permitted == null)
			{
				return;
			}
			IEnumerator enumerator = permitted.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string constraint = (string)enumerator.Current;
				if (this.IsUriConstrained(uri, constraint))
				{
					return;
				}
			}
			if (uri.Length == 0 && permitted.Count == 0)
			{
				return;
			}
			throw new PkixNameConstraintValidatorException("URI is not from a permitted subtree.");
		}

		private bool IsUriConstrained(string uri, string constraint)
		{
			string text = PkixNameConstraintValidator.ExtractHostFromURL(uri);
			if (!constraint.StartsWith("."))
			{
				if (Platform.CompareIgnoreCase(text, constraint) == 0)
				{
					return true;
				}
			}
			else if (this.WithinDomain(text, constraint))
			{
				return true;
			}
			return false;
		}

		private static string ExtractHostFromURL(string url)
		{
			string text = url.Substring(url.IndexOf(':') + 1);
			if (text.IndexOf("//") != -1)
			{
				text = text.Substring(text.IndexOf("//") + 2);
			}
			if (text.LastIndexOf(':') != -1)
			{
				text = text.Substring(0, text.LastIndexOf(':'));
			}
			text = text.Substring(text.IndexOf(':') + 1);
			text = text.Substring(text.IndexOf('@') + 1);
			if (text.IndexOf('/') != -1)
			{
				text = text.Substring(0, text.IndexOf('/'));
			}
			return text;
		}

		public void checkPermitted(GeneralName name)
		{
			switch (name.TagNo)
			{
			case 1:
				this.CheckPermittedEmail(this.permittedSubtreesEmail, this.ExtractNameAsString(name));
				return;
			case 2:
				this.CheckPermittedDNS(this.permittedSubtreesDNS, DerIA5String.GetInstance(name.Name).GetString());
				return;
			case 3:
			case 5:
				break;
			case 4:
				this.CheckPermittedDN(Asn1Sequence.GetInstance(name.Name.ToAsn1Object()));
				return;
			case 6:
				this.CheckPermittedURI(this.permittedSubtreesURI, DerIA5String.GetInstance(name.Name).GetString());
				return;
			case 7:
			{
				byte[] octets = Asn1OctetString.GetInstance(name.Name).GetOctets();
				this.CheckPermittedIP(this.permittedSubtreesIP, octets);
				break;
			}
			default:
				return;
			}
		}

		public void checkExcluded(GeneralName name)
		{
			switch (name.TagNo)
			{
			case 1:
				this.CheckExcludedEmail(this.excludedSubtreesEmail, this.ExtractNameAsString(name));
				return;
			case 2:
				this.checkExcludedDNS(this.excludedSubtreesDNS, DerIA5String.GetInstance(name.Name).GetString());
				return;
			case 3:
			case 5:
				break;
			case 4:
				this.CheckExcludedDN(Asn1Sequence.GetInstance(name.Name.ToAsn1Object()));
				return;
			case 6:
				this.checkExcludedURI(this.excludedSubtreesURI, DerIA5String.GetInstance(name.Name).GetString());
				return;
			case 7:
			{
				byte[] octets = Asn1OctetString.GetInstance(name.Name).GetOctets();
				this.checkExcludedIP(this.excludedSubtreesIP, octets);
				break;
			}
			default:
				return;
			}
		}

		public void IntersectPermittedSubtree(Asn1Sequence permitted)
		{
			IDictionary dictionary = Platform.CreateHashtable();
			IEnumerator enumerator = permitted.GetEnumerator();
			while (enumerator.MoveNext())
			{
				GeneralSubtree instance = GeneralSubtree.GetInstance(enumerator.Current);
				int tagNo = instance.Base.TagNo;
				if (dictionary[tagNo] == null)
				{
					dictionary[tagNo] = new HashSet();
				}
				((ISet)dictionary[tagNo]).Add(instance);
			}
			IEnumerator enumerator2 = dictionary.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator2.Current;
				switch ((int)dictionaryEntry.Key)
				{
				case 1:
					this.permittedSubtreesEmail = this.IntersectEmail(this.permittedSubtreesEmail, (ISet)dictionaryEntry.Value);
					break;
				case 2:
					this.permittedSubtreesDNS = this.intersectDNS(this.permittedSubtreesDNS, (ISet)dictionaryEntry.Value);
					break;
				case 4:
					this.permittedSubtreesDN = this.IntersectDN(this.permittedSubtreesDN, (ISet)dictionaryEntry.Value);
					break;
				case 6:
					this.permittedSubtreesURI = this.intersectURI(this.permittedSubtreesURI, (ISet)dictionaryEntry.Value);
					break;
				case 7:
					this.permittedSubtreesIP = this.IntersectIP(this.permittedSubtreesIP, (ISet)dictionaryEntry.Value);
					break;
				}
			}
		}

		private string ExtractNameAsString(GeneralName name)
		{
			return DerIA5String.GetInstance(name.Name).GetString();
		}

		public void IntersectEmptyPermittedSubtree(int nameType)
		{
			switch (nameType)
			{
			case 1:
				this.permittedSubtreesEmail = new HashSet();
				return;
			case 2:
				this.permittedSubtreesDNS = new HashSet();
				return;
			case 3:
			case 5:
				break;
			case 4:
				this.permittedSubtreesDN = new HashSet();
				return;
			case 6:
				this.permittedSubtreesURI = new HashSet();
				return;
			case 7:
				this.permittedSubtreesIP = new HashSet();
				break;
			default:
				return;
			}
		}

		public void AddExcludedSubtree(GeneralSubtree subtree)
		{
			GeneralName @base = subtree.Base;
			switch (@base.TagNo)
			{
			case 1:
				this.excludedSubtreesEmail = this.UnionEmail(this.excludedSubtreesEmail, this.ExtractNameAsString(@base));
				return;
			case 2:
				this.excludedSubtreesDNS = this.unionDNS(this.excludedSubtreesDNS, this.ExtractNameAsString(@base));
				return;
			case 3:
			case 5:
				break;
			case 4:
				this.excludedSubtreesDN = this.UnionDN(this.excludedSubtreesDN, (Asn1Sequence)@base.Name.ToAsn1Object());
				return;
			case 6:
				this.excludedSubtreesURI = this.unionURI(this.excludedSubtreesURI, this.ExtractNameAsString(@base));
				return;
			case 7:
				this.excludedSubtreesIP = this.UnionIP(this.excludedSubtreesIP, Asn1OctetString.GetInstance(@base.Name).GetOctets());
				break;
			default:
				return;
			}
		}

		private static byte[] Max(byte[] ip1, byte[] ip2)
		{
			for (int i = 0; i < ip1.Length; i++)
			{
				if (((int)ip1[i] & 65535) > ((int)ip2[i] & 65535))
				{
					return ip1;
				}
			}
			return ip2;
		}

		private static byte[] Min(byte[] ip1, byte[] ip2)
		{
			for (int i = 0; i < ip1.Length; i++)
			{
				if (((int)ip1[i] & 65535) < ((int)ip2[i] & 65535))
				{
					return ip1;
				}
			}
			return ip2;
		}

		private static int CompareTo(byte[] ip1, byte[] ip2)
		{
			if (Arrays.AreEqual(ip1, ip2))
			{
				return 0;
			}
			if (Arrays.AreEqual(PkixNameConstraintValidator.Max(ip1, ip2), ip1))
			{
				return 1;
			}
			return -1;
		}

		private static byte[] Or(byte[] ip1, byte[] ip2)
		{
			byte[] array = new byte[ip1.Length];
			for (int i = 0; i < ip1.Length; i++)
			{
				array[i] = (ip1[i] | ip2[i]);
			}
			return array;
		}

		[Obsolete("Use GetHashCode instead")]
		public int HashCode()
		{
			return this.GetHashCode();
		}

		public override int GetHashCode()
		{
			return this.HashCollection(this.excludedSubtreesDN) + this.HashCollection(this.excludedSubtreesDNS) + this.HashCollection(this.excludedSubtreesEmail) + this.HashCollection(this.excludedSubtreesIP) + this.HashCollection(this.excludedSubtreesURI) + this.HashCollection(this.permittedSubtreesDN) + this.HashCollection(this.permittedSubtreesDNS) + this.HashCollection(this.permittedSubtreesEmail) + this.HashCollection(this.permittedSubtreesIP) + this.HashCollection(this.permittedSubtreesURI);
		}

		private int HashCollection(ICollection coll)
		{
			if (coll == null)
			{
				return 0;
			}
			int num = 0;
			IEnumerator enumerator = coll.GetEnumerator();
			while (enumerator.MoveNext())
			{
				object current = enumerator.Current;
				if (current is byte[])
				{
					num += Arrays.GetHashCode((byte[])current);
				}
				else
				{
					num += current.GetHashCode();
				}
			}
			return num;
		}

		public override bool Equals(object o)
		{
			if (!(o is PkixNameConstraintValidator))
			{
				return false;
			}
			PkixNameConstraintValidator pkixNameConstraintValidator = (PkixNameConstraintValidator)o;
			return this.CollectionsAreEqual(pkixNameConstraintValidator.excludedSubtreesDN, this.excludedSubtreesDN) && this.CollectionsAreEqual(pkixNameConstraintValidator.excludedSubtreesDNS, this.excludedSubtreesDNS) && this.CollectionsAreEqual(pkixNameConstraintValidator.excludedSubtreesEmail, this.excludedSubtreesEmail) && this.CollectionsAreEqual(pkixNameConstraintValidator.excludedSubtreesIP, this.excludedSubtreesIP) && this.CollectionsAreEqual(pkixNameConstraintValidator.excludedSubtreesURI, this.excludedSubtreesURI) && this.CollectionsAreEqual(pkixNameConstraintValidator.permittedSubtreesDN, this.permittedSubtreesDN) && this.CollectionsAreEqual(pkixNameConstraintValidator.permittedSubtreesDNS, this.permittedSubtreesDNS) && this.CollectionsAreEqual(pkixNameConstraintValidator.permittedSubtreesEmail, this.permittedSubtreesEmail) && this.CollectionsAreEqual(pkixNameConstraintValidator.permittedSubtreesIP, this.permittedSubtreesIP) && this.CollectionsAreEqual(pkixNameConstraintValidator.permittedSubtreesURI, this.permittedSubtreesURI);
		}

		private bool CollectionsAreEqual(ICollection coll1, ICollection coll2)
		{
			if (coll1 == coll2)
			{
				return true;
			}
			if (coll1 == null || coll2 == null)
			{
				return false;
			}
			if (coll1.Count != coll2.Count)
			{
				return false;
			}
			IEnumerator enumerator = coll1.GetEnumerator();
			while (enumerator.MoveNext())
			{
				object current = enumerator.Current;
				IEnumerator enumerator2 = coll2.GetEnumerator();
				bool flag = false;
				while (enumerator2.MoveNext())
				{
					object current2 = enumerator2.Current;
					if (this.SpecialEquals(current, current2))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		private bool SpecialEquals(object o1, object o2)
		{
			if (o1 == o2)
			{
				return true;
			}
			if (o1 == null || o2 == null)
			{
				return false;
			}
			if (o1 is byte[] && o2 is byte[])
			{
				return Arrays.AreEqual((byte[])o1, (byte[])o2);
			}
			return o1.Equals(o2);
		}

		private string StringifyIP(byte[] ip)
		{
			string text = "";
			for (int i = 0; i < ip.Length / 2; i++)
			{
				text = text + (int)(ip[i] & 255) + ".";
			}
			text = text.Substring(0, text.Length - 1);
			text += "/";
			for (int j = ip.Length / 2; j < ip.Length; j++)
			{
				text = text + (int)(ip[j] & 255) + ".";
			}
			return text.Substring(0, text.Length - 1);
		}

		private string StringifyIPCollection(ISet ips)
		{
			string text = "";
			text += "[";
			IEnumerator enumerator = ips.GetEnumerator();
			while (enumerator.MoveNext())
			{
				text = text + this.StringifyIP((byte[])enumerator.Current) + ",";
			}
			if (text.Length > 1)
			{
				text = text.Substring(0, text.Length - 1);
			}
			return text + "]";
		}

		public override string ToString()
		{
			string text = "";
			text += "permitted:\n";
			if (this.permittedSubtreesDN != null)
			{
				text += "DN:\n";
				text = text + this.permittedSubtreesDN.ToString() + "\n";
			}
			if (this.permittedSubtreesDNS != null)
			{
				text += "DNS:\n";
				text = text + this.permittedSubtreesDNS.ToString() + "\n";
			}
			if (this.permittedSubtreesEmail != null)
			{
				text += "Email:\n";
				text = text + this.permittedSubtreesEmail.ToString() + "\n";
			}
			if (this.permittedSubtreesURI != null)
			{
				text += "URI:\n";
				text = text + this.permittedSubtreesURI.ToString() + "\n";
			}
			if (this.permittedSubtreesIP != null)
			{
				text += "IP:\n";
				text = text + this.StringifyIPCollection(this.permittedSubtreesIP) + "\n";
			}
			text += "excluded:\n";
			if (!this.excludedSubtreesDN.IsEmpty)
			{
				text += "DN:\n";
				text = text + this.excludedSubtreesDN.ToString() + "\n";
			}
			if (!this.excludedSubtreesDNS.IsEmpty)
			{
				text += "DNS:\n";
				text = text + this.excludedSubtreesDNS.ToString() + "\n";
			}
			if (!this.excludedSubtreesEmail.IsEmpty)
			{
				text += "Email:\n";
				text = text + this.excludedSubtreesEmail.ToString() + "\n";
			}
			if (!this.excludedSubtreesURI.IsEmpty)
			{
				text += "URI:\n";
				text = text + this.excludedSubtreesURI.ToString() + "\n";
			}
			if (!this.excludedSubtreesIP.IsEmpty)
			{
				text += "IP:\n";
				text = text + this.StringifyIPCollection(this.excludedSubtreesIP) + "\n";
			}
			return text;
		}
	}
}
