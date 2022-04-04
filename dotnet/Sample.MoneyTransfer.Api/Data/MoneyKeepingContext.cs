using System;
using Microsoft.EntityFrameworkCore;
using Sample.MoneyTransfer.API.Domain.Models;

namespace Sample.MoneyTransfer.API.Data
{
	public class MoneyKeepingContext : DbContext
    {
        public MoneyKeepingContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<TransferRecord> Transfers { get; set; }
    }
}

