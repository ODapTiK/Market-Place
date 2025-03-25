using AutoMapper;

namespace OrderService
{
    public class CreateOrderDTO : IMapWith<CreateOrderCommand>
    {
        public List<OrderPoint> Points { get; set; } = [];
        public decimal TotalPrice { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CreateOrderDTO, CreateOrderCommand>()
                .ForMember(cmd => cmd.Points, opt => opt.MapFrom(dto => dto.Points))
                .ForMember(cmd => cmd.TotalPrice, opt => opt.MapFrom(dto => dto.TotalPrice));
        }
    }
}
