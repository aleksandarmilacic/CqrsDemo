﻿using AutoMapper;
using CqrsDemo.Application.Models.DTOs.Order;
using CqrsDemo.Domain.Entities.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CqrsDemo.Application.Mapper
{
    public class OrderMapping : Profile
    {
        public OrderMapping()
        {
            // Map Order -> OrderDTO
            CreateMap<Order, OrderDTO>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id)); ;

        }
    }
}
