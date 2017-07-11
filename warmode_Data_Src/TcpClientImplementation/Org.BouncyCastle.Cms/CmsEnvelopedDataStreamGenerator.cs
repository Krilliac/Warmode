using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.IO;
using System;
using System.IO;

namespace Org.BouncyCastle.Cms
{
	public class CmsEnvelopedDataStreamGenerator : CmsEnvelopedGenerator
	{
		private class CmsEnvelopedDataOutputStream : BaseOutputStream
		{
			private readonly CmsEnvelopedGenerator _outer;

			private readonly CipherStream _out;

			private readonly BerSequenceGenerator _cGen;

			private readonly BerSequenceGenerator _envGen;

			private readonly BerSequenceGenerator _eiGen;

			public CmsEnvelopedDataOutputStream(CmsEnvelopedGenerator outer, CipherStream outStream, BerSequenceGenerator cGen, BerSequenceGenerator envGen, BerSequenceGenerator eiGen)
			{
				this._outer = outer;
				this._out = outStream;
				this._cGen = cGen;
				this._envGen = envGen;
				this._eiGen = eiGen;
			}

			public override void WriteByte(byte b)
			{
				this._out.WriteByte(b);
			}

			public override void Write(byte[] bytes, int off, int len)
			{
				this._out.Write(bytes, off, len);
			}

			public override void Close()
			{
				this._out.Close();
				this._eiGen.Close();
				if (this._outer.unprotectedAttributeGenerator != null)
				{
					Org.BouncyCastle.Asn1.Cms.AttributeTable attributes = this._outer.unprotectedAttributeGenerator.GetAttributes(Platform.CreateHashtable());
					Asn1Set obj = new BerSet(attributes.ToAsn1EncodableVector());
					this._envGen.AddObject(new DerTaggedObject(false, 1, obj));
				}
				this._envGen.Close();
				this._cGen.Close();
				base.Close();
			}
		}

		private object _originatorInfo;

		private object _unprotectedAttributes;

		private int _bufferSize;

		private bool _berEncodeRecipientSet;

		private DerInteger Version
		{
			get
			{
				int value = (this._originatorInfo != null || this._unprotectedAttributes != null) ? 2 : 0;
				return new DerInteger(value);
			}
		}

		public CmsEnvelopedDataStreamGenerator()
		{
		}

		public CmsEnvelopedDataStreamGenerator(SecureRandom rand) : base(rand)
		{
		}

		public void SetBufferSize(int bufferSize)
		{
			this._bufferSize = bufferSize;
		}

		public void SetBerEncodeRecipients(bool berEncodeRecipientSet)
		{
			this._berEncodeRecipientSet = berEncodeRecipientSet;
		}

		private Stream Open(Stream outStream, string encryptionOid, CipherKeyGenerator keyGen)
		{
			byte[] array = keyGen.GenerateKey();
			KeyParameter keyParameter = ParameterUtilities.CreateKeyParameter(encryptionOid, array);
			Asn1Encodable asn1Params = this.GenerateAsn1Parameters(encryptionOid, array);
			ICipherParameters cipherParameters;
			AlgorithmIdentifier algorithmIdentifier = this.GetAlgorithmIdentifier(encryptionOid, keyParameter, asn1Params, out cipherParameters);
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			foreach (RecipientInfoGenerator recipientInfoGenerator in this.recipientInfoGenerators)
			{
				try
				{
					asn1EncodableVector.Add(new Asn1Encodable[]
					{
						recipientInfoGenerator.Generate(keyParameter, this.rand)
					});
				}
				catch (InvalidKeyException e)
				{
					throw new CmsException("key inappropriate for algorithm.", e);
				}
				catch (GeneralSecurityException e2)
				{
					throw new CmsException("error making encrypted content.", e2);
				}
			}
			return this.Open(outStream, algorithmIdentifier, cipherParameters, asn1EncodableVector);
		}

		private Stream Open(Stream outStream, AlgorithmIdentifier encAlgID, ICipherParameters cipherParameters, Asn1EncodableVector recipientInfos)
		{
			Stream result;
			try
			{
				BerSequenceGenerator berSequenceGenerator = new BerSequenceGenerator(outStream);
				berSequenceGenerator.AddObject(CmsObjectIdentifiers.EnvelopedData);
				BerSequenceGenerator berSequenceGenerator2 = new BerSequenceGenerator(berSequenceGenerator.GetRawOutputStream(), 0, true);
				berSequenceGenerator2.AddObject(this.Version);
				Stream rawOutputStream = berSequenceGenerator2.GetRawOutputStream();
				Asn1Generator asn1Generator = this._berEncodeRecipientSet ? new BerSetGenerator(rawOutputStream) : new DerSetGenerator(rawOutputStream);
				foreach (Asn1Encodable obj in recipientInfos)
				{
					asn1Generator.AddObject(obj);
				}
				asn1Generator.Close();
				BerSequenceGenerator berSequenceGenerator3 = new BerSequenceGenerator(rawOutputStream);
				berSequenceGenerator3.AddObject(CmsObjectIdentifiers.Data);
				berSequenceGenerator3.AddObject(encAlgID);
				Stream stream = CmsUtilities.CreateBerOctetOutputStream(berSequenceGenerator3.GetRawOutputStream(), 0, false, this._bufferSize);
				IBufferedCipher cipher = CipherUtilities.GetCipher(encAlgID.ObjectID);
				cipher.Init(true, new ParametersWithRandom(cipherParameters, this.rand));
				CipherStream outStream2 = new CipherStream(stream, null, cipher);
				result = new CmsEnvelopedDataStreamGenerator.CmsEnvelopedDataOutputStream(this, outStream2, berSequenceGenerator, berSequenceGenerator2, berSequenceGenerator3);
			}
			catch (SecurityUtilityException e)
			{
				throw new CmsException("couldn't create cipher.", e);
			}
			catch (InvalidKeyException e2)
			{
				throw new CmsException("key invalid in message.", e2);
			}
			catch (IOException e3)
			{
				throw new CmsException("exception decoding algorithm parameters.", e3);
			}
			return result;
		}

		public Stream Open(Stream outStream, string encryptionOid)
		{
			CipherKeyGenerator keyGenerator = GeneratorUtilities.GetKeyGenerator(encryptionOid);
			keyGenerator.Init(new KeyGenerationParameters(this.rand, keyGenerator.DefaultStrength));
			return this.Open(outStream, encryptionOid, keyGenerator);
		}

		public Stream Open(Stream outStream, string encryptionOid, int keySize)
		{
			CipherKeyGenerator keyGenerator = GeneratorUtilities.GetKeyGenerator(encryptionOid);
			keyGenerator.Init(new KeyGenerationParameters(this.rand, keySize));
			return this.Open(outStream, encryptionOid, keyGenerator);
		}
	}
}
