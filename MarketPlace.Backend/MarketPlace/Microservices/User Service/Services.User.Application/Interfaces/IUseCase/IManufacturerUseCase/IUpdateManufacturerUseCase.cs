namespace UserService
{
    public interface IUpdateManufacturerUseCase
    {
        public Task Execute(ManufacturerDTO manufacturerDTO);
    }
}
