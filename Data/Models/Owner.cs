using System.Collections.Generic;

namespace Data.Models
{
	public class Owner
    {
	    public int Id { get; set; }
		public string Name { get; set; }
	    public virtual IEnumerable<Account> Accounts { get; set; }
    }
}