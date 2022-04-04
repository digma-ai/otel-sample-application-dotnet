using System;
namespace Sample.MoneyTransfer.API.Domain.Models
{
	public class TransferRecord
	{
		public string Id { get; set; }
		public Account SourceAccountId { get; set; }
		public Account TargetAccountId { get; set; }

		public DateTime TransferTime { get; set; }

	}
}

