using System;

namespace AutomatonAlgorithms.CommandPipeline.ScriptSections.Exceptions
{
    public class PipelineInitException : Exception
    {
        public PipelineInitException(string message) : base(message)
        {
        }
    }
}