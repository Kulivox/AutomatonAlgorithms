using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using NLog;

namespace AutomatonAlgorithms
{
    internal static class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static RootCommand PrepareCommandLineParsing()
        {
            var rootCommand = new RootCommand(
                "Executes .pln scripts (does automaton operations specified in these files)")
            {
                new Option<string>(
                    new[] {"-i", "--input"},
                    () => ".",
                    "Path to .pln scripts, default is current directory"
                ),
                new Option<string>(
                    new[] {"-c", "--config"},
                    () => "./config.cfg",
                    "Path to configuration file for .pln script executors, default is ./config.cfg"
                ),
                new Option<int>(
                    new[] {"-t", "--threads"},
                    () => 5,
                    "Max number of threads that should be spawned at the same time if parallelization is possible"
                    + " Default is 5, minimum is 1 and max is 30"
                )
            };
            return rootCommand;
        }

        private static void ConfigureLogger()
        {
            var config = new NLog.Config.LoggingConfiguration();
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "logfile.log" };
            var logConsole = new NLog.Targets.ConsoleTarget
            {
                Name = "logconsole",
                Layout = "${longdate} | ${level:uppercase=true} | ${message}"
            };
            
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logConsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
            
            LogManager.Configuration = config;
        }

        private static int Main(string[] args)
        {
            ConfigureLogger();
            var command = PrepareCommandLineParsing();

            command.Handler = CommandHandler.Create<string, string, int>((input, config, threads) =>
                {
                    if (!File.Exists(config))
                    {
                        Logger.Error("Path to config doesn't lead to existing file");
                        return;
                    }

                    if (!Directory.Exists(input))
                    {
                        Logger.Error("Path to input scripts doesn't lead to valid directory");
                        return;
                    }

                    if (threads is < 1 or > 30)
                    {
                        Logger.Error("Incorrect number of threads");
                        return;
                    }


                    ScriptExecution.Start(input, config, threads);
                }
            );
            
            
            
            return command.Invoke(args);
        }
    }
}