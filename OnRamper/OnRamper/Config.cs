using NDesk.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnRamper
{
    public class Config
    {
        public string SourceDirectory { get; set; }
        public string DestinationDirectory { get; set; }
        public string ConfigDirectory { get; set; }
        public string ExecutePath { get; set; }

        public bool ShouldExecute
        {
            get
            {
                return ExecutePath != null;
            }
        }

        public bool ShowHelp { get; set; }

        public bool IsValid()
        {
            if (Directory.Exists(SourceDirectory) == false)
            {
                Console.WriteLine("Source directory does not exist");
                return false;
            }
            if (Directory.Exists(DestinationDirectory) == false)
            {
                Console.WriteLine("Destination directory does not exist");
                return false;
            }
            if (Directory.Exists(ConfigDirectory) == false)
            {
                Console.WriteLine("Config directory does not exist");
                return false;
            }
            if (ShouldExecute)
            {
                if (File.Exists(ExecutePath) == false)
                {
                    Console.WriteLine("Execute does not point to a file.");
                    return false;
                }

                if (Path.GetExtension(ExecutePath).Equals("exe",StringComparison.InvariantCultureIgnoreCase))
                {
                    Console.WriteLine("Execute is not an executable.");
                    return false;
                }
            }

            return true;
        }
    }
}
