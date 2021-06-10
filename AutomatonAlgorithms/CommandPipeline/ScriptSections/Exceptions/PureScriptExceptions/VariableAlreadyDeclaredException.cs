namespace AutomatonAlgorithms.CommandPipeline.ScriptSections.Exceptions.PureScriptExceptions
{
    public class VariableAlreadyDeclaredException : ScriptException
    {
        public VariableAlreadyDeclaredException()
        {
        }

        public VariableAlreadyDeclaredException(string message) : base(message)
        {
        }
    }
}