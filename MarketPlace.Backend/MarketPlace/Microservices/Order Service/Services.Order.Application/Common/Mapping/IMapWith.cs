using AutoMapper;

namespace OrderService
{
    public interface IMapWith<T> where T : class
    {
        void Mapping(Profile profile) => profile.CreateMap(typeof(T), GetType());
    }
}
