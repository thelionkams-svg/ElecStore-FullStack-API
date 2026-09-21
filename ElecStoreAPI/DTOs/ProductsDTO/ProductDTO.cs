namespace ElecStoreAPI.DTOs.ProductsDTO
{
    public class ProductDTO
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int CategoryID { get; set; }
        public decimal Price { get; set; }
        public int QuantityInStock { get; set; }
    }
}
