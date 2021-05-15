using System;

namespace AutomatonAlgorithms.CommandPipeline.ScriptSections.Exceptions
{
    public abstract class ScriptException : Exception
    {
        protected ScriptException()
        {
        }

        protected ScriptException(string message) : base(message)
        {
        }
    }
}