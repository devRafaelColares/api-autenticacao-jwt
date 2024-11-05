namespace Auth.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Model { get; set; } = null!;
        public string Brand { get; set; } = null!;
        public decimal Price { get; set; }
        public DateTime ManufactureDate { get; set; }
    }
}