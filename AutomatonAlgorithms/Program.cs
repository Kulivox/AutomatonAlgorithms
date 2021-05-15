using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.IO;

namespace AutomatonAlgorithms
{
    internal static class Program
    {
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

        private static int Main(string[] args)
        {
            var command = PrepareCommandLineParsing();

            command.Handler = CommandHandler.Create<string, string, int>((input, config, threads) =>
                {
                    if (!File.Exists(config))
                    {
                        Console.WriteLine("Path to config doesn't lead to existing file");
                        return;
                    }

                    if (!Directory.Exists(input))
                    {
                        Console.WriteLine("Path to input scripts doesn't lead to valid directory");
                        return;
                    }

                    if (threads is < 1 or > 30)
                    {
                        Console.WriteLine("Incorrect number of threads");
                        return;
                    }


                    ScriptExecution.Start(input, config, threads);
                }
            );
            // var watch = new Stopwatch();
            // watch.Start();
            // var asd = command.Invoke(args);
            // watch.Stop();
            // Console.WriteLine(watch.Elapsed.ToString("mm\\:ss\\.ff"));
            
            return command.Invoke(args);
        }
    }
}