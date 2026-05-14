namespace ASPStellysCake.Models
{
    public class Favorite
    {
        public int Id { get; set; }
        public string? CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public DateTime AddedOn { get; set; } = DateTime.Now;
    }
}
