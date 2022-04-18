using System;
namespace Sample.MoneyTransfer.API.DTO
{
	public class NewAccountRequest
	{
		public string AccountName { get; set; }
		public bool Internal { get; set; }
		public int InitialFunds { get; set; }

	}
}

