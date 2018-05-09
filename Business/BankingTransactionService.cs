using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Data.Models;

namespace Business
{
	public class BankingTransactionService
    {
	    private readonly BankingDataContext _context;
		
		//dbcontext is injected via constructor
	    public BankingTransactionService(BankingDataContext context)
	    {
		    _context = context;
	    }

		//decided against using a repository class for each data object since for this sample codebase,
		//the repos would mostly be simple wrappers instead of particularly useful CRUD encapsulations
	    public Account GetAccountById(int accountId)
	    {
		    return _context.Accounts.Single(a => a.Id == accountId);
		}

		//prevents negative amounts, which would be confusing or misleading given the method names
	    public void EnsureAmountPositive(decimal amount)
	    {
		    //could use uint for limiting amount to positive only, but that would require devs to use a lot of casts to int
		    if (amount < 0)
		    {
			    throw new ArgumentException("Removal amount cannot be a negative value");
		    }
	    }

		//Could use a single method called ChangeFundsForAccount which could take a positive or negative value,
		//in order to replace the remove and add funds methods,
		//but I think that is more confusing from a development perspective than explicitely named separate methods	
		public void RemoveFundsFromAccount(int accountId, decimal amount)
	    {
		    EnsureAmountPositive(amount);
			Account account = GetAccountById(accountId);

			//prevents any type of fund removal (withdrawal, transfer) from overdrafting account
			if (amount > account.Funds)
			{
				throw new WithdrawalExceedsAvailableFundsException("It is not permissible to overdraft an account");
			}

			account.Funds -= amount;
		}

	    public void AddFundsToAccount(int accountId, decimal amount)
	    {
		    EnsureAmountPositive(amount);
			Account account = GetAccountById(accountId);
		    account.Funds += amount;
		}

		public void WithdrawFromAccount(int accountId, decimal amount)
		{
			Account account = GetAccountById(accountId);

			//prevents withdrawal, but not transfer, of more than $1000 from invididual investment accounts
			if (account.AccountType == AccountTypeEnum.IndividualInvestment && amount > 1000)
		    {
			    throw new ExceededMaxWithdrawalException("Individual Investment Accounts may not withdraw more than $1000");
		    }

			RemoveFundsFromAccount(accountId, amount);
			_context.SaveChanges();
		}

	    public void DepositToAccount(int accountId, decimal amount)
	    {
			AddFundsToAccount(accountId, amount);
		    _context.SaveChanges();
		}

	    public void Transfer(int fromAccountId, int toAccountId, decimal amount)
	    {
			//entity framework should ensure that either both of these changes occur or neither occur, using a transaction with rollback capability
			RemoveFundsFromAccount(fromAccountId, amount);
			AddFundsToAccount(toAccountId, amount);
		    _context.SaveChanges();
		}
	}
}
