namespace OrderService
{
    public class LackOfRightsException : Exception
    {
        public LackOfRightsException(string entityName, object entityKey, string action) :
            base($"Action \"{action}\" can not be executed because entity \"{entityName}\"({entityKey}) have not enough rights!")
        { }
    }
}
