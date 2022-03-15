namespace Fakestagram.Exceptions
{
    public class EntityAlreadyDislikedException : Exception
    {
        public EntityAlreadyDislikedException(string message) : base(message)
        {
        }
    }
}
