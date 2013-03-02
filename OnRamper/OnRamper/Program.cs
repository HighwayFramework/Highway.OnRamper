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
            bool show_help = false;
            string source = null;
            string destination = null;
            string config = null;

            var p = new OptionSet() {
                { "s|source=", "the {SOURCE} directory of the templated project.",
                   v => source = v },
                { "d|destination=", 
                   "the {DESTINATION} directory of the outputted NuGet directories",
                    v => destination = v },
                { "c|config=",  "the {CONFIG} directory.", 
                   v => config = v },
                { "h|help",  "show this message and exit", 
                   v => show_help = v != null },
            };

            List<string> extra;
            try
            {
                extra = p.Parse(args);
                Console.WriteLine(source);
                Console.WriteLine(destination);
                Console.WriteLine(config);
                foreach (string item in extra)
                {
                    Console.WriteLine(item);
                }
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
