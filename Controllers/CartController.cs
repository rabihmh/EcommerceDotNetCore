using System.Security.Claims;
using AutoMapper;
using EcommerceDotNetCore.Data;
using EcommerceDotNetCore.DTOs.Cart;
using EcommerceDotNetCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceDotNetCore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CartController> _logger;
        private readonly IMapper _mapper;


        public CartController(ApplicationDbContext context, ILogger<CartController> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }
        [HttpGet("{token}")]
        public async Task<IActionResult> GetCart( string token)
        {
            if (token== null)
            {
                return BadRequest("Token is required");
            }
            var cart = await _context.Carts.Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.Token == token);
           var cartdto=_mapper.Map<CartDto>(cart);
            return Ok(cartdto);
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] CartItemUpsertDTO cartItemDTO, string token = null)
        {
            var emailClaim = ((ClaimsIdentity)HttpContext.User.Identity).FindFirst(ClaimTypes.Email)?.Value;
            var userId = "";
            if (emailClaim != null)
            {
                var userFromDb = await _context.Users.FirstOrDefaultAsync(u => u.Email == emailClaim);
                userId = userFromDb?.Id;
                _logger.LogInformation($"User email: {emailClaim}, User Id: {userId}");
            }

            if (string.IsNullOrEmpty(token))
            {
                token = Guid.NewGuid().ToString();
            }

            var cart = await _context.Carts.Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.Token == token);

            if (cart == null)
            {
                cart = new Cart
                {
                    Token = token,
                    Timestamp = DateTime.Now,
                    UserId = userId
                };
                _context.Carts.Add(cart);
            }

            var existingCartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == cartItemDTO.ProductId);
            if (existingCartItem != null)
            {
                existingCartItem.Quantity += cartItemDTO.Quantity;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    ProductId = cartItemDTO.ProductId,
                    Quantity = cartItemDTO.Quantity
                });
            }

            await _context.SaveChangesAsync();
            var carDto = _mapper.Map<CartDto>(cart);

            return StatusCode(201, new { Token=token });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCartItem([FromBody] CartItemUpsertDTO cartItemDTO, string token)
        {
            var cart = await _context.Carts.Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.Token == token);

            if (cart == null)
            {
                return BadRequest("Cart not found");
            }

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == cartItemDTO.ProductId);

            if (cartItem == null)
            {
                return BadRequest("Item not found in cart");
            }

            cartItem.Quantity = cartItemDTO.Quantity;

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Cart item updated successfully" });
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveCartItem([FromBody] CartItemUpsertDTO cartItemDTO, string token)
        {
            var cart = await _context.Carts.Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.Token == token);
            if (cart == null)
            {
                return BadRequest("Cart not found");
            }

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == cartItemDTO.ProductId);
            if (cartItem == null)
            {
                return BadRequest("Item not found in cart");
            }

            cart.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart(string token)
        {
            var cart = await _context.Carts.Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.Token == token);
            if (cart == null)
            {
                return BadRequest("Cart not found");
            }

            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }


}
