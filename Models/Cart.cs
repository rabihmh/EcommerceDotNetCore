using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EcommerceDotNetCore.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }
        public string UserId { get; set; } 
        public string Token { get; set; } 
        public DateTime Timestamp { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }=new List<CartItem>();
    }
}
