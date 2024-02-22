namespace EcommerceDotNetCore.DTOs.Cart
{
    public class CartDto
    {
        public int CartId { get; set; }
        public string UserId { get; set; }
        public string Token { get; set; }
        public DateTime Timestamp { get; set; }
        public List<CartItemDto> CartItems { get; set; }
    }
}
