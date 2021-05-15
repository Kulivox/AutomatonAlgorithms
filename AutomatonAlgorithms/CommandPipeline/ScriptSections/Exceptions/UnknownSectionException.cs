using System;

namespace AutomatonAlgorithms.CommandPipeline.ScriptSections.Exceptions
{
    public class UnknownSectionException : Exception
    {
        public UnknownSectionException()
        {
        }

        public UnknownSectionException(string message) : base(message)
        {
        }
    }
}