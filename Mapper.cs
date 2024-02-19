using AutoMapper;
using EcommerceDotNetCore.DTOs.Category;
using EcommerceDotNetCore.Models;

namespace EcommerceDotNetCore;

public class Mapper:Profile
{
    public Mapper()
    {
        CreateMap<Category, CategoryDto>().ReverseMap();
        CreateMap<Category, CategoryCreateDto>().ReverseMap();
    }
}