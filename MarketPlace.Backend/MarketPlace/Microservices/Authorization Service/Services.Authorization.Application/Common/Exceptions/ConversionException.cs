namespace AuthorizationService
{
    public class ConversionException : Exception
    {
        public ConversionException(string convertFrom, string convertTo) :
            base($"An error occured when trying to transform \"{convertFrom}\" object in \"{convertTo}\" object!")
        { }
    }
}
