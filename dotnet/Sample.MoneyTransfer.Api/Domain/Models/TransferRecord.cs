using System;
namespace Sample.MoneyTransfer.API.Domain.Models
{
	public class TransferRecord
	{
		public long Id { get; set; }
		public Account SourceAccount { get; set; }
		public Account TargetAccount { get; set; }
		public int Amount { get; set; }
		public DateTime TransferTime { get; set; }

	}
}

