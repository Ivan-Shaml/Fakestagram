﻿namespace Fakestagram.Exceptions
{
    public class RefreshTokenNotFoundException : Exception
    {
        public RefreshTokenNotFoundException(string message)
            : base(message)
        {
        }
    }
}
