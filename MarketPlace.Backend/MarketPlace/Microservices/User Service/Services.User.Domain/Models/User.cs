namespace UserService
{
    public class User : BaseUser
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public List<Guid> UserOrdersId { get; set; } = [];
        public List<Notification> UserNotifications { get; set; } = [];
    }
}
