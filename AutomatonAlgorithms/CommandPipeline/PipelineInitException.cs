using System;

namespace AutomatonAlgorithms.CommandPipeline
{
    public class PipelineInitException : Exception
    {
        public PipelineInitException(string message) : base(message) {}
    }
}