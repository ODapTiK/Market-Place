﻿namespace OrderService
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string name, object key) : base($"Entity |\"{name}\"({key}) not found!") { }
    }
}
