using System;
using AutomatonAlgorithms.CommandPipeline.ScriptSections.Exceptions.PureScriptExceptions;
using AutomatonAlgorithms.CommandPipeline.ScriptSections.Init;
using AutomatonAlgorithms.CommandPipeline.ScriptSections.Procedure;
using AutomatonAlgorithms.CommandPipeline.ScriptSections.Transformation;
using AutomatonAlgorithms.Configurations;
using AutomatonAlgorithms.Parsers;

namespace AutomatonAlgorithms.CommandPipeline.ScriptSections
{
    public class SectionFactory
    {
        private readonly IConfiguration _configuration;

        private readonly AutomatonLoader _loader;

        public SectionFactory(AutomatonLoader loader, IConfiguration configuration)
        {
            _loader = loader;
            _configuration = configuration;
        }

        public Section BuildSection(SectionType type)
        {
            return type switch
            {
                SectionType.Init => new InitSection(_loader, _configuration),
                SectionType.Procedures => new ProceduresSection(_configuration),
                SectionType.Transformations => new TransformationsSection(_configuration),
                _ => throw new UnknownSectionException()
            };
        }

        public Section BuildSection(string sectionName)
        {
            if (!Enum.TryParse(sectionName, true, out SectionType result))
                throw new UnknownSectionException();

            return BuildSection(result);
        }
    }
}