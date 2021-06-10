using System;

namespace AutomatonAlgorithms.Parsers.Exceptions
{
    public class AutomatonFileFormatException : Exception
    {
        public AutomatonFileFormatException()
        {
        }

        public AutomatonFileFormatException(string message) : base(message)
        {
        }
    }
}