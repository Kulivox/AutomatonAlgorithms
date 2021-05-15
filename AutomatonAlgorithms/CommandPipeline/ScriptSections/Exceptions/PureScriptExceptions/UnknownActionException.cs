namespace AutomatonAlgorithms.CommandPipeline.ScriptSections.Exceptions.PureScriptExceptions
{
    public class UnknownActionException : ScriptException
    {
        public UnknownActionException()
        {
        }

        public UnknownActionException(string message) : base(message)
        {
            
        }
    }
}