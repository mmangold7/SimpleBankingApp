using System;

namespace Data.Models
{
    public class AccountType
    {
	    private AccountType(AccountTypeEnum @enum)
	    {
		    Id = (int)@enum;
		    Name = @enum.ToString();
	    }

		//for ef
	    protected AccountType() { }

		public int Id { get; set; }
		public string Name { get; set; }

		//allows more easy use of this type
	    public static implicit operator AccountType(AccountTypeEnum @enum) => new AccountType(@enum);
	    public static implicit operator AccountTypeEnum(AccountType faculty) => (AccountTypeEnum)faculty.Id;
	}
}
