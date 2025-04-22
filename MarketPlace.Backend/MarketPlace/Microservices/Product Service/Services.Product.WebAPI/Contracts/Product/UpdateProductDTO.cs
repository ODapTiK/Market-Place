using AutoMapper;

namespace ProductService
{
    public class UpdateProductDTO : IMapWith<UpdateProductCommand>
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? Type { get; set; }
        public string? Image { get; set; }
        public double Price { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<UpdateProductDTO, UpdateProductCommand>()
                .ForMember(cmd => cmd.Id, opt => opt.MapFrom(dto => dto.Id))
                .ForMember(cmd => cmd.Name, opt => opt.MapFrom(dto => dto.Name))
                .ForMember(cmd => cmd.Description, opt => opt.MapFrom(dto => dto.Description))
                .ForMember(cmd => cmd.Category, opt => opt.MapFrom(dto => dto.Category))
                .ForMember(cmd => cmd.Type, opt => opt.MapFrom(dto => dto.Type))
                .ForMember(cmd => cmd.Image, opt => opt.MapFrom(dto => dto.Image))
                .ForMember(cmd => cmd.Price, opt => opt.MapFrom(dto => dto.Price));
        }
    }
}
