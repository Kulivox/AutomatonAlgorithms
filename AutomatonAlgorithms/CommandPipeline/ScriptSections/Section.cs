using System;
using System.Collections.Generic;
using System.Linq;
using AutomatonAlgorithms.Configurations;
using AutomatonAlgorithms.DataStructures.Automatons;

namespace AutomatonAlgorithms.CommandPipeline.ScriptSections
{
    public abstract class Section
    {
        protected Section(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public abstract int Priority { get; }

        public IConfiguration Configuration { get; }

        public abstract void ExecuteSection(string sectionString, Dictionary<string, Automaton> automatonVariables,
            Dictionary<string, string> stringVariables);

        protected Dictionary<string, T> GetOperationDictionary<T>(HashSet<string> uniqueTransformationNames)
        {
            var resultType = typeof(T);
            var transformationDict = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(t => resultType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .Where(t => uniqueTransformationNames.Contains(t.Name))
                .ToDictionary(type => type.Name,
                    type => (T) Activator.CreateInstance(type, Configuration));
            return transformationDict;
        }
    }
}