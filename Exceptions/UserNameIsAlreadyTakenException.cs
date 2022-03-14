namespace Fakestagram.Exceptions
{
    public class UserNameIsAlreadyTakenException : Exception
    {
        public UserNameIsAlreadyTakenException(string message) : base(message)
        {
        }
    }
}
