namespace ASPStellysCake.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CatNumber { get; set; }
        public int CategoryId { get; set; }
        public Category Categories { get; set; }
        public decimal Weight { get; set; }
        public bool GlutenFree { get; set; }
        public string Description { get; set; }
        public string ImageURL { get; set; }
        public decimal Price { get; set; }
        public DateTime RegisterOn { get; set; } = DateTime.Now;
        public ICollection<Order> Orders { get; set; }
    }
}
