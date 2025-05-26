namespace OrderService
{
    public interface IObsoleteOrdersClearingSettings
    {
        public int RemoveOrdersRepeatTimeoutHours { get; set; }
    }
}
