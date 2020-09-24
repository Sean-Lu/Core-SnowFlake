using System;

namespace Sean.Core.SnowFlake
{
    public class InvalidSystemClockException : Exception
    {
        public InvalidSystemClockException(string message) : base(message) { }
    }
}