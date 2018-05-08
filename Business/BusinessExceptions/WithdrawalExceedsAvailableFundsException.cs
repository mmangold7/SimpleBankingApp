using System;
using System.Runtime.Serialization;

namespace Business
{
	[Serializable]
	public class WithdrawalExceedsAvailableFundsException : Exception
	{
		public WithdrawalExceedsAvailableFundsException()
		{
		}

		public WithdrawalExceedsAvailableFundsException(string message) : base(message)
		{
		}

		public WithdrawalExceedsAvailableFundsException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected WithdrawalExceedsAvailableFundsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}