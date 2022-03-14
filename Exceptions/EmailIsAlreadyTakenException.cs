namespace Fakestagram.Exceptions
{
    public class EmailIsAlreadyTakenException : Exception
    {
        public EmailIsAlreadyTakenException(string message) : base(message)
        {

        }
    }
}
