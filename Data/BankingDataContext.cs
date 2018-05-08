using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data
{
	public class BankingDataContext : DbContext
	{
		public BankingDataContext()
		{

		}

		public BankingDataContext(DbContextOptions<BankingDataContext> options)
			: base(options)
		{ }

		public virtual DbSet<Bank> Banks { get; set; }
		public virtual DbSet<Account> Accounts { get; set; }
		public virtual DbSet<AccountType> AccountTypes { get; set; }
		public virtual DbSet<Owner> Owners { get; set; }
	}
}
