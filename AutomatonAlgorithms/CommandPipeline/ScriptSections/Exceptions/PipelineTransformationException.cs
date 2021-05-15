using System;

namespace AutomatonAlgorithms.CommandPipeline.ScriptSections.Exceptions
{
    public class PipelineTransformationException : Exception
    {
        public PipelineTransformationException(string message) : base(message)
        {
        }
    }
}