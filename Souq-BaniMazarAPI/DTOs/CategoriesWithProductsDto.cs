namespace Souq_BaniMazarAPI.DTOs
{
    public class CategoriesWithProductsDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<ProductDto>  Products { get; set; }
        public int ProductsCount { get; set; }

    }
}
