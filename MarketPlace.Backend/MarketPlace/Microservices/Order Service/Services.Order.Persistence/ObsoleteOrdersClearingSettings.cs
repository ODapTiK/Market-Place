namespace OrderService
{
    public class ObsoleteOrdersClearingSettings : IObsoleteOrdersClearingSettings
    {
        public int RemoveOrdersRepeatTimeoutHours { get; set; }
    }
}
