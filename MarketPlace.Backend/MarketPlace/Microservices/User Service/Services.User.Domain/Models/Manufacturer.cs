namespace UserService
{
    public class Manufacturer : BaseUser
    {
        public string Organization { get; set; } = string.Empty;
        public List<Guid> OrganizationProductsId { get; set; } = [];
    }
}
