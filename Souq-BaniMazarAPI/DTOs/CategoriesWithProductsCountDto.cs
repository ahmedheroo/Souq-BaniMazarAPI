namespace Souq_BaniMazarAPI.DTOs
{
    public class CategoriesWithProductsCountDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ImgUrl { get; set; }
        public int ProductsCount { get; set; }
    }
}
