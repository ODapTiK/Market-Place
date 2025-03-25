using AutoMapper;

namespace UserService
{
    public class ManufacturerDTO : IMapWith<Manufacturer>
    {
        public Guid Id { get; set; }
        public string Organization { get; set; } = string.Empty;

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ManufacturerDTO, Manufacturer>()
                .ForMember(m => m.Id, opt => opt.MapFrom(dto => dto.Id))
                .ForMember(m => m.Organization, opt => opt.MapFrom(dto => dto.Organization));
        }
    }
}
