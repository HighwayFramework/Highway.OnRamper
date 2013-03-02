using NDesk.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnRamper
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new Config();

            var p = new OptionSet() {
                { "s|source=", "the {SOURCE} directory of the templated project.",
                   v => config.SourceDirectory = v },
                { "d|destination=", 
                   "the {DESTINATION} directory of the outputted NuGet directories",
                    v => config.DestinationDirectory = v },
                { "c|config=",  "the {CONFIG} directory.", 
                   v => config.ConfigDirectory = v },
                { "h|help",  "show this message and exit", 
                   v => config.ShowHelp =( v != null )},
            };

            List<string> extra;
            try
            {
                extra = p.Parse(args);
                
                if (extra.Count != 0)
                {
                    Console.WriteLine("Unknown arguments : " + string.Join(",",extra.ToArray()));
                    return;
                }

                if (config.ShowHelp)
                {
                    Console.WriteLine("I should show help");
                    return;
                }

                if (config.IsValid() == false) return;

                var factory = new PackageFactory();
                factory.Build(config);
            }
            catch (OptionException e)
            {
                Console.Write("OnRamper: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `OnRamper --help' for more information.");
                return;
            }
        }
    }
}