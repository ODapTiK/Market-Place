﻿using AutoMapper;

namespace OrderService
{
    public class CreateOrderDTO : IMapWith<CreateOrderCommand>
    {
        public List<OrderPointDTO> Points { get; set; } = [];
        public Guid ControlAdminId { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CreateOrderDTO, CreateOrderCommand>()
                .ForMember(cmd => cmd.Points, opt => opt.MapFrom(dto => dto.Points))
                .ForMember(cmd => cmd.ControlAdminId, opt => opt.MapFrom(dto => dto.ControlAdminId));
        }
    }
}
