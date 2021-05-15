namespace AutomatonAlgorithms.CommandPipeline.ScriptSections.Exceptions.FailedOperations
{
    public class TransformationFailedException : ScriptException
    {
        public TransformationFailedException()
        {
        }

        public TransformationFailedException(string message) : base(message)
        {
        }
    }
}