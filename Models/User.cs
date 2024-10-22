using System.ComponentModel.DataAnnotations;

namespace PersonalWebsite.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<CartItem> CartItems { get; set; }
    }
}
