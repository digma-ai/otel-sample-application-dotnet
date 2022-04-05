using System;
namespace Sample.MoneyTransfer.API.DTO
{
	public class TransferRequest
	{
		public long SouceAccountId { get; set; }
		public long TargetAccountId { get; set; }
		public int Amount { get; set; }

	}
}

