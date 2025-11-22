using System.Collections;

namespace Souq_BaniMazarAPI.Models
{
    public class Categories
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImgUrl { get; set; }
        public virtual ICollection<Products> Products { get; set; } = new List<Products>();
    }
}
