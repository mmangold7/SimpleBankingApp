# SimpleBankingApp

A simple banking application without a user or web service interface.

This application uses .NET Core, Entity Framework Core, MSTest, and Moq.

Projects:

The Data project is a .NET core class library data access layer using entity framework core. There is a data context defined with dbsets.
The data model follows these rules:
  Bank objects have a name and a list of accounts
  Account objects have an owner, account type, funds, and name
  Owner objects have a list of accounts and a name
  AccountType objects have a name using the values from an enum: Checking, Corporate Investment, Individual Investment
  
The Business Layer is a .NET core class library which contains a business logic service with read/update logic.
The BankingTransactionService receives a data context at construction and contains business methods for removing and adding funds, withdrawing and depositing funds, and transfering funds.
It also contains a method to get an account object by ID, and to throw an exception if a negative amount was passed in to the records that update the data context.

The UnitTests project uses MSTest and Moq. Moq provides a way to easily test collections of mock data that conform to the structure of the EF DbContext.
Each method:
  defines relevant data
  sets up mock accounts within a context
  calls business service methods
  uses either Moq methods or Assert statements to show that the app results match the expected results
  
The application, as it stands, should be easy to hook up to a Web API project and a database such as SQL Server. More testing could be done to ensure that the different types of accounts can all do the same things. Other types of test data could be used to ensure proper transactional arithmetic.
