namespace ProductService
{
    public class LackOfRightException : Exception
    {
        public LackOfRightException(string entityName, object entityKey, string action) : 
            base($"Action \"{action}\" can not be executed because entity \"{entityName}\"({entityKey}) have not enough rights!") { }
    }
}
