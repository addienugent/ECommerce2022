
namespace Library.ECommerceApp
{
    public partial class Product
    {
        public int Id { get; set; }     // Model

        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }
        public int AssignedUser { get; set; }

        public Product()
        {

        }

        public override string ToString()
        {
            return $"{Id}\t{Name}\t$ {Price}\t  {Quantity}\t\t{Description}";
        }
    }
}   