using EcommerceDotNetCore.Data;
using EcommerceDotNetCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EcommerceDotNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("Checkout")]
        public async Task<IActionResult> Checkout()
        {
            try
            {
                var emailClaim = ((ClaimsIdentity)HttpContext.User.Identity).FindFirst(ClaimTypes.Email)?.Value;
                var userId = "";
                if (emailClaim != null)
                {
                    var userFromDb = await _context.Users.FirstOrDefaultAsync(u => u.Email == emailClaim);
                    userId = userFromDb?.Id;
                }
                else
                {
                    throw new Exception("Unknown User");
                }

                var order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.Now
                };

                var cartItems = await _context.CartItems
                    .Where(ci => ci.Cart.UserId == userId)
                    .ToListAsync();

                foreach (var cartItem in cartItems)
                {
                    var product = await _context.Products.FindAsync(cartItem.ProductId);

                    if (product != null)
                    {
                        order.OrderItems.Add(new OrderItem
                        {
                            ProductId = cartItem.ProductId,
                            Quantity = cartItem.Quantity,
                            Price = product.Price
                        });
                    }

                    _context.CartItems.Remove(cartItem);
                }

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                return Ok(new { orderId = order.OrderId,Message="Order completed" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
