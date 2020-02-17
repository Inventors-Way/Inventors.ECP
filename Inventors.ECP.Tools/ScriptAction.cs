using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventors.ECP.Tools
{
    [Verb("execute", HelpText = "Execute a script")]
    public class ScriptAction :
        CommandAction
    {
        public override int Run()
        {
            Console.WriteLine("TODO Implement execution of scripts");
            return 0;
        }
    }
}
