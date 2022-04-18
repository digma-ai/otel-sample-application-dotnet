using System;
namespace Sample.MoneyTransfer.API.Domain.Models
{
	public class Account
	{
		public long Id { get; set; }

		public int Balance { get; set; }

		public string AccountName { get; set; }

		public bool Internal { get; set; }

	}
}

