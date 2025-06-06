namespace UserService
{
    public class Admin : BaseUser
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public List<Guid> AdminControlOrdersId { get; set; } = [];
        public List<Notification> AdminNotifications { get; set; } = [];
    }
}
