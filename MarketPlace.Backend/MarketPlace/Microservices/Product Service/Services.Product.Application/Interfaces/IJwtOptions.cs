namespace ProductService
{
    public interface IJwtOptions
    {
        public string Key { get; set; }
        public int ExpiredMinutes { get; set; }
    }
}
