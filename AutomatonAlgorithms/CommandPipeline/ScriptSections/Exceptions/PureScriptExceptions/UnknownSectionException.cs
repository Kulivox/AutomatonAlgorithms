namespace AutomatonAlgorithms.CommandPipeline.ScriptSections.Exceptions.PureScriptExceptions
{
    public class UnknownSectionException : ScriptException
    {
        public UnknownSectionException()
        {
        }

        public UnknownSectionException(string message) : base(message)
        {
        }
    }
}