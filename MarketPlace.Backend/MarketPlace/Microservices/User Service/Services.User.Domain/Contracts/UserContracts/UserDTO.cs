using AutoMapper;

namespace UserService
{
    public class UserDTO : IMapWith<User>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; } 

        public void Mapping(Profile profile)
        {
            profile.CreateMap<UserDTO, User>()
                .ForMember(u => u.Id, opt => opt.MapFrom(dto => dto.Id))
                .ForMember(u => u.Name, opt => opt.MapFrom(dto => dto.Name))
                .ForMember(u => u.Surname, opt => opt.MapFrom(dto => dto.Surname))
                .ForMember(u => u.BirthDate, opt => opt.MapFrom(dto => dto.Surname));
        }
    }
}
