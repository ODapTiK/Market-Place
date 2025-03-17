using AutoMapper;

namespace ProductService
{
    public class DeleteProductReviewDTO : IMapWith<DeleteProductReviewCommand>
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<DeleteProductReviewDTO, DeleteProductReviewCommand>()
                .ForMember(cmd => cmd.Id, opt => opt.MapFrom(dto => dto.Id))
                .ForMember(cmd => cmd.ProductId, opt => opt.MapFrom(dto => dto.ProductId));
        }
    }
}
