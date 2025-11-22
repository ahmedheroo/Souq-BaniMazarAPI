namespace Souq_BaniMazarAPI.Models
{
    public class Products
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImgUrl { get; set; }
        public int Quantity { get; set; }
        public string SellerId { get; set; }
        public int CategoriesId { get; set; }
        public int StatusId { get; set; }
        public ApplicationUser Seller { get; set; }
        public virtual Categories Categories { get; set; }
        public virtual ProductStatus Status { get; set; }

    }
}
