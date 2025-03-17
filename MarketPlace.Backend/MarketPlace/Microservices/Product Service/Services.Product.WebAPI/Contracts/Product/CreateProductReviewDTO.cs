using AutoMapper;

namespace ProductService
{
    public class CreateProductReviewDTO : IMapWith<CreateProductReviewCommand>
    {
        public Guid ProductId { get; set; }
        public int Raiting { get; set; }
        public string Description { get; set; } = string.Empty;

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CreateProductReviewDTO, CreateProductReviewCommand>()
                .ForMember(cmd => cmd.ProductId, opt => opt.MapFrom(dto => dto.ProductId))
                .ForMember(cmd => cmd.Raiting, opt => opt.MapFrom(dto => dto.Raiting))
                .ForMember(cmd => cmd.Description, opt => opt.MapFrom(dto => dto.Description));
        }
    }
}
