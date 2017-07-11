using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.X509.Store;
using System;
using System.Text;

namespace Org.BouncyCastle.Pkix
{
	public class PkixBuilderParameters : PkixParameters
	{
		private int maxPathLength = 5;

		private ISet excludedCerts = new HashSet();

		public virtual int MaxPathLength
		{
			get
			{
				return this.maxPathLength;
			}
			set
			{
				if (value < -1)
				{
					throw new InvalidParameterException("The maximum path length parameter can not be less than -1.");
				}
				this.maxPathLength = value;
			}
		}

		public static PkixBuilderParameters GetInstance(PkixParameters pkixParams)
		{
			PkixBuilderParameters pkixBuilderParameters = new PkixBuilderParameters(pkixParams.GetTrustAnchors(), new X509CertStoreSelector(pkixParams.GetTargetCertConstraints()));
			pkixBuilderParameters.SetParams(pkixParams);
			return pkixBuilderParameters;
		}

		public PkixBuilderParameters(ISet trustAnchors, IX509Selector targetConstraints) : base(trustAnchors)
		{
			this.SetTargetCertConstraints(targetConstraints);
		}

		public virtual ISet GetExcludedCerts()
		{
			return new HashSet(this.excludedCerts);
		}

		public virtual void SetExcludedCerts(ISet excludedCerts)
		{
			if (excludedCerts == null)
			{
				excludedCerts = new HashSet();
				return;
			}
			this.excludedCerts = new HashSet(excludedCerts);
		}

		protected override void SetParams(PkixParameters parameters)
		{
			base.SetParams(parameters);
			if (parameters is PkixBuilderParameters)
			{
				PkixBuilderParameters pkixBuilderParameters = (PkixBuilderParameters)parameters;
				this.maxPathLength = pkixBuilderParameters.maxPathLength;
				this.excludedCerts = new HashSet(pkixBuilderParameters.excludedCerts);
			}
		}

		public override object Clone()
		{
			PkixBuilderParameters pkixBuilderParameters = new PkixBuilderParameters(this.GetTrustAnchors(), this.GetTargetCertConstraints());
			pkixBuilderParameters.SetParams(this);
			return pkixBuilderParameters;
		}

		public override string ToString()
		{
			string newLine = Platform.NewLine;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("PkixBuilderParameters [" + newLine);
			stringBuilder.Append(base.ToString());
			stringBuilder.Append("  Maximum Path Length: ");
			stringBuilder.Append(this.MaxPathLength);
			stringBuilder.Append(newLine + "]" + newLine);
			return stringBuilder.ToString();
		}
	}
}
