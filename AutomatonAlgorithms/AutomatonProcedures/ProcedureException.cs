using System;
using AutomatonAlgorithms.CommandPipeline.ScriptSections.Exceptions;

namespace AutomatonAlgorithms.AutomatonProcedures
{
    public class ProcedureException : ScriptException
    {
        public ProcedureException()
        {
        }

        public ProcedureException(string message) : base(message)
        {
        }

        public ProcedureException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}