using Business;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Data.Models;

namespace UnitTests
{
	[TestClass]
    public class TransactionTests
	{
		[TestMethod]
		public void CanWithdrawFromAccount()
		{
			var data = new List<Account>
			{
				new Account { Id = 3, Name = "Account 3", Funds = 75000, AccountType = AccountTypeEnum.CorporateInvestment}
			}.AsQueryable();

			var mockSet = new Mock<DbSet<Account>>();
			mockSet.As<IQueryable<Account>>().Setup(m => m.Provider).Returns(data.Provider);
			mockSet.As<IQueryable<Account>>().Setup(m => m.Expression).Returns(data.Expression);
			mockSet.As<IQueryable<Account>>().Setup(m => m.ElementType).Returns(data.ElementType);
			mockSet.As<IQueryable<Account>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

			var mockContext = new Mock<BankingDataContext>();
			mockContext.Setup(m => m.Accounts).Returns(mockSet.Object);

			var service = new BankingTransactionService(mockContext.Object);

			//progressively withdraw funds to test different amounts
			Account account3 = service.GetAccountById(3);
			Assert.AreEqual(account3.Funds, 75000);
			service.WithdrawFromAccount(3, 10000); //minus $10,000
			Assert.AreEqual(account3.Funds, 65000);
			service.WithdrawFromAccount(3, 50000); //minus $50,000
			Assert.AreEqual(account3.Funds, 15000);
			service.WithdrawFromAccount(3, 10); //minus $10
			Assert.AreEqual(account3.Funds, 14990);
		}

		[TestMethod]
		public void CanDepositToAccount()
		{
			var data = new List<Account>
			{
				new Account { Id = 2, Name = "Account 2", Funds = 10, AccountType = AccountTypeEnum.Checking}
			}.AsQueryable();

			var mockSet = new Mock<DbSet<Account>>();
			mockSet.As<IQueryable<Account>>().Setup(m => m.Provider).Returns(data.Provider);
			mockSet.As<IQueryable<Account>>().Setup(m => m.Expression).Returns(data.Expression);
			mockSet.As<IQueryable<Account>>().Setup(m => m.ElementType).Returns(data.ElementType);
			mockSet.As<IQueryable<Account>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

			var mockContext = new Mock<BankingDataContext>();
			mockContext.Setup(m => m.Accounts).Returns(mockSet.Object);

			var service = new BankingTransactionService(mockContext.Object);

			//progressively deposit funds to test different amounts
			Account account2 = service.GetAccountById(2);
			Assert.AreEqual(account2.Funds, 10); //check that it starts with only $10
			service.DepositToAccount(2, 5); //plus $5
			Assert.AreEqual(account2.Funds, 15);
			service.DepositToAccount(2, 2000); //plus $2,000
			Assert.AreEqual(account2.Funds, 2015);
			service.DepositToAccount(2, 50000); //plus $50,000
			Assert.AreEqual(account2.Funds, 52015);
		}

		[TestMethod]
		public void CanTransferFunds()
		{
			var data = new List<Account>
			{
				new Account { Id = 1, Name = "Account 1", Funds = 500, AccountType = AccountTypeEnum.IndividualInvestment},
				new Account { Id = 2, Name = "Account 2", Funds = 750, AccountType = AccountTypeEnum.Checking},
			}.AsQueryable();

			var mockSet = new Mock<DbSet<Account>>();
			mockSet.As<IQueryable<Account>>().Setup(m => m.Provider).Returns(data.Provider);
			mockSet.As<IQueryable<Account>>().Setup(m => m.Expression).Returns(data.Expression);
			mockSet.As<IQueryable<Account>>().Setup(m => m.ElementType).Returns(data.ElementType);
			mockSet.As<IQueryable<Account>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

			var mockContext = new Mock<BankingDataContext>();
			mockContext.Setup(m => m.Accounts).Returns(mockSet.Object);

			var service = new BankingTransactionService(mockContext.Object);

			Account account1 = service.GetAccountById(1);
			Account account2 = service.GetAccountById(2);
			//check that ef returns correct starting data
			Assert.AreEqual(account1.Funds, 500);
			Assert.AreEqual(account2.Funds, 750);
			//transfer $100 from 1 to 2
			service.Transfer(1, 2, 100);
			Assert.AreEqual(account1.Funds, 400);
			Assert.AreEqual(account2.Funds, 850);
			//transfer $200 from 2 to 1
			service.Transfer(2, 1, 200);
			Assert.AreEqual(account1.Funds, 600);
			Assert.AreEqual(account2.Funds, 650);
		}

		[TestMethod]
		public void CannotTransferMoreThanAccountHas()
		{
			var data = new List<Account>
			{
				new Account { Id = 1, Name = "Account 1", Funds = 500, AccountType = AccountTypeEnum.IndividualInvestment},
				new Account { Id = 2, Name = "Account 2", Funds = 750, AccountType = AccountTypeEnum.Checking},
			}.AsQueryable();

			var mockSet = new Mock<DbSet<Account>>();
			mockSet.As<IQueryable<Account>>().Setup(m => m.Provider).Returns(data.Provider);
			mockSet.As<IQueryable<Account>>().Setup(m => m.Expression).Returns(data.Expression);
			mockSet.As<IQueryable<Account>>().Setup(m => m.ElementType).Returns(data.ElementType);
			mockSet.As<IQueryable<Account>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

			var mockContext = new Mock<BankingDataContext>();
			mockContext.Setup(m => m.Accounts).Returns(mockSet.Object);

			var service = new BankingTransactionService(mockContext.Object);

			Account account1 = service.GetAccountById(1);
			Account account2 = service.GetAccountById(2);
			//check that ef returns correct starting data
			Assert.AreEqual(account1.Funds, 500);
			Assert.AreEqual(account2.Funds, 750);

			//test that method fails with exception when trying to transfer 1200 from 1 to 2
			try
			{
				service.Transfer(1, 2, 1200);
				Assert.Fail("An exception should have been thrown"); //if this runs an exception was not thrown
			}
			catch (Exception e)
			{
				//check that account funds weren't changed
				Assert.AreEqual(account1.Funds, 500);
				Assert.AreEqual(account2.Funds, 750);
				//check that this operation threw the expected exception
				Assert.IsInstanceOfType(e, typeof(WithdrawalExceedsAvailableFundsException));
			}
		}

