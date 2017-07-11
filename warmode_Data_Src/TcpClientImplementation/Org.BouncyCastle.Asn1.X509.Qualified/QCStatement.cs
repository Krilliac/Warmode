using System;

namespace Org.BouncyCastle.Asn1.X509.Qualified
{
	public class QCStatement : Asn1Encodable
	{
		private readonly DerObjectIdentifier qcStatementId;

		private readonly Asn1Encodable qcStatementInfo;

		public DerObjectIdentifier StatementId
		{
			get
			{
				return this.qcStatementId;
			}
		}

		public Asn1Encodable StatementInfo
		{
			get
			{
				return this.qcStatementInfo;
			}
		}

		public static QCStatement GetInstance(object obj)
		{
			if (obj == null || obj is QCStatement)
			{
				return (QCStatement)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new QCStatement(Asn1Sequence.GetInstance(obj));
			}
			throw new ArgumentException("unknown object in GetInstance: " + obj.GetType().FullName, "obj");
		}

		private QCStatement(Asn1Sequence seq)
		{
			this.qcStatementId = DerObjectIdentifier.GetInstance(seq[0]);
			if (seq.Count > 1)
			{
				this.qcStatementInfo = seq[1];
			}
		}

		public QCStatement(DerObjectIdentifier qcStatementId)
		{
			this.qcStatementId = qcStatementId;
		}

		public QCStatement(DerObjectIdentifier qcStatementId, Asn1Encodable qcStatementInfo)
		{
			this.qcStatementId = qcStatementId;
			this.qcStatementInfo = qcStatementInfo;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.qcStatementId
			});
			if (this.qcStatementInfo != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					this.qcStatementInfo
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
