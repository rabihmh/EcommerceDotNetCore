using AutoMapper;
using EcommerceDotNetCore.DTOs.Cart;
using EcommerceDotNetCore.DTOs.Category;
using EcommerceDotNetCore.DTOs.Product;
using EcommerceDotNetCore.Models;

namespace EcommerceDotNetCore;

public class Mapper:Profile
{
    public Mapper()
    {
        CreateMap<Category, CategoryDto>().ReverseMap();
        CreateMap<Category, CategoryCreateDto>().ReverseMap();
        CreateMap<Product, ProductDto>().ReverseMap();
        CreateMap<Product, ProductCreateDto>().ReverseMap();
        CreateMap<Cart, CartDto>();
        CreateMap<CartItem, CartItemDto>();
    }
}