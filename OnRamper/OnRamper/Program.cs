using NDesk.Options;
using System;
using System.Collections.Generic;
using System.IO;
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
                { "s|source=", "the source {dir} of the templated project.",
                   v => config.SourceDirectory = v },
                { "d|destination=", 
                   "the destination {dir} of the outputted NuGet directories",
                    v => config.DestinationDirectory = v },
                { "c|config=",  "the config {dir} which contains your nuspec files.", 
                   v => config.ConfigDirectory = v },
                { "h|help",  "show this message and exit", 
                   v => config.ShowHelp =( v != null )},
                { "x|execute=", "execute {NuGet.exe} when creation is complete",
                    v => config.ExecutePath = v }
            };

            List<string> extra;
            try
            {
                extra = p.Parse(args);

                DisplayBanner();

                Console.ForegroundColor = ConsoleColor.Red;
                if (config.IsValid() == false)
                {
                    config.ShowHelp = true;
                }
                Console.ResetColor();

                if (config.ShowHelp)
                {
                    p.WriteOptionDescriptions(Console.Out);
                    return;
                }

                var factory = new PackageFactory();
                factory.Build(config);
            }
            catch (OptionException e)
            {
                Console.Write("OnRamper: ");
                p.WriteOptionDescriptions(Console.Out);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ResetColor();
                return;
            }
        }
        private static void DisplayBanner()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(" OOOOO  NN   NN RRRRRR     A    MM   MM PPPPPP  EEEEEEE RRRRRR ");
            Console.WriteLine("OO   OO NNN  NN RR   RR   AAA   MMM MMM PP   PP EE      RR   RR");
            Console.WriteLine("OO   OO NNNN NN RRRRRR   AA AA  MM M MM PPPPPP  EEEEEEE RRRRRR ");
            Console.WriteLine("OO   OO NN NNNN RR  RR  AA   AA MM   MM PP      EE      RR  RR ");
            Console.WriteLine("OO   OO NN  NNN RR   RR AAAAAAA MM   MM PP      EE      RR   RR");
            Console.WriteLine(" OOOOO  NN   NN RR   RR AA   AA MM   MM PP      EEEEEEE RR   RR");
            Console.WriteLine();
            Console.WriteLine("A tool for the creation of NuGet packages which kickstart projects");
            Console.ResetColor();
        }
    }
}