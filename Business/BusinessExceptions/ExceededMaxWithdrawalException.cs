using System;
using System.Runtime.Serialization;

namespace Business
{
	[Serializable]
	public class ExceededMaxWithdrawalException : Exception
	{
		public ExceededMaxWithdrawalException()
		{
		}

		public ExceededMaxWithdrawalException(string message) : base(message)
		{
		}

		public ExceededMaxWithdrawalException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected ExceededMaxWithdrawalException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}