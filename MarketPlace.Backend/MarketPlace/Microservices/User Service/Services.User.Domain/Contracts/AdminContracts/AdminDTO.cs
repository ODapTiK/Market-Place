using AutoMapper;

namespace UserService
{
    public class AdminDTO : IMapWith<Admin>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AdminDTO, Admin>()
                .ForMember(a => a.Id, opt => opt.MapFrom(dto => dto.Id))
                .ForMember(a => a.Name, opt => opt.MapFrom(dto => dto.Name))
                .ForMember(a => a.Surname, opt => opt.MapFrom(dto => dto.Surname));
        }
    }
}
