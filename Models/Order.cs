using EcommerceDotNetCore.Enums;
using System.ComponentModel.DataAnnotations;

namespace EcommerceDotNetCore.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        public string UserId { get; set; }

        public DateTime OrderDate { get; set; }

        //TODO: Make Pending as a default status
        public OrderStatus Status { get; set; } = OrderStatus.Completed; 

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
