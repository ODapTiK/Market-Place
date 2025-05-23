﻿namespace OrderService
{
    public class ObsoleteOrdersCollector : IObsoleteOrderCollector
    {
        private readonly IOrderRepository _orderRepository;

        public ObsoleteOrdersCollector(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task RemoveObsoleteOrderAsync(Order order, CancellationToken cancellationToken)
        {
            await _orderRepository.DeleteAsync(order, cancellationToken);
        }
    }
}
