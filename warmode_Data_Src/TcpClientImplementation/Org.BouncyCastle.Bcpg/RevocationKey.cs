using System;

namespace Org.BouncyCastle.Bcpg
{
	public class RevocationKey : SignatureSubpacket
	{
		public virtual RevocationKeyTag SignatureClass
		{
			get
			{
				return (RevocationKeyTag)base.GetData()[0];
			}
		}

		public virtual PublicKeyAlgorithmTag Algorithm
		{
			get
			{
				return (PublicKeyAlgorithmTag)base.GetData()[1];
			}
		}

		public RevocationKey(bool isCritical, byte[] data) : base(SignatureSubpacketTag.RevocationKey, isCritical, data)
		{
		}

		public RevocationKey(bool isCritical, RevocationKeyTag signatureClass, PublicKeyAlgorithmTag keyAlgorithm, byte[] fingerprint) : base(SignatureSubpacketTag.RevocationKey, isCritical, RevocationKey.CreateData(signatureClass, keyAlgorithm, fingerprint))
		{
		}

		private static byte[] CreateData(RevocationKeyTag signatureClass, PublicKeyAlgorithmTag keyAlgorithm, byte[] fingerprint)
		{
			byte[] array = new byte[2 + fingerprint.Length];
			array[0] = (byte)signatureClass;
			array[1] = (byte)keyAlgorithm;
			Array.Copy(fingerprint, 0, array, 2, fingerprint.Length);
			return array;
		}

		public virtual byte[] GetFingerprint()
		{
			byte[] data = base.GetData();
			byte[] array = new byte[data.Length - 2];
			Array.Copy(data, 2, array, 0, array.Length);
			return array;
		}
	}
}
