﻿namespace OrderService
{
    public interface IRabbitMqOptions
    {
        public string UserName { get; set; } 
        public string Password { get; set; } 
        public string VirtualHost { get; set; }
        public string HostName { get; set; }
        public int Port { get; set; }
    }
}
