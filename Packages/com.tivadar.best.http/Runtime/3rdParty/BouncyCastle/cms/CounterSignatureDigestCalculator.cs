#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;

using Best.HTTP.SecureProtocol.Org.BouncyCastle.Crypto;
using Best.HTTP.SecureProtocol.Org.BouncyCastle.Security;

namespace Best.HTTP.SecureProtocol.Org.BouncyCastle.Cms
{
	internal class CounterSignatureDigestCalculator
		: IDigestCalculator
	{
		private readonly string alg;
		private readonly byte[] data;

		internal CounterSignatureDigestCalculator(
			string	alg,
			byte[]	data)
		{
			this.alg = alg;
			this.data = data;
		}

		public byte[] GetDigest()
		{
			IDigest digest = CmsSignedHelper.Instance.GetDigestInstance(alg);
			return DigestUtilities.DoFinal(digest, data);
		}
	}
}
#pragma warning restore
#endif
