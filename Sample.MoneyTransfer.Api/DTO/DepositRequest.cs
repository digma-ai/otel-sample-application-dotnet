using System;
namespace Sample.MoneyTransfer.API.DTO
{
	public class DepositRequest
	{
		public long AccountId { get; set; }
		public int Amount { get; set; }
	
	}
}

