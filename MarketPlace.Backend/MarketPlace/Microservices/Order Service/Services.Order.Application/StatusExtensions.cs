namespace OrderService
{
    public static class StatusExtensions
    {
            private static readonly Dictionary<OrderStatus, string> DisplayNames = new()
        {
            { OrderStatus.Canceled, "Cancelled" },
            { OrderStatus.Ready, "Completed" },
            { OrderStatus.InProgress, "Pending" }
        };

            public static string GetDisplayName(this OrderStatus status)
            {
                return DisplayNames.TryGetValue(status, out var name) ? name : status.ToString();
            }
    }
}
