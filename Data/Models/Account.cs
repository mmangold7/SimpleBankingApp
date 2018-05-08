namespace Data.Models
{
	public class Account
    {
	    public int Id { get; set; }
		public string Name { get; set; }
		public virtual Owner Owner { get; set; } 
		public virtual Bank Bank { get; set; }
		public virtual AccountType AccountType { get; set; }
		public decimal Funds { get; set; }
    }
}
