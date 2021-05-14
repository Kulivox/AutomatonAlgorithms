using System;

namespace AutomatonAlgorithms.CommandPipeline
{
    public class PipelineTransformationException : Exception
    {
        public PipelineTransformationException(string message) : base(message) {}
    }
}