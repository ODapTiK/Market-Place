namespace OrderService
{
    public static class RoleExtensions
    {
        private static readonly Dictionary<Role, string> DisplayNames = new()
        {
            { Role.Admin, "Admin" },
            { Role.Manufacturer, "Manufacturer" },
            { Role.User, "User" },
        };

        public static string GetDisplayName(this Role role)
        {
            return DisplayNames.TryGetValue(role, out var name) ? name : role.ToString();
        }
    }
}
