namespace UserService
{
    public abstract class BaseUser
    {
        public Guid Id { get; set; }
        public string? Logo { get; set; } = null;
    }
}
