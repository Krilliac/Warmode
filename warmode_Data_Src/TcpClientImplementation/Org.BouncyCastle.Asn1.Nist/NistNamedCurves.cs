using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections;

namespace Org.BouncyCastle.Asn1.Nist
{
	public sealed class NistNamedCurves
	{
		private static readonly IDictionary objIds;

		private static readonly IDictionary names;

		public static IEnumerable Names
		{
			get
			{
				return new EnumerableProxy(NistNamedCurves.names.Values);
			}
		}

		private NistNamedCurves()
		{
		}

		private static void DefineCurveAlias(string name, DerObjectIdentifier oid)
		{
			NistNamedCurves.objIds.Add(Platform.ToUpperInvariant(name), oid);
			NistNamedCurves.names.Add(oid, name);
		}

		static NistNamedCurves()
		{
			NistNamedCurves.objIds = Platform.CreateHashtable();
			NistNamedCurves.names = Platform.CreateHashtable();
			NistNamedCurves.DefineCurveAlias("B-163", SecObjectIdentifiers.SecT163r2);
			NistNamedCurves.DefineCurveAlias("B-233", SecObjectIdentifiers.SecT233r1);
			NistNamedCurves.DefineCurveAlias("B-283", SecObjectIdentifiers.SecT283r1);
			NistNamedCurves.DefineCurveAlias("B-409", SecObjectIdentifiers.SecT409r1);
			NistNamedCurves.DefineCurveAlias("B-571", SecObjectIdentifiers.SecT571r1);
			NistNamedCurves.DefineCurveAlias("K-163", SecObjectIdentifiers.SecT163k1);
			NistNamedCurves.DefineCurveAlias("K-233", SecObjectIdentifiers.SecT233k1);
			NistNamedCurves.DefineCurveAlias("K-283", SecObjectIdentifiers.SecT283k1);
			NistNamedCurves.DefineCurveAlias("K-409", SecObjectIdentifiers.SecT409k1);
			NistNamedCurves.DefineCurveAlias("K-571", SecObjectIdentifiers.SecT571k1);
			NistNamedCurves.DefineCurveAlias("P-192", SecObjectIdentifiers.SecP192r1);
			NistNamedCurves.DefineCurveAlias("P-224", SecObjectIdentifiers.SecP224r1);
			NistNamedCurves.DefineCurveAlias("P-256", SecObjectIdentifiers.SecP256r1);
			NistNamedCurves.DefineCurveAlias("P-384", SecObjectIdentifiers.SecP384r1);
			NistNamedCurves.DefineCurveAlias("P-521", SecObjectIdentifiers.SecP521r1);
		}

		public static X9ECParameters GetByName(string name)
		{
			DerObjectIdentifier oid = NistNamedCurves.GetOid(name);
			if (oid != null)
			{
				return NistNamedCurves.GetByOid(oid);
			}
			return null;
		}

		public static X9ECParameters GetByOid(DerObjectIdentifier oid)
		{
			return SecNamedCurves.GetByOid(oid);
		}

		public static DerObjectIdentifier GetOid(string name)
		{
			return (DerObjectIdentifier)NistNamedCurves.objIds[Platform.ToUpperInvariant(name)];
		}

		public static string GetName(DerObjectIdentifier oid)
		{
			return (string)NistNamedCurves.names[oid];
		}
	}
}
