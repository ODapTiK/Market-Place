namespace OrderService
{
    public class EntityAlreadyExistsException : Exception
    {
        public EntityAlreadyExistsException(string name, object key) : base($"Entity \"{name}\"({key}) already exists!") { }
    }
}
