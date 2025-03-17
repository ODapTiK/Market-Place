using AutoMapper;

namespace UserService
{
    public interface IMapWith<T> where T : class
    {
        void Mapping(Profile profile) => profile.CreateMap(typeof(T), GetType());
    }
}