		[TestMethod]
		public void IndividualInvestmentAccountCannotWithdrawMoreThan1000()
		{
			var data = new List<Account>
			{
				new Account {Id = 1, Name = "Account 1", Funds = 75000, AccountType = AccountTypeEnum.IndividualInvestment},
			}.AsQueryable();

			var mockSet = new Mock<DbSet<Account>>();
			mockSet.As<IQueryable<Account>>().Setup(m => m.Provider).Returns(data.Provider);
			mockSet.As<IQueryable<Account>>().Setup(m => m.Expression).Returns(data.Expression);
			mockSet.As<IQueryable<Account>>().Setup(m => m.ElementType).Returns(data.ElementType);
			mockSet.As<IQueryable<Account>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

			var mockContext = new Mock<BankingDataContext>();
			mockContext.Setup(m => m.Accounts).Returns(mockSet.Object);

			var service = new BankingTransactionService(mockContext.Object);

			Account account1 = service.GetAccountById(1);

			//can successfully withdraw exactly 1000
			Assert.AreEqual(account1.Funds, 75000);
			service.WithdrawFromAccount(1, 1000);
			Assert.AreEqual(account1.Funds, 74000);

			//cannot withdraw more than 1000
			try
			{
				//try with withdraw 1 cent more than 1000
				service.WithdrawFromAccount(1, 1000.01m);
				Assert.Fail("An exception should have been thrown"); //if this runs an exception was not thrown
			}
			catch (Exception e)
			{
				Assert.IsInstanceOfType(e, typeof(ExceededMaxWithdrawalException));
				Assert.AreEqual(account1.Funds, 74000); //must be unchanged from above
			}
		}

		[TestMethod]
		public void CannotOverdraft()
		{
			var data = new List<Account>
			{
				new Account { Id = 1, Name = "Account 1", Funds = 500, AccountType = AccountTypeEnum.IndividualInvestment},
				new Account { Id = 2, Name = "Account 2", Funds = 750, AccountType = AccountTypeEnum.Checking},
				new Account { Id = 3, Name = "Account 3", Funds = 1000, AccountType = AccountTypeEnum.CorporateInvestment},
			}.AsQueryable();

			var mockSet = new Mock<DbSet<Account>>();
			mockSet.As<IQueryable<Account>>().Setup(m => m.Provider).Returns(data.Provider);
			mockSet.As<IQueryable<Account>>().Setup(m => m.Expression).Returns(data.Expression);
			mockSet.As<IQueryable<Account>>().Setup(m => m.ElementType).Returns(data.ElementType);
			mockSet.As<IQueryable<Account>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

			var mockContext = new Mock<BankingDataContext>();
			mockContext.Setup(m => m.Accounts).Returns(mockSet.Object);

			var service = new BankingTransactionService(mockContext.Object);

			//test that method fails with exception when trying to withdraw 1000 from 500
			try
			{
				service.WithdrawFromAccount(1, 1000);
				Assert.Fail("An exception should have been thrown"); //if this runs an exception was not thrown
			}
			catch (Exception e)
			{
				//check that account 1's funds weren't changed
				mockSet.Verify(m => m.Update(It.IsAny<Account>()), Times.Never);
				//check that this operation threw the expected exception
				Assert.IsInstanceOfType(e, typeof(WithdrawalExceedsAvailableFundsException));
			}
		}

		[TestMethod]
        public void CannotAddNegativeMoney() //tests AddFundsToAccount against negative amount value
		{
			var mockSet = new Mock<DbSet<Account>>();

			var mockContext = new Mock<BankingDataContext>();
			mockContext.Setup(m => m.Accounts).Returns(mockSet.Object);

			var service = new BankingTransactionService(mockContext.Object);
			
	        try
	        {
				//try to add a negative dollar to an account
		        service.AddFundsToAccount(1, -1);
		        Assert.Fail("An exception should have been thrown"); //if this runs an exception was not thrown
			}
	        catch (Exception e)
	        {
				//check that no accounts were updated
				mockSet.Verify(m => m.Update(It.IsAny<Account>()), Times.Never);
				//check that this operation threw the expected exception
				Assert.IsInstanceOfType(e, typeof(ArgumentException));
	        }
		}

		[TestMethod]
		public void CannotRemoveNegativeMoney() //tests RemoveFundsFromAccount against negative amount value
		{
			var mockSet = new Mock<DbSet<Account>>();

			var mockContext = new Mock<BankingDataContext>();
			mockContext.Setup(m => m.Accounts).Returns(mockSet.Object);

			var service = new BankingTransactionService(mockContext.Object);

			try
			{
				//try to take a negative dollar from an account
				service.RemoveFundsFromAccount(1, -1);
				Assert.Fail("An exception should have been thrown"); //if this runs an exception was not thrown
			}
			catch (Exception e)
			{
				//check that no accounts were updated
				mockSet.Verify(m => m.Update(It.IsAny<Account>()), Times.Never);
				//check that this operation threw the expected exception
				Assert.IsInstanceOfType(e, typeof(ArgumentException));
			}
		}
	}
}
