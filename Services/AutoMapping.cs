using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs;
using Entities;

namespace Services
{
    internal class AutoMapping: Profile
    {

        public AutoMapping()
        {
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<User, LoginUserDTO>().ReverseMap();
            CreateMap<Order, OrderDTO>().ReverseMap();
            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<OrderItem, OrderItemDTO>().ReverseMap();


        }
    }
}
