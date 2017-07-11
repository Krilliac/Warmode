using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections;
using System.Text;

namespace Org.BouncyCastle.Pkix
{
	public class PkixPolicyNode
	{
		protected IList mChildren;

		protected int mDepth;

		protected ISet mExpectedPolicies;

		protected PkixPolicyNode mParent;

		protected ISet mPolicyQualifiers;

		protected string mValidPolicy;

		protected bool mCritical;

		public virtual int Depth
		{
			get
			{
				return this.mDepth;
			}
		}

		public virtual IEnumerable Children
		{
			get
			{
				return new EnumerableProxy(this.mChildren);
			}
		}

		public virtual bool IsCritical
		{
			get
			{
				return this.mCritical;
			}
			set
			{
				this.mCritical = value;
			}
		}

		public virtual ISet PolicyQualifiers
		{
			get
			{
				return new HashSet(this.mPolicyQualifiers);
			}
		}

		public virtual string ValidPolicy
		{
			get
			{
				return this.mValidPolicy;
			}
		}

		public virtual bool HasChildren
		{
			get
			{
				return this.mChildren.Count != 0;
			}
		}

		public virtual ISet ExpectedPolicies
		{
			get
			{
				return new HashSet(this.mExpectedPolicies);
			}
			set
			{
				this.mExpectedPolicies = new HashSet(value);
			}
		}

		public virtual PkixPolicyNode Parent
		{
			get
			{
				return this.mParent;
			}
			set
			{
				this.mParent = value;
			}
		}

		public PkixPolicyNode(IList children, int depth, ISet expectedPolicies, PkixPolicyNode parent, ISet policyQualifiers, string validPolicy, bool critical)
		{
			if (children == null)
			{
				this.mChildren = Platform.CreateArrayList();
			}
			else
			{
				this.mChildren = Platform.CreateArrayList(children);
			}
			this.mDepth = depth;
			this.mExpectedPolicies = expectedPolicies;
			this.mParent = parent;
			this.mPolicyQualifiers = policyQualifiers;
			this.mValidPolicy = validPolicy;
			this.mCritical = critical;
		}

		public virtual void AddChild(PkixPolicyNode child)
		{
			child.Parent = this;
			this.mChildren.Add(child);
		}

		public virtual void RemoveChild(PkixPolicyNode child)
		{
			this.mChildren.Remove(child);
		}

		public override string ToString()
		{
			return this.ToString("");
		}

		public virtual string ToString(string indent)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(indent);
			stringBuilder.Append(this.mValidPolicy);
			stringBuilder.Append(" {");
			stringBuilder.Append(Platform.NewLine);
			foreach (PkixPolicyNode pkixPolicyNode in this.mChildren)
			{
				stringBuilder.Append(pkixPolicyNode.ToString(indent + "    "));
			}
			stringBuilder.Append(indent);
			stringBuilder.Append("}");
			stringBuilder.Append(Platform.NewLine);
			return stringBuilder.ToString();
		}

		public virtual object Clone()
		{
			return this.Copy();
		}

		public virtual PkixPolicyNode Copy()
		{
			PkixPolicyNode pkixPolicyNode = new PkixPolicyNode(Platform.CreateArrayList(), this.mDepth, new HashSet(this.mExpectedPolicies), null, new HashSet(this.mPolicyQualifiers), this.mValidPolicy, this.mCritical);
			foreach (PkixPolicyNode pkixPolicyNode2 in this.mChildren)
			{
				PkixPolicyNode pkixPolicyNode3 = pkixPolicyNode2.Copy();
				pkixPolicyNode3.Parent = pkixPolicyNode;
				pkixPolicyNode.AddChild(pkixPolicyNode3);
			}
			return pkixPolicyNode;
		}
	}
}
