using CommandLine;
using System;

namespace Inventors.ECP.Tools
{
    class Program
    {
        static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<ScriptAction,
                                                 ScanAction>(args).MapResult(
                            (ScriptAction opts) => opts.Run(),
                            (ScanAction opts) => opts.Run(),
                            errs => 1);
        }
    }
}
